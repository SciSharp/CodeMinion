using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CodeMinion.Core;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;
using CodeMinion.Parser;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator.PyTorch
{
    public partial class ApiGenerator : ICodeGenerator
    {
        private CodeGenerator _generator;
        public ApiGenerator()
        {
            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            var test_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\test\\";
            _generator = new CodeGenerator
            {
                //PrintModelJson=true,  // <-- if enabled prints the declaration model as JSON for debugging reasons
                NameSpace = "Torch",
                PythonModuleName = "torch",
                StaticModuleName = "torch",
                UsePythonIncluded = false,
                TestFilesPath = Path.Combine(test_dir, "Torch.UnitTest"),
                Usings =
                {
                    "using Numpy;",
                    "using Numpy.Models;"
                },
                ToCsharpConversions =
                {
                    "case \"Tensor\": return (T)(object)new Tensor(pyobj);",
                    "case \"Dtype\": return (T)(object)new Dtype(pyobj);",
                    "case \"Layout\": return (T)(object)new Layout(pyobj);",
                    "case \"Device\": return (T)(object)new Device(pyobj);",
                    "case \"NDarray\": return (T)(object)new NDarray(pyobj);",
                    "case \"Storage\": return (T)(object)new Storage(pyobj);",
                    "case \"Shape\": return (T)(object)new Shape(pyobj.As<int[]>());",
                },
                ToPythonConversions =
                {
                    "case Shape o: return ToTuple(o.Dimensions);",
                    "case Torch.PythonObject o: return o.PyObject;",
                    "case Numpy.PythonObject o: return o.PyObject;",
                },
                SpecialConversionGenerators =
                {
                    //SpecialGenerators.GenNDArrayToPython,
                }
            };
        }

        private HashSet<string> ManualOverride = new HashSet<string>() { "tensor" };

        public HtmlDocument LoadDoc(string url)
        {
            HtmlDocument doc;

            if (File.Exists(url))
            {
                doc = new HtmlDocument();
                doc.Load(url);
            }
            else
            {
                var web = new HtmlWeb();
                doc = web.Load(BaseUrl + url);
                File.WriteAllText(url, doc.Text);
            }
            return doc;
        }

        private string BaseUrl = "https://pytorch.org/docs/stable/";

        public string Generate()
        {
            ParseStaticApi("torch.html", stop_at: null);
            ParseDynamicApi("tensors.html", "Tensor", stop_at: null);
            ParseClasses("nn.html", subdir: "nn", stop_at: null);
            ParseStaticApi("nn.html", partial_name:"nn.util", start_at: "clip_grad_norm_", stop_at: "conv1d");
            ParseStaticApi("nn.html", partial_name: "nn.functional", start_at: "conv1d", stop_at: "calculate_gain");
            ParseStaticApi("nn.html", partial_name: "nn.init", start_at: "calculate_gain", stop_at: null);

            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            _generator.StaticApiFilesPath = Path.Combine(src_dir, "Torch");
            _generator.DynamicApiFilesPath = Path.Combine(src_dir, "Torch\\Models");
            _generator.ModelsPath = Path.Combine(src_dir, "Torch\\Models");
            //_generator.GenerateIntermediateJson();
            _generator.Generate();
            Thread.Sleep(2000);
            return "DONE";
        }

        private StaticApi torch_api = null;

        private void ParseStaticApi(string uri, string partial_name = null, string start_at = null, string stop_at = null)
        {
            Console.WriteLine("Parsing: " + uri);
            var doc = LoadDoc(uri);
            var api = new StaticApi()
            {
                StaticName = "torch", // name of the static API class
                ImplName = "PyTorch", // name of the singleton that implements the static API behind the scenes
                PythonModule = "torch", // name of the Python module that the static api wraps 
                PartialName = partial_name,
            };
            torch_api = api;
            _generator.StaticApis.Add(api);
            var testfile = new TestFile() { Name = $"{api.ImplName}_{api.PartialName}" };
            _generator.TestFiles.Add(testfile);

            var nodes = doc.DocumentNode.Descendants("dl")
                .Where(x => x.Attributes["class"]?.Value == "function")
                .ToList();
            var started = false;
            foreach (var node in nodes)
            {
                if (node.Descendants("dl").Any(x => x.Attributes["class"]?.Value == "function")) // skip over the overview funcs that group overloads
                    continue;
                var decl = new Function() { ClassName = api.StaticName };
                ParseFunctionName(decl, node);
                if (start_at != null && started == false)
                {
                    if (decl.Name == start_at)
                        started = true;
                    else 
                        continue;
                }
                if (stop_at != null && decl.Name == stop_at)
                    break;
                ParseDocString(decl, node);
                if (ManualOverride.Contains(decl.Name)) continue;
                //if (!InMigrationApiList(decl.Name)) continue;
                ParseReturnValue(decl, node);
                ParseArguments(decl, node);
                ParseDefaultValues(decl, node);

                foreach (var d in InferOverloads(decl))
                    api.Declarations.Add(d);

                PostProcess(decl);

                // see if there are any examples which we can convert to test cases
                var testcase = ParseTests(decl, node);
                if (testcase != null)
                    testfile.TestCases.Add(testcase);
            }
        }

        private void ParseDynamicApi(string uri, string classname, string partial_name = null, string stop_at = null)
        {
            Console.WriteLine("Parsing: " + uri);
            var doc = LoadDoc(uri);
            var api = new DynamicApi()
            {
                ClassName = classname, // name of the class to generate
                PartialName = partial_name,
            };
            _generator.DynamicApis.Add(api);
            var testfile = new TestFile() { Name = $"{api.ClassName}_{api.PartialName}" };
            _generator.TestFiles.Add(testfile);

            HtmlNode class_node = null;
            foreach (var node in doc.DocumentNode.Descendants("dl").Where(x => x.Attributes["class"]?.Value == "class"))
            {
                var code = node.Descendants("code").FirstOrDefault(y => y.Attributes["class"]?.Value == "descname");
                if (code.InnerText.Trim() == classname)
                {
                    class_node = node;
                    break;
                }
            }
            var nodes = class_node.Descendants("dl")
                .Where(x => x.Attributes["class"]?.Value == "method")
                .ToList();

            var stopped = false;
            foreach (var node in nodes)
            {
                var dd = node.Descendants("dd").FirstOrDefault();
                var decl = new Function() { ClassName = classname };
                ParseFunctionName(decl, node);
                if (dd.InnerText.Contains("See torch."))
                {
                    // todo: allow to search in all static apis, not only "torch."!
                    var static_version=torch_api.Declarations.FirstOrDefault(x => x.Name == decl.Name) as Function;
                    if (static_version==null)
                        continue;
                    //if (decl.Name=="abs")
                    //    Debugger.Break();
                    api.Declarations.Add(static_version.Clone(f =>
                    {
                        f.Arguments.RemoveAt(0);
                    }));
                    continue;
                };
                if (dd.InnerText.Contains("In-place version"))
                    continue;
                ParseDocString(decl, node);
                if (ManualOverride.Contains(decl.Name)) continue;
                //if (!InMigrationApiList(decl.Name)) continue;
                ParseReturnValue(decl, node);
                ParseArguments(decl, node);
                ParseDefaultValues(decl, node);
                PostProcess(decl);

                if (stop_at != null && decl.Name == stop_at)
                    stopped = true;
                if (stopped)
                    decl.Ignore = stopped;
                foreach (var d in InferOverloads(decl))
                    api.Declarations.Add(d);

                // see if there are any examples which we can convert to test cases
                var testcase = ParseTests(decl, node);
                if (testcase != null)
                    testfile.TestCases.Add(testcase);
            }
        }

        private void ParseClasses(string uri, string subdir, string stop_at = null)
        {
            Console.WriteLine("Parsing: " + uri);
            var doc = LoadDoc(uri);
            foreach (var classNode in doc.DocumentNode.Descendants("dl").Where(x => x.Attributes["class"]?.Value == "class"))
            {
                var constructor_parameters = classNode.Element("dt").InnerText;
                var fullname = classNode.Element("dt").Attributes["id"]?.Value;
                //if (fullname== "torch.nn.parallel.DistributedDataParallel")
                //    Debugger.Break();
                if (stop_at != null && fullname == stop_at)
                    return;
                //var classname = fullname.Split(".").Last();
                var api = new ApiClass()
                {
                    ClassName = fullname,
                    SubDir = subdir,
                };
                _generator.ApiClasses.Add(api);
                var testfile = new TestFile() { Name = $"{api.ClassName.Split(".").Last()}", SubDir = subdir };
                _generator.TestFiles.Add(testfile);
                var dd = classNode.Element("dd");
                api.DocString = string.Join("\r\n\r\n", dd.ChildNodes.TakeWhile(x => x.Name != "dl").Select(x => x.InnerText.Trim()).Where(x => !string.IsNullOrEmpty(x)));
                // Parse constructor
                var dl = dd.Element("dl");
                if (dl != null)
                {
                    var dt = dl.Element("dt");
                    if (dt != null && dt.InnerText == "Parameters")
                    {
                        var parameters_dd = dl.Element("dd");
                        var decl = new Function() { Name = fullname };
                        decl.Arguments = ParseArgumentsList(decl, parameters_dd);
                        ParseDefaultValuesFromText(decl as Function, constructor_parameters);
                        api.Constructors.Add(decl);
                    }
                }
                // parse functions if any
                var func_nodes = classNode.Descendants("dl")
                    .Where(x =>
                    {
                        var c = x.Attributes["class"]?.Value;
                        return c == "method" || c == "attribute";
                    }).ToList();
                foreach (var node in func_nodes)
                {
                    var c = node.Attributes["class"]?.Value;
                    Declaration decl = null;
                    if (c == "method")
                    {
                        decl = new Function() {ClassName = null};
                        ParseFunctionName(decl, node);
                        ParseDocString(decl, node);
                        if (ManualOverride.Contains(decl.Name)) continue;
                        //if (!InMigrationApiList(decl.Name)) continue;
                        ParseReturnValue(decl as Function, node);
                        ParseArguments(decl as Function, node);
                        ParseDefaultValues(decl as Function, node);
                        api.Declarations.Add(decl);
                    }
                    else if (c == "attribute")
                    {
                        var prop= new Property() { ClassName = null, HasSetter = true };
                        decl = prop;
                        var dt = node.Element("dt");
                        decl.Name=dt.Attributes["id"].Value.Split(".").Last();
                        if (dt.InnerText.Contains("="))
                            prop.DefaultValue = dt.InnerText.Split('=').Last().Trim(' ', '¶', '\r', '\n', '\t');
                        ParseDocString(decl, node);
                        api.Declarations.Add(decl);
                    }
                    // see if there are any examples which we can convert to test cases
                    var testcase = ParseTests(decl, node);
                    if (testcase != null)
                        testfile.TestCases.Add(testcase);
                }
                PostProcess(api);
            }
            Console.WriteLine($"\t... {_generator.ApiClasses.Count} classes");
        }

        private void PostProcess(ApiClass api)
        {
            if (api.ClassName.StartsWith("torch.nn."))
                PostProcessNN_Class(api);
        }

        private void ParseDefaultValues(Function decl, HtmlNode dl)
        {
            switch (decl.Name)
            {
                case "is_floating_point":
                case "bernoulli":
                    return;
            }
            var dt = dl.Descendants("dt").FirstOrDefault();
            if (dt == null)
                return;
            //if (decl.Name=="lu")
            //    Debugger.Break();
            var ems = dt.Descendants("em").ToArray();
            foreach (var em in ems)
            {
                Argument arg = null;
                var tokens = em.InnerText.Split("=");
                if (tokens.Length == 1)
                {
                    var attr_name = tokens[0].TrimStart('*', ' ');
                    if (attr_name.Contains(')'))
                        attr_name = attr_name.Split(')')[0];
                    arg = decl.Arguments.FirstOrDefault(x => x.Name == attr_name);
                    if (arg == null)
                        decl.Arguments.Add(arg = new Argument() { Name = attr_name });
                }
                else if (tokens.Length >= 2)
                {
                    var (attr_name, default_value) = (tokens[0].Trim('*', ' '), tokens[1].Trim());
                    arg = decl.Arguments.FirstOrDefault(x => x.Name == attr_name);
                    if (default_value.Contains(')'))
                        default_value = default_value.Split(')')[0].Trim();
                    if (arg == null)
                        decl.Arguments.Add(arg = new Argument() { Name = attr_name });
                    if (arg.DefaultValue == null)
                        arg.DefaultValue = InferDefaultValue(default_value, arg);
                }
                if (em.InnerText.Contains("-&gt;"))
                    break;
            }
        }

        private void ParseDefaultValuesFromText(Function f, string fullDeclaration)
        {
            var args= Regex.Match(fullDeclaration, @"\(.+?\)").Value?.Trim('(', ')', ' ');
            if (string.IsNullOrWhiteSpace(args))
                return;
            foreach (var token in Regex.Split(args, @",\s*"))
            {
                if (!token.Contains("="))
                    continue;
                var a = token.Split("=");
                var arg=f.Arguments.FirstOrDefault(x => x.Name == a[0]);
                if (arg==null)
                    continue;
                arg.DefaultValue = InferDefaultValue(a[1].Trim(), arg);
            }
        }

        private void ParseDocString(Declaration decl, HtmlNode node)
        {
            var dd = node.Descendants("dd").FirstOrDefault();
            if (dd == null)
                return;
            // function description
            decl.Description = string.Join("\n\n", dd.ChildNodes.TakeWhile(x => x.Name != "dl" && !x.InnerText.StartsWith("Example")).Select(x => x.InnerText.Trim()).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private TestCase ParseTests(Declaration decl, HtmlNode node)
        {
            var testcase = new TestCase() { Name = $"{decl.Name}Test" };
            foreach (var pre in node.Descendants("pre"))
            {
                var part = new ExampleCode() { Text = HtmlEntity.DeEntitize(pre.InnerText) };
                var lines = new Queue<string>(Regex.Split(part.Text.Trim(), @"\r?\n"));
                foreach (var line in lines)
                {
                    if (line.StartsWith(">>>"))
                    {
                        var cmd = line.Replace(">>>", "");
                        if (cmd.Contains("torch."))
                            cmd = cmd.Replace('[', '{').Replace(']', '}');
                        part.Lines.Add(new CodeLine() { Text = { cmd }, Type = "cmd" });
                        continue;
                    }
                    if (line.StartsWith("#"))
                    {
                        part.Lines.Add(new CodeLine() { Text = { line.Replace("#", "//") }, Type = "comment" });
                        continue;
                    }
                    if (part.Lines.Count == 0 || part.Lines.Last().Type != "output")
                        part.Lines.Add(new CodeLine() { Text = { line }, Type = "output" });
                    else
                        part.Lines.Last().Text.Add(line);
                }
                testcase.TestParts.Add(part);
            }
            if (testcase.TestParts.Count == 0)
                return null;
            return testcase;
        }

        private void ParseFunctionName(Declaration decl, HtmlNode node)
        {
            decl.Name = node.Element("dt").Descendants().First(x => x.Attributes["class"]?.Value == "descname").InnerText.Replace(".", "");
            var descclassname = node.Element("dt").Descendants()
                .FirstOrDefault(x => x.Attributes["class"]?.Value == "descclassname");
            if (descclassname!=null)
                decl.ClassName = descclassname.InnerText.TrimEnd('.');
        }

        private void ParseReturnValue(Function decl, HtmlNode node)
        {
            decl.Returns = new List<Argument>();
            //if (decl.Name == "lu")
            //    Debugger.Break();
            //var dts = node.Descendants("dt").ToArray();
            var yields = node.Descendants("dt").FirstOrDefault(x => x.InnerText == "Yields");
            if (yields != null)
            {
                var dd = yields.NextSibling.NextSibling;
                if (dd.InnerText.Contains('–'))
                {
                    var arg = new Argument() { IsReturnValue = true, };
                    var type = InferType(dd.InnerText.Split('–').First().Trim(), null, arg);
                    arg.Type = $"IEnumerable<{type}>";
                    decl.Returns.Add(arg);
                    return;
                }
                //else
                //{
                //    foreach (var token in dd.InnerText.Trim('(', ')', ' ').Split(","))
                //    {
                //        var arg = new Argument() {IsReturnValue = true,};
                //        var type = InferType(token.Replace("(optional)", "").Trim(' ', '\n', ')'), null, arg);
                //        arg.Type = $"IEnumerable<{type}>";
                //        decl.Returns.Add(arg);
                //    }
                //}
            }
            var returntype = node.Descendants("dt").FirstOrDefault(x => x.InnerText == "Return type");
            if (returntype != null)
            {
                var dd = returntype.NextSibling.NextSibling;
                foreach (var token in dd.InnerText.Trim('(', ')', ' ').Split(","))
                {
                    var arg = new Argument() { IsReturnValue = true, };
                    arg.Type = InferType(token.Replace("(optional)", "").Trim(' ', '\n', ')'), null, arg);
                    decl.Returns.Add(arg);
                }
            }
            var returns = node.Descendants("dt").FirstOrDefault(x => x.InnerText == "Returns");
            if (returns != null)
            {
                var dd = returns.NextSibling.NextSibling;
                if (decl.Returns.Count == 0 && dd.Descendants("ul").FirstOrDefault() != null)
                    decl.Returns = ParseArgumentsList(decl, dd);
            }
            if (decl.Returns.Count > 0)
                return;
            var dt = node.Element("dt");
            if (dt.InnerText.Contains("&#x2192;"))
            {
                var return_type = dt.InnerText.Trim().Split(' ').Last().Trim('¶', '(', ')', ' ');
                foreach (var token in return_type.Split(','))
                {
                    var arg = new Argument { Name = "retval", IsReturnValue = true, };
                    arg.Type = InferType(token.Trim(), null, arg);
                    decl.Returns.Add(arg);
                }
            }
            else if (dt.InnerText.Contains("-&gt;"))
            {
                var return_type = dt.InnerText.Trim().Split("-&gt;").Last().Trim('¶', '(', ')', ' ');
                foreach (var token in return_type.Split(','))
                {
                    var arg = new Argument { Name = "retval", IsReturnValue = true, };
                    arg.Type = InferType(token.Trim(), null, arg);
                    decl.Returns.Add(arg);
                }
            }
        }

        private void ParseArguments(Function decl, HtmlNode node)
        {
            //var p_nodes = node.Descendants("dd").First().Descendants("dl").FirstOrDefault();
            //if (p_nodes == null) return;

            //var p_node = p_nodes.Descendants("dd").FirstOrDefault();
            //if (p_node == null || p_node.InnerHtml == "")
            //    return;

            var dt = node.Descendants("dt").FirstOrDefault(x => x.InnerText == "Parameters");
            if (dt == null)
                return; // no params
            var dd = dt.NextSibling.NextSibling;


            //if (decl.Name == "mode")
            //    Debugger.Break();

            decl.Arguments = ParseArgumentsList(decl, dd);
        }

        private List<Argument> ParseArgumentsList(Function decl, HtmlNode dd)
        {
            //if (decl.Name=="torch.nn.Conv1d")
            //    Debugger.Break();
            var args = new List<Argument>();
            var ul = dd.Descendants("ul").FirstOrDefault();
            if (ul != null) // multiple parameters
            {
                foreach (var li in ul.Elements("li"))
                {
                    var arg = new Argument();

                    // precision – Number of digits of precision for floating point output(default = 4).
                    var p_desc = li.InnerText;
                    arg.Name = p_desc.Split(' ')[0].TrimStart('*', ' ');
                    arg.Description = string.Join(":", p_desc.Split('–', ':').Skip(1)).Trim();

                    var type_part = Regex.Match(p_desc, @"\((.+?)\)")?.Value; //(torch.dtype, optional)
                    if (!string.IsNullOrEmpty(type_part))
                    {
                        if (p_desc.Contains("optional"))
                        {
                            arg.IsNullable = true;
                            arg.IsNamedArg = true;
                        }
                        arg.Type = InferType(type_part.Split(',')[0].Trim(' ', '(', ')', '*'), arg.Description, arg);
                    }

                    //type_part = Regex.Match(p_desc, @"\(int...\)")?.Value; //(int...)
                    //if (!string.IsNullOrEmpty(type_part))
                    //    arg.Type = InferType("int...", null, arg);

                    var default_part = Regex.Match(p_desc, @"\(default\s*=\s*\S+\)")?.Value; //(default = 4)
                    if (!string.IsNullOrEmpty(default_part))
                    {
                        arg.DefaultValue = default_part.Split('=')[1].Trim(' ', '(', ')');
                        var hint = p_desc.Split('–')[1].Trim(' ', '(', ')');
                        // infer data type
                        if (string.IsNullOrEmpty(arg.Type))
                            arg.Type = InferType(arg.DefaultValue, hint, arg);
                        arg.IsNamedArg = true;
                    }

                    if (string.IsNullOrEmpty(arg.Type))
                    {
                        var hint = p_desc.Split('–', ':')[1];
                        arg.Type = InferType(Regex.Match(p_desc, @"\(\S+\)")?.Value, hint, arg);
                    }

                    args.Add(arg);
                }
            }
            else
            {
                var arg = new Argument();

                var p_desc = dd.InnerText; // obj (Object) – Object to test
                arg.Name = p_desc.Split(' ')[0].TrimStart('*', ' ');
                // may contain type desc
                var type_part = Regex.Match(p_desc.Split('–')[0], @"\([\S,\s]+\):")?.Value; // (list of Tensor):
                if (!string.IsNullOrEmpty(type_part))
                {
                    arg.Type = InferType(type_part.Replace(":", string.Empty), p_desc, arg);
                }
                if (string.IsNullOrEmpty(arg.Type))
                {
                    if (p_desc.Trim() == "self")
                        arg.Type = decl.ClassName;
                    else
                        arg.Type = InferType(p_desc.Split('–')[0].Split(' ')[1].Trim('(', ')', ',', ' '), p_desc, arg);
                }
                if (p_desc.Contains("optional"))
                {
                    arg.IsNullable = true;
                    arg.IsNamedArg = true;
                }
                args.Add(arg);
            }

            return args;
        }

        private void PostProcess(Argument arg)
        {
            switch (arg.Name)
            {
                case "pin_memory":
                    arg.PassOnlyIfNotNull = true;
                    break;
                case "gradient":
                    arg.Type = "Tensor";
                    break;
                case "generator":
                    arg.Type = "object";
                    break;
                case "tensor":
                case "input":
                case "other":
                case "tensor1":
                case "tensor2":
                    if (string.IsNullOrWhiteSpace(arg.Type))
                        arg.Type = "Tensor";
                    break;
                case "callable":
                    arg.Type = "Delegate";
                    break;
                case "shape":
                case "sizes":
                    arg.Type = "Shape";
                    break;
                case "mask":
                    arg.Type = "Tensor<byte>";
                    break;
                case "dim":
                case "dimension":
                    arg.Type = "int";
                    break;
                case "dtype":
                case "type":
                    arg.Type = "Dtype";
                    break;
                case "weight":
                    arg.Type = "double";
                    break;
                case "keepdim":
                    arg.Type = "bool";
                    break;
                case "dims":
                    arg.Type = "int[]";
                    break;
                case "out":
                    if (arg.Type == "tuple")
                        arg.Type = "Tensor[]";
                    break;
                case "modules":
                    arg.Type = "Module[]";
                    break;
            }
            switch (arg.Type)
            {
                case "Dtype":
                case "Device":
                case "Layout":
                    arg.IsValueType = false;
                    if (arg.DefaultValue != null)
                        arg.DefaultValue = "null";
                    break;
                case null:
                case "":
                    switch (arg.Name)
                    {
                        case "ndarray":
                            arg.Type = "NDarray";
                            break;
                        case "fill_value":
                            arg.Type = "object";
                            break;
                        case "out":
                            arg.Type = "Tensor";
                            break;
                        case "tensors":
                            arg.Type = "Tensor[]";
                            break;
                        case "shape":
                            arg.Type = "Shape";
                            break;

                    }
                    break;
            }

        }

        private void PostProcess(Function f)
        {
            foreach (var arg in f.Arguments.ToArray())
            {
                if (string.IsNullOrWhiteSpace(arg.Name))
                    f.Arguments.Remove(arg);
                PostProcess(arg);
            }
            switch (f.Name)
            {
                // ignore
                case "normal":
                case "add":
                case "apply_":
                    f.Ignore = true;
                    break;
                // ignore Tensor methods
                case "argsort":
                case "bernoulli_":
                case "flatten":
                case "item":
                case "requires_grad":
                    f.Ignore = (f.ClassName == "Tensor");
                    break;
                // ------------------
                case "empty":
                case "tensor":
                    f["requires_grad"].IsNullable = false;
                    f["pin_memory"].IsNullable = false;
                    break;
                case "is_tensor":
                case "is_storage":
                case "is_floating_point":
                    f.Returns.Add(new Argument() { Name = "retval", Type = "bool" });
                    break;
                case "set_printoptions":
                    f.ChangeArg("profile", Type: "string", DefaultValue: "\"default\"");
                    f.ChangeArg("sci_mode", IsNullable: true);
                    f.ChangeArg("precision", Type: "int", IsNullable: true);
                    f.ChangeArg("threshold", Type: "int", IsNullable: true);
                    f.ChangeArg("edgeitems", Type: "int", IsNullable: true);
                    f.ChangeArg("linewidth", Type: "int", IsNullable: true);
                    break;
                case "sparse_coo_tensor":
                    f["indices"].Type = "NDarray<int>";
                    f["values"].Type = "NDarray";
                    f.ChangeArg("size", Type: "int", IsNullable: true);
                    break;
                case "stack":
                    f["seq"].Type = "Tensor[]";
                    break;
                case "save":
                    f["obj"].Type = "PythonObject";
                    f["f"].Type = "string";
                    f["pickle_module"].Type = "PyObject";
                    f["pickle_module"].DefaultValue = "null";
                    f["pickle_protocol"].Type = "int";
                    break;
                case "load":
                    f["f"].Type = "string";
                    f["map_location"].Type = "PyObject";
                    f["map_location"].DefaultValue = "null";
                    f["pickle_module"].Type = "PyObject";
                    f["pickle_module"].DefaultValue = "null";
                    f["pickle_load_args"].Type = "params PyObject[]";
                    break;
                case "unbind":
                    f.Returns[0].Type = "Tensor[]";
                    break;
                case "set_num_threads":
                    f.Arguments[0].Name = "num";
                    f.Arguments[0].Type = "int";
                    break;
                case "new_full":
                    f.Arguments.RemoveAt(f.Arguments.Count - 1);
                    f.Arguments.Insert(0, new Argument() { Type = "Shape", Name = "size" });
                    f["fill_value"].Type = "T";
                    f.Generics = new[] { "T" };
                    break;
                case "new_empty":
                    f.Arguments.RemoveAt(f.Arguments.Count - 1);
                    f.Arguments.Insert(0, new Argument() { Type = "Shape", Name = "size" });
                    break;
                case "cauchy_":
                    f["median"].Type = "double";
                    f["sigma"].Type = "double";
                    f.Arguments.RemoveAt(2);
                    //func["generator"].Type = "object";
                    break;
                case "expand":
                    f.Arguments.Clear();
                    f.Arguments.Add(new Argument() { Name = "sizes", Type = "params int[]" });
                    break;
                case "exponential_":
                    f["lambd"].Type = "double";
                    f.Arguments.RemoveAt(1);
                    break;
                case "fill_":
                    f["value"].Type = "T";
                    f.Generics = new[] { "T" };
                    break;
                case "geometric_":
                    f["p"].Type = "double";
                    f.Arguments.RemoveAt(1);
                    break;
                case "get_device":
                    f.Name = "get_device_nr";
                    f.Returns[0].Type = "int";
                    f.Arguments.Clear();
                    break;
                case "index_add":
                case "index_copy":
                    f["dim"].Type = "int";
                    f["index"].Type = "Tensor<long>";
                    f["tensor"].Type = "Tensor";
                    break;
                case "index_fill":
                    f["dim"].Type = "int";
                    f["index"].Type = "Tensor<long>";
                    f["value"].Type = "float";
                    break;
                case "index_put_":
                case "index_put":
                    f["indices"].Type = "Tensor<long>[]";
                    f["value"].Type = "Tensor";
                    f["accumulate"].Type = "bool";
                    break;
                case "normal_":
                case "log_normal_":
                    if (f.ClassName == "torch" || f.ClassName == "Tensor")
                    {
                        f["mean"].Type = "double";
                        f["std"].Type = "double";
                        f.Arguments.RemoveAt(2);
                    }
                    break;
                case "random_":
                case "uniform_":
                    if (f.ClassName == "torch" || f.ClassName == "Tensor")
                    {
                        f["from"].Type = "T";
                        f["from"].DefaultValue = null;
                        f["to"].Type = "T";
                        f["to"].DefaultValue = null;
                        if (f.Arguments.Count > 2)
                            f.Arguments.RemoveAt(2);
                        f.Generics = new string[] {"T"};
                        f.Returns[0].Type = "Tensor<T>";
                    }
                    else if (f.ClassName == "torch.nn.init")
                    {
                        // todo:
                    }
                    else
                    {
                        Debugger.Break();
                    }
                    break;
                case "register_hook":
                    f["hook"].Type = "Func<Tensor, Tensor>";
                    break;
                case "narrow_copy":
                    f["start"].Type = "int";
                    f["length"].Type = "int";
                    break;
                case "masked_fill":
                case "masked_fill_":
                    f["value"].Type = "double";
                    break;
                case "quantize_linear":
                    f["scale"].Type = "double";
                    f["zero_point"].Type = "double";
                    break;
                case "scatter":
                case "scatter_add":
                    f["index"].Type = "Tensor<long>";
                    f["source"].Type = "Tensor";
                    break;
                case "set_":
                    f["source"].Type = "Tensor";
                    f["stride"].Type = "int[]";
                    break;
                case "sub":
                    f["value"].Type = "T";
                    f.Generics = new string[] { "T" };
                    f["other"].Type = "Tensor";
                    f["other"].DefaultValue = "null";
                    break;
                case "sum_to_size":
                    f["size"].Type = "Shape";
                    f.Arguments.Add(new Argument() { Name = "other", Type = "Tensor", DefaultValue = "null" });
                    break;
                case "type":
                    f.Arguments.Remove(f["kwargs"]);
                    break;
                case "clamp":
                    f["input"].Type = "Tensor";
                    if (f.Arguments.Count < 4)
                    {
                        f.Ignore = true;
                        break;
                    }
                    f["min"].Type = "double";
                    f["min"].IsNullable = true;
                    f["min"].DefaultValue = "null";
                    f["max"].Type = "double";
                    f["max"].IsNullable = true;
                    f["max"].DefaultValue = "null";
                    break;
                case "div":
                case "mul":
                    if (f.Arguments.Count == 0)
                    {
                        f.Ignore = true;
                        break;
                    }
                    if (f.Arguments.Any(x => x.Name == "value"))
                    {
                        f["input"].Type = "Tensor";
                        f["value"].Type = "T";
                        f.MakeGeneric("T");
                        break;
                    }
                    f["input"].Type = "Tensor";
                    f["other"].Type = "Tensor";
                    break;
                case "erfc":
                    f["input"].Ignore = true;
                    break;
                case "pow":
                    if (f.Arguments.Count == 0)
                    {
                        f.Ignore = true;
                        break;
                    }
                    if (f.Arguments.Any(x => x.Name == "exponent"))
                        f["exponent"].Type = "double";
                    if (f.Arguments.Any(x => x.Name == "base"))
                        f["base"].Type = "double";
                    break;
                case "mean":
                case "median":
                    if (f.Arguments.Count == 0)
                    {
                        f.Ignore = true;
                        break;
                    }
                    if (f.Arguments.Any(x => x.Name == "indices"))
                    {
                        f.Returns.Add(new Argument() { Type = "Tensor" });
                        f.Returns.Add(new Argument() { Type = "Tensor<long>" });
                        f["values"].Type = "Tensor";
                        f["indices"].Type = "Tensor";
                        f["indices"].DefaultValue = "null";
                    }
                    break;
                case "prod":
                    if (f.Arguments.Count == 0)
                        f.Ignore = true;
                    break;
                case "std":
                    if (f.Arguments.Count == 0)
                        f.Ignore = true;
                    break;
                case "norm":
                    f["p"].Type = "object";
                    f["p"].IsNullable = true;
                    f["p"].DefaultValue = "null";
                    f["dim"].Type = "int[]";
                    f["dim"].IsNullable = true;
                    break;
                case "unique":
                case "unique_consecutive":
                    f["dim"].IsNullable = true;
                    break;
                case "allclose":
                    f["equal_nan"].Type = "bool";
                    break;
                case "kthvalue":
                case "sort":
                case "topk":
                    f["out"].Type = "Tensor[]";
                    break;
                case "irfft":
                    f["signal_sizes"].Type = "Shape";
                    break;
                case "hamming_window":
                    f["alpha"].Type = "double";
                    f["beta"].Type = "double";
                    break;
                case "bincount":
                    f.Returns[0].Type = "Tensor";
                    f["self"].Ignore = true;
                    break;
                case "diagflat":
                    f["diagonal"].Ignore = true;
                    f["offset"].DefaultValue = "0";
                    break;
                case "einsum":
                    f["operands"].Type = "params Tensor[]";
                    break;
                case "meshgrid":
                    f["tensors"].Type = "params Tensor[]";
                    f["kwargs"].Ignore = true;
                    break;
                case "repeat_interleave":
                    if (f.Arguments.Count == 0)
                    {
                        f.Ignore = true;
                        break;
                    }
                    if (f.Arguments.Count == 3)
                    {
                        f["repeats"].Type = "int";
                        f["dim"].IsNullable = true;
                        break;
                    }
                    if (f.Arguments.Count == 1)
                    {
                        f.Arguments.Insert(0, new Argument() { Type = "Tensor", Name = "input" });
                        f.Arguments.Add(new Argument() { Type = "int", Name = "dim", IsNullable = true, IsNamedArg = true, DefaultValue = "null" });
                        f["repeats"].Type = "Tensor";
                    }
                    break;
                case "roll":
                    f["shifts"].Type = "int[]";
                    break;
                case "tensordot":
                    f["dims"].DefaultValue = null;
                    break;
                case "cholesky":
                    f["A"].Ignore = true;
                    break;
                case "eig":
                    f["out"].Type = "Tensor[]";
                    break;
                case "btrifact":
                case "btrisolve":
                case "btrifact_with_info":
                case "btriunpack":
                case "gesv":
                case "potrf":
                case "potri":
                case "potrs":
                case "pstrf":
                case "trtrs":
                    // deprecated!!
                    f.Ignore = true;
                    break;
                case "matrix_rank":
                    f["bool symmetric"].Ignore = true;
                    f["symmetric"].DefaultValue = "false";
                    break;
                case "compiled_with_cxx11_abi":
                    f.Returns.Add(new Argument() { Type = "bool" });
                    break;
                // from torch.nn.util
                case "clip_grad_norm_":
                case "clip_grad_value_":
                case "parameters_to_vector":
                case "vector_to_parameters":
                    f["parameters"].Type = "IEnumerable<Tensor>";
                    break;
                case "PackedSequence":
                    f.Ignore = true; // it is really a class
                    break;
                case "pack_padded_sequence":
                    f.ReturnType = "PackedSequence";
                    break;
                // from torch.nn.init
                case "calculate_gain":
                    f["nonlinearity"].Type = "string";
                    f["param"].SetNullableOptional( "double");
                    break;
                case "constant_":
                    f["val"].Type = "T";
                    f.MakeGeneric("T");
                    break;
                case "kaiming_uniform_":
                case "kaiming_normal_":
                    f["mode"].Type = "string";
                    f["nonlinearity"].SetType( "string", "\"leaky_relu\"");
                    break;
                case "orthogonal_":
                    f["tensor"].Type = "Tensor";
                    break;
                case "sparse_":
                    f["sparsity"].Type = "double";
                    break;
            }
        }

        protected string InferType(string value, string hint, Argument arg)
        {
            if (value.Contains("[source]"))
                value = value.Replace("[source]", "");
            value = value.Trim('(', ')').Replace("torch.", "");
            switch (value)
            {
                case "array_like":
                case "numpy.ndarray":
                    return "NDarray";
                case "int":
                    return "int";
                case "float":
                case "float or int":
                    return "float";
                case "list of Tensor":
                case "Tensors...":
                case "Tensors":
                case "sequence of Tensors":
                case "list[Tensor]":
                    return "Tensor[]";
                case "int or tuple":
                    return "int[]";
                case "IntArrayRef":
                    if (arg.Name == "size")
                        return "Shape"; // <-- int[] size usually means Shape of the tensor. 
                    return "int[]";
                case "Number":
                    return "double";
                case "boolean":
                case "bool":
                case "True":
                case "False":
                    return "bool";
                case "bool,optional":
                    arg.IsNullable = true;
                    return "bool";
                // torch types
                case "int...":
                case "Size":
                    return "Shape";
                case "Tensor":
                case "tensor":
                case "1-D":
                case "2-D":
                case "3-D":
                case "Tensor or float":
                    return "Tensor";
                case "LongTensor": return "Tensor<long>";
                case "IntTensor": return "Tensor<int>";
                case "FloatTensor": return "Tensor<float>";
                case "DoubleTensor": return "Tensor<double>";
                case "ByteTensor": return "Tensor<byte>";
                case "dtype":
                case "type":
                    return "Dtype";
                case "layout":
                    return "Layout";
                case "device":
                    return "Device";
                case "dict":
                    return "Hashtable";
                case "str":
                    return "string";
            }
            //if (arg.Name=="track_running_stats")
            //    Debugger.Break();
            if (!string.IsNullOrWhiteSpace(arg.Description))
                hint = arg.Description;
            if (hint != null)
            {
                if (Regex.IsMatch(hint, @"(Number|Amount) of", RegexOptions.IgnoreCase))
                {
                    arg.DefaultValue = InferDefaultValue(Regex.Match(hint, @"Default: ([+-]?\d+)", RegexOptions.IgnoreCase).FirstGroupOrNull(), arg);
                    return "int";
                }
                if (Regex.IsMatch(hint, @"If (True|False)", RegexOptions.IgnoreCase))
                {
                    arg.DefaultValue = InferDefaultValue(Regex.Match(hint, @"Default: (True|False)", RegexOptions.IgnoreCase).FirstGroupOrNull(), arg);
                    return "bool";
                }
                var match = Regex.Match(hint, @"Default: ([+-]?(\d+.\d+|\d+e[+-]\d+))", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    arg.DefaultValue = InferDefaultValue(match.FirstGroupOrNull(), arg);
                    return "double";
                }
                match = Regex.Match(hint, @"Default: ([+-]?\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    arg.DefaultValue = InferDefaultValue(match.FirstGroupOrNull(), arg);
                    return "int";
                }
                match = Regex.Match(hint, @"Default: (True|False)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    arg.DefaultValue = InferDefaultValue(match.FirstGroupOrNull(), arg);
                    return "bool";
                }
                match = Regex.Match(hint, @"Default: '(.+?)'", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    arg.DefaultValue = $"\"{match.FirstGroupOrNull()}\"";
                    return "string";
                }
            }
            return value;
        }

        private string InferDefaultValue(string defaultValue, Argument arg)
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;
            switch (defaultValue)
            {
                case "torch.strided":
                case "None":
                    return "null";
                case "True":
                    if (string.IsNullOrWhiteSpace(arg.Type))
                        arg.Type = "bool";
                    return "true";
                case "False":
                    if (string.IsNullOrWhiteSpace(arg.Type))
                        arg.Type = "bool";
                    return "false";
            }
            if (arg.Type == "float" && defaultValue != "null")
                return defaultValue + "f";
            if (defaultValue != null && defaultValue.StartsWith('\''))
            {
                if (string.IsNullOrWhiteSpace(arg.Type))
                    arg.Type = "string";
                return "\"" + defaultValue.Trim('\'') + "\"";
            }
            if (string.IsNullOrWhiteSpace(arg.Type))
            {
                if (Regex.IsMatch(defaultValue, @"([+-]?(\d+.\d+|\d+e[+-]\d+))", RegexOptions.IgnoreCase))
                    arg.Type = "double";
                else if (Regex.IsMatch(defaultValue, @"^([+-]?(\d+))$"))
                    arg.Type = "int";
            }
            return defaultValue;
        }

        private IEnumerable<Function> InferOverloads(Function func)
        {
            // without args we don't need to consider possible overloads
            if (func.Arguments.Count == 0)
            {
                yield return func;
                yield break;
            }
            switch (func.Name)
            {
                case "arange":
                case "range":
                    func["start"].DefaultValue = null;
                    yield return func.Clone(clone =>
                    {
                        clone.Arguments.RemoveAt(0);
                        clone.Arguments.RemoveAt(1);
                    });
                    break;
                case "randint":
                    func["size"].Type = "Shape";
                    func["low"].DefaultValue = null;
                    func["low"].IsNullable = false;
                    yield return func.Clone(clone => { clone.Arguments.RemoveAt(0); });
                    break;
                case "randint_like":
                    func["low"].DefaultValue = null;
                    func["low"].IsNullable = false;
                    yield return func.Clone(clone => { clone.Arguments.RemoveAt(1); });
                    break;
                case "stride":
                    func["dim"].IsNullable = false;
                    func["dim"].DefaultValue = null;
                    yield return func.Clone(clone =>
                    {
                        clone.Arguments.RemoveAt(0);
                        clone.Returns[0].Type = "int[]";
                    });
                    break;
                case "to":
                    func.Arguments.Clear();
                    func.Arguments.Add(new Argument() { Name = "dtype", Type = "Dtype", });
                    func.Arguments.Add(new Argument() { Name = "non_blocking", Type = "bool", DefaultValue = "false" });
                    func.Arguments.Add(new Argument() { Name = "copy", Type = "bool", DefaultValue = "false" });
                    yield return func.Clone(clone =>
                    {
                        clone.Arguments.Insert(0, new Argument() { Name = "device", Type = "Device" });
                        clone["dtype"].DefaultValue = "null";
                    });
                    yield return func.Clone(clone =>
                    {
                        clone.Arguments.RemoveAt(0);
                        clone.Arguments.Insert(0, new Argument() { Name = "other", Type = "Tensor" });
                    });
                    break;
                case "addcdiv":
                case "addcmul":
                    func["value"].DefaultValue = null;
                    func["value"].IsNullable = false;
                    yield return func.Clone(clone =>
                    {
                        clone.Arguments.RemoveAt(1);
                    });
                    break;
                case "fmod":
                case "remainder":
                    func["divisor"].Type = "double";
                    yield return func.Clone(clone => { clone["divisor"].Type = "Tensor"; });
                    break;
                case "pow":
                    if (func.Arguments.Any(x => x.Name == "exponent"))
                        yield return func.Clone(clone =>
                        {
                            clone["input"].Type = "Tensor";
                            clone["exponent"].Type = "Tensor";
                            clone["out"].Type = "Tensor";
                        });
                    break;
                case "addbmm":
                case "addmm":
                case "addmv":
                case "addr":
                case "baddbmm":
                    func["beta"].IsNullable = false;
                    func["beta"].DefaultValue = null;
                    func["alpha"].IsNullable = false;
                    func["alpha"].DefaultValue = null;
                    yield return func.Clone(clone =>
                    {
                        clone["beta"].Ignore = true;
                        clone["alpha"].Ignore = true;
                    });
                    break;
            }
            yield return func;
        }


    }
}
