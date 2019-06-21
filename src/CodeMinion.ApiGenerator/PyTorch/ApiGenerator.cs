using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeMinion.Core;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;
using CodeMinion.Parser;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator.PyTorch
{
    public class ApiGenerator : ICodeGenerator
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
            ParseClasses("nn.html", subdir:"nn");

            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            _generator.StaticApiFilesPath = Path.Combine(src_dir, "Torch");
            _generator.DynamicApiFilesPath = Path.Combine(src_dir, "Torch\\Models");
            //_generator.GenerateIntermediateJson();
            _generator.Generate();
            return "DONE";
        }

        private void ParseStaticApi(string uri, string partial_name = null, string stop_at = null)
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
            _generator.StaticApis.Add(api);
            var testfile = new TestFile() { Name = $"{api.ImplName}_{api.PartialName}" };
            _generator.TestFiles.Add(testfile);

            var nodes = doc.DocumentNode.Descendants("dl")
                .Where(x => x.Attributes["class"]?.Value == "function")
                .ToList();

            foreach (var node in nodes)
            {
                var decl = new Function() { ClassName = api.StaticName };
                ParseFunctionName(decl, node);
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
                if (dd.InnerText.Contains("See torch.") || dd.InnerText.Contains("In-place version"))
                    continue;
                var decl = new Function() { ClassName = classname };
                ParseFunctionName(decl, node);
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

        private void ParseClasses(string uri, string subdir)
        {
            Console.WriteLine("Parsing: " + uri);
            var doc = LoadDoc(uri);
            foreach (var classNode in doc.DocumentNode.Descendants("dl").Where(x => x.Attributes["class"]?.Value == "class"))
            {
                var fullname = classNode.Element("dt").Attributes["id"]?.Value;
                //var classname = fullname.Split(".").Last();
                var api = new ApiClass()
                {
                    ClassName = fullname, 
                    SubDir = subdir,
                };
                _generator.ApiClasses.Add(api);
                var testfile = new TestFile() { Name = $"{api.ClassName}" };
                _generator.TestFiles.Add(testfile);
                var dd = classNode.Element("dd");
                api.DocString=dd.Element("p").InnerText;
                var dl=dd.Element("dl");
                if (dl != null)
                {
                    var dt = dl.Element("dt");
                    if (dt != null && dt.InnerText == "Parameters")
                    {
                        var parameters_dd = dl.Element("dd");
                        var decl=new Function() { Name="Constructor" };
                        ParseArgumentsList(decl, parameters_dd);
                        api.Constructors.Add(decl);
                    }
                }
            }
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

        private void ParseDocString(Function decl, HtmlNode node)
        {
            var dd = node.Descendants("dd").FirstOrDefault();
            if (dd == null)
                return;
            // function description
            decl.Description = string.Join("\n\n", dd.ChildNodes.TakeWhile(x => x.Name != "dl" && !x.InnerText.StartsWith("Example")).Select(x => x.InnerText.Trim()).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private TestCase ParseTests(Function decl, HtmlNode node)
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
            decl.Name = node.Element("dt").Descendants().First(x => x.Attributes["class"]?.Value == "descname").InnerText.Replace(".", string.Empty);
        }

        private void ParseReturnValue(Function decl, HtmlNode node)
        {
            decl.Returns = new List<Argument>();
            //if (decl.Name == "lu")
            //    Debugger.Break();
            //var dts = node.Descendants("dt").ToArray();
            var returntype = node.Descendants("dt").FirstOrDefault(x => x.InnerText == "Return type");
            if (returntype != null)
            {
                var dd = returntype.NextSibling.NextSibling;
                foreach (var token in dd.InnerText.Trim('(', ')', ' ').Split(","))
                {
                    var arg = new Argument() {IsReturnValue = true,};
                    arg.Type = InferType(token.Replace("(optional)", "").Trim(' ', '\n', ')'), null, arg);
                    decl.Returns.Add(arg);
                }
            }
            var returns = node.Descendants("dt").FirstOrDefault(x => x.InnerText=="Returns");
            if (returns != null)
            {
                var dd = returns.NextSibling.NextSibling;
                if (decl.Returns.Count==0 && dd.Descendants("ul").FirstOrDefault()!=null)
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
            var p_nodes = node.Descendants("dd").First().Descendants("dl").FirstOrDefault();
            if (p_nodes == null) return;

            var p_node = p_nodes.Descendants("dd").FirstOrDefault();
            if (p_node == null || p_node.InnerHtml == "")
                return;

            //if (decl.Name == "mode")
            //    Debugger.Break();

            decl.Arguments= ParseArgumentsList(decl, p_node);
        }

        private List<Argument> ParseArgumentsList(Function decl, HtmlNode dd)
        {
            //if (decl.Name=="to_sparse")
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
                    arg.Description = string.Join(":", p_desc.Split('–', ':' ).Skip(1)).Trim();

                    var type_part = Regex.Match(p_desc, @"\((\S+(, optional)?)\)")?.Value; //(torch.dtype, optional)
                    if (!string.IsNullOrEmpty(type_part))
                    {
                        if (p_desc.Contains("optional"))
                        {
                            arg.IsNullable = true;
                            arg.IsNamedArg = true;
                        }
                        arg.Type = InferType(type_part.Split(',')[0].Trim(' ', '(', ')', '*'), null, arg);
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
                    arg.Type = InferType(type_part.Replace(":", string.Empty), p_desc, arg);
                if (string.IsNullOrEmpty(arg.Type))
                    arg.Type = InferType(
                        p_desc.Split('–')[0].Split(' ')[1].Trim('(', ')', ',', ' '), p_desc, arg);
                //var desc = p_desc.Split('–')[1].Trim();

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

        private void PostProcess(Function func)
        {
            foreach (var arg in func.Arguments.ToArray())
            {
                if (string.IsNullOrWhiteSpace(arg.Name))
                    func.Arguments.Remove(arg);
                PostProcess(arg);
            }
            switch (func.Name)
            {
                // ignore
                case "normal":
                case "add":
                case "apply_":
                    func.Ignore = true;
                    break;
                // ignore Tensor methods
                case "argsort":
                case "bernoulli_":
                case "flatten":
                case "item":
                case "requires_grad":
                    func.Ignore = (func.ClassName == "Tensor");
                    break;
                // ------------------
                case "is_tensor":
                case "is_storage":
                case "is_floating_point":
                    func.Returns.Add(new Argument() { Name = "retval", Type = "bool" });
                    break;
                case "set_printoptions":
                    func.ChangeArg("profile", Type: "string", DefaultValue: "\"default\"");
                    func.ChangeArg("sci_mode", IsNullable: true);
                    func.ChangeArg("precision", Type: "int", IsNullable: true);
                    func.ChangeArg("threshold", Type: "int", IsNullable: true);
                    func.ChangeArg("edgeitems", Type: "int", IsNullable: true);
                    func.ChangeArg("linewidth", Type: "int", IsNullable: true);
                    break;
                case "sparse_coo_tensor":
                    func["indices"].Type = "NDarray<int>";
                    func["values"].Type = "NDarray";
                    func.ChangeArg("size", Type: "int", IsNullable: true);
                    break;
                case "stack":
                    func["seq"].Type = "Tensor[]";
                    break;
                case "save":
                    func["obj"].Type = "PythonObject";
                    func["f"].Type = "string";
                    func["pickle_module"].Type = "PyObject";
                    func["pickle_module"].DefaultValue = "null";
                    func["pickle_protocol"].Type = "int";
                    break;
                case "load":
                    func["f"].Type = "string";
                    func["map_location"].Type = "PyObject";
                    func["map_location"].DefaultValue = "null";
                    func["pickle_module"].Type = "PyObject";
                    func["pickle_module"].DefaultValue = "null";
                    func["pickle_load_args"].Type = "params PyObject[]";
                    break;
                case "unbind":
                    func.Returns[0].Type = "Tensor[]";
                    break;
                case "set_num_threads":
                    func.Arguments[0].Name = "num";
                    func.Arguments[0].Type = "int";
                    break;
                case "new_full":
                    func.Arguments.RemoveAt(func.Arguments.Count - 1);
                    func.Arguments.Insert(0, new Argument() { Type = "Shape", Name = "size" });
                    func["fill_value"].Type = "T";
                    func.Generics = new[] { "T" };
                    break;
                case "new_empty":
                    func.Arguments.RemoveAt(func.Arguments.Count - 1);
                    func.Arguments.Insert(0, new Argument() { Type = "Shape", Name = "size" });
                    break;
                case "cauchy_":
                    func["median"].Type = "double";
                    func["sigma"].Type = "double";
                    func.Arguments.RemoveAt(2);
                    //func["generator"].Type = "object";
                    break;
                case "expand":
                    func.Arguments.Clear();
                    func.Arguments.Add(new Argument() { Name = "sizes", Type = "params int[]" });
                    break;
                case "exponential_":
                    func["lambd"].Type = "double";
                    func.Arguments.RemoveAt(1);
                    break;
                case "fill_":
                    func["value"].Type = "T";
                    func.Generics = new[] { "T" };
                    break;
                case "geometric_":
                    func["p"].Type = "double";
                    func.Arguments.RemoveAt(1);
                    break;
                case "get_device":
                    func.Name = "get_device_nr";
                    func.Returns[0].Type = "int";
                    func.Arguments.Clear();
                    break;
                case "index_add":
                case "index_copy":
                    func["dim"].Type = "int";
                    func["index"].Type = "Tensor<long>";
                    func["tensor"].Type = "Tensor";
                    break;
                case "index_fill":
                    func["dim"].Type = "int";
                    func["index"].Type = "Tensor<long>";
                    func["value"].Type = "float";
                    break;
                case "index_put_":
                case "index_put":
                    func["indices"].Type = "Tensor<long>[]";
                    func["value"].Type = "Tensor";
                    func["accumulate"].Type = "bool";
                    break;
                case "normal_":
                case "log_normal_":
                    func["mean"].Type = "double";
                    func["std"].Type = "double";
                    func.Arguments.RemoveAt(2);
                    break;
                case "random_":
                case "uniform_":
                    func["from"].Type = "T";
                    func["from"].DefaultValue = null;
                    func["to"].Type = "T";
                    func["to"].DefaultValue = null;
                    if (func.Arguments.Count > 2)
                        func.Arguments.RemoveAt(2);
                    func.Generics = new string[] { "T" };
                    func.Returns[0].Type = "Tensor<T>";
                    break;
                case "register_hook":
                    func["hook"].Type = "Func<Tensor, Tensor>";
                    break;
                case "narrow_copy":
                    func["start"].Type = "int";
                    func["length"].Type = "int";
                    break;
                case "masked_fill":
                case "masked_fill_":
                    func["value"].Type = "double";
                    break;
                case "quantize_linear":
                    func["scale"].Type = "double";
                    func["zero_point"].Type = "double";
                    break;
                case "scatter":
                case "scatter_add":
                    func["index"].Type = "Tensor<long>";
                    func["source"].Type = "Tensor";
                    break;
                case "set_":
                    func["source"].Type = "Tensor";
                    func["stride"].Type = "int[]";
                    break;
                case "sub":
                    func["value"].Type = "T";
                    func.Generics = new string[] { "T" };
                    func["other"].Type = "Tensor";
                    func["other"].DefaultValue = "null";
                    break;
                case "sum_to_size":
                    func["size"].Type = "Shape";
                    func.Arguments.Add(new Argument() { Name = "other", Type = "Tensor", DefaultValue = "null" });
                    break;
                case "type":
                    func.Arguments.Remove(func["kwargs"]);
                    break;
                case "clamp":
                    func["input"].Type = "Tensor";
                    if (func.Arguments.Count < 4)
                    {
                        func.Ignore = true;
                        break;
                    }
                    func["min"].Type = "double";
                    func["min"].IsNullable = true;
                    func["min"].DefaultValue = "null";
                    func["max"].Type = "double";
                    func["max"].IsNullable = true;
                    func["max"].DefaultValue = "null";
                    break;
                case "div":
                case "mul":
                    if (func.Arguments.Count == 0)
                    {
                        func.Ignore = true;
                        break;
                    }
                    if (func.Arguments.Any(x => x.Name == "value"))
                    {
                        func["input"].Type = "Tensor";
                        func["value"].Type = "T";
                        func.MakeGeneric("T");
                        break;
                    }
                    func["input"].Type = "Tensor";
                    func["other"].Type = "Tensor";
                    break;
                case "erfc":
                    func["input"].Ignore = true;
                    break;
                case "pow":
                    if (func.Arguments.Count == 0)
                    {
                        func.Ignore = true;
                        break;
                    }
                    if (func.Arguments.Any(x => x.Name == "exponent"))
                        func["exponent"].Type = "double";
                    if (func.Arguments.Any(x => x.Name == "base"))
                        func["base"].Type = "double";
                    break;
                case "mean":
                case "median":
                    if (func.Arguments.Count == 0)
                    {
                        func.Ignore = true;
                        break;
                    }
                    if (func.Arguments.Any(x => x.Name == "indices"))
                    {
                        func.Returns.Add(new Argument() { Type = "Tensor" });
                        func.Returns.Add(new Argument() { Type = "Tensor<long>" });
                        func["values"].Type = "Tensor";
                        func["indices"].Type = "Tensor";
                        func["indices"].DefaultValue = "null";
                    }
                    break;
                case "prod":
                    if (func.Arguments.Count == 0)
                        func.Ignore = true;
                    break;
                case "std":
                    if (func.Arguments.Count == 0)
                        func.Ignore = true;
                    break;
                case "norm":
                    func["p"].Type = "object";
                    func["p"].IsNullable = true;
                    func["p"].DefaultValue = "null";
                    func["dim"].Type = "int[]";
                    func["dim"].IsNullable = true;
                    break;
                case "unique":
                case "unique_consecutive":
                    func["dim"].IsNullable = true;
                    break;
                case "allclose":
                    func["equal_nan"].Type = "bool";
                    break;
                case "kthvalue":
                case "sort":
                case "topk":
                    func["out"].Type = "Tensor[]";
                    break;
                case "irfft":
                    func["signal_sizes"].Type = "Shape";
                    break;
                case "hamming_window":
                    func["alpha"].Type = "double";
                    func["beta"].Type = "double";
                    break;
                case "bincount":
                    func.Returns[0].Type = "Tensor";
                    func["self"].Ignore = true;
                    break;
                case "diagflat":
                    func["diagonal"].Ignore = true;
                    func["offset"].DefaultValue = "0";
                    break;
                case "einsum":
                    func["operands"].Type = "params Tensor[]";
                    break;
                case "meshgrid":
                    func["tensors"].Type = "params Tensor[]";
                    func["kwargs"].Ignore = true;
                    break;
                case "repeat_interleave":
                    if (func.Arguments.Count == 0)
                    {
                        func.Ignore = true;
                        break;
                    }
                    if (func.Arguments.Count == 3)
                    {
                        func["repeats"].Type = "int";
                        func["dim"].IsNullable = true;
                        break;
                    }
                    if (func.Arguments.Count==1)
                    {
                        func.Arguments.Insert(0, new Argument() { Type = "Tensor", Name="input" });
                        func.Arguments.Add(new Argument() { Type = "int", Name = "dim", IsNullable = true, IsNamedArg = true, DefaultValue = "null"});
                        func["repeats"].Type = "Tensor";
                    }
                    break;
                case "roll":
                    func["shifts"].Type = "int[]";
                    break;
                case "tensordot":
                    func["dims"].DefaultValue = null;
                    break;
                case "cholesky":
                    func["A"].Ignore = true;
                    break;
                case "eig":
                    func["out"].Type = "Tensor[]";
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
                    func.Ignore = true;
                    break;
                case "matrix_rank":
                    func["bool symmetric"].Ignore=true;
                    func["symmetric"].DefaultValue = "false";
                    break;
                case "compiled_with_cxx11_abi":
                    func.Returns.Add(new Argument(){ Type="bool"});
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
                    return "float";
                case "list of Tensor":
                case "Tensors...":
                case "Tensors":
                    return "Tensor[]";
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
                default:
                    return value;
            }
        }

        private string InferDefaultValue(string defaultValue, Argument arg)
        {
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
                return "\"" + defaultValue.Trim('\'') + "\"";
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
                        clone["beta"].Ignore=true;
                        clone["alpha"].Ignore = true;
                    });
                    break;
            }
            yield return func;
        }


    }
}
