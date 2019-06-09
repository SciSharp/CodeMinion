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

namespace Torch.ApiGenerator
{
    public class TorchApiGenerator
    {
        private CodeGenerator _generator;
        public TorchApiGenerator()
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

        public string Generate()
        {
            var docs = LoadDocs();
            var api = new StaticApi()
            {
                StaticName = "torch", // name of the static API class
                ImplName = "PyTorch", // name of the singleton that implements the static API behind the scenes
                PythonModule = "torch" // name of the Python module that the static api wraps 
            };
            _generator.StaticApis.Add(api);
            foreach (var html in docs)
            {
                var testfile = new TestFile() { Name = $"{api.ImplName}_{api.PartialName}" };
                _generator.TestFiles.Add(testfile);

                var doc = new HtmlDocument();
                doc.LoadHtml(html.Value);

                var nodes = doc.DocumentNode.Descendants("dl")
                     .Where(x => x.Attributes["class"]?.Value == "function")
                     .ToList();

                foreach (var node in nodes)
                {
                    var decl = new Function();
                    ParseFunctionName(decl, node);
                    if (decl.Name == "addcdiv")
                        break;
                    ParseDocString(decl, node);
                    if (ManualOverride.Contains(decl.Name)) continue;
                    //if (!InMigrationApiList(decl.Name)) continue;
                    SetReturnType(decl, node);
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

            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            _generator.StaticApiFilesPath = Path.Combine(src_dir, "Torch");
            _generator.DynamicApiFilesPath = Path.Combine(src_dir, "Torch\\Models");
            //_generator.GenerateIntermediateJson();
            _generator.Generate();
            return "DONE";
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
            foreach (var em in dt.Descendants("em"))
            {
                Argument arg = null;
                var tokens = em.InnerText.Split("=");
                if (tokens.Length == 1)
                {
                    var attr_name = tokens[0].TrimStart('*');
                    arg = decl.Arguments.FirstOrDefault(x => x.Name == attr_name);
                    if (arg == null)
                        decl.Arguments.Add(arg = new Argument() { Name = attr_name });
                }
                else if (tokens.Length >= 2)
                {
                    var (attr_name, default_value) = (tokens[0].TrimStart('*'), tokens[1]);
                    arg = decl.Arguments.FirstOrDefault(x => x.Name == attr_name);
                    if (arg == null)
                        decl.Arguments.Add(arg = new Argument() { Name = attr_name });
                    if (arg.DefaultValue == null)
                        arg.DefaultValue = InferDefaultValue(default_value, arg);
                }
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

        private void SetReturnType(Declaration decl, HtmlNode node)
        {
            decl.Returns = new List<Argument>();
            if (decl.Name.StartsWith("is_"))
            {
                decl.Returns.Add(new Argument { Type = "bool" });
            }

            var dt = node.Element("dt");
            if (dt.InnerText.Contains("&#x2192;"))
            {
                var return_type = dt.InnerText.Trim().Split(' ').Last().Trim('¶');
                var arg = new Argument { Name = "retval" };
                arg.Type = InferType(return_type, null, arg);
                decl.Returns.Add(arg);
            }
        }

        private void ParseArguments(Function decl, HtmlNode node)
        {
            decl.Arguments = new List<Argument>();
            var p_nodes = node.Descendants("dd").First().Descendants("dl").FirstOrDefault();
            if (p_nodes == null) return;

            var p_node = p_nodes.Descendants("dd").First();
            if (p_node.InnerHtml == "")
                return;

            if (p_node.Element("ul") != null) // multiple parameters
            {
                foreach (var li in p_node.Element("ul").Elements("li"))
                {
                    var arg = new Argument();

                    // precision – Number of digits of precision for floating point output(default = 4).
                    var p_desc = li.InnerText;
                    arg.Name = p_desc.Split(' ')[0].TrimStart('*');
                    arg.Description = p_desc.Split('–')[1].Trim();

                    var type_part = Regex.Match(p_desc, @"\(\S+, optional\)")?.Value; //(torch.dtype, optional)
                    if (!string.IsNullOrEmpty(type_part))
                    {
                        arg.Type = InferType(type_part.Split(',')[0].Substring(1).Trim(), null, arg);
                        arg.IsNullable = true;
                        arg.IsNamedArg = true;
                    }

                    type_part = Regex.Match(p_desc, @"\(int...\)")?.Value; //(int...)
                    if (!string.IsNullOrEmpty(type_part))
                        arg.Type = InferType("int...", null, arg);

                    var default_part = Regex.Match(p_desc, @"\(default = \d+\)")?.Value; //(default = 4)
                    if (!string.IsNullOrEmpty(default_part))
                    {
                        arg.DefaultValue = default_part.Split('=')[1].Replace(")", string.Empty);
                        var hint = p_desc.Split('–')[1];
                        // infer data type
                        if (string.IsNullOrEmpty(arg.Type))
                            arg.Type = InferType(arg.DefaultValue, hint, arg);
                        arg.IsNamedArg = true;
                    }

                    if (string.IsNullOrEmpty(arg.Type))
                    {
                        var hint = p_desc.Split('–')[1];
                        arg.Type = InferType(Regex.Match(p_desc, @"\(\S+\)")?.Value, hint, arg);
                    }
                    decl.Arguments.Add(arg);
                }
            }
            else
            {
                var arg = new Argument();

                var p_desc = p_node.InnerText; // obj (Object) – Object to test
                arg.Name = p_desc.Split(' ')[0];
                // may contain type desc
                var type_part = Regex.Match(p_desc.Split('–')[0], @"\([\S,\s]+\):")?.Value; // (list of Tensor):
                if (!string.IsNullOrEmpty(type_part))
                    arg.Type = InferType(type_part.Replace(":", string.Empty), p_desc, arg);
                if (string.IsNullOrEmpty(arg.Type))
                    arg.Type = InferType(p_desc.Split('–')[0].Split(' ')[1].Replace("(", string.Empty).Replace(")", string.Empty), p_desc, arg);
                //var desc = p_desc.Split('–')[1].Trim();

                decl.Arguments.Add(arg);
            }
        }

        private void PostProcess(Argument arg)
        {
            switch (arg.Name)
            {
                case "pin_memory":
                    arg.PassOnlyIfNotNull = true;
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
            foreach (var arg in func.Arguments)
                PostProcess(arg);
            switch (func.Name)
            {
                case "set_printoptions":
                    func.ChangeArg("profile", Type: "string", DefaultValue: "\"default\"");
                    func.ChangeArg("sci_mode", IsNullable: true);
                    break;
                case "sparse_coo_tensor":
                    func["indices"].Type = "NDarray<int>";
                    func["values"].Type = "NDarray";
                    func.ChangeArg("size", Type: "int", IsNullable: true);
                    break;
                case "stack":
                    func["seq"].Type = "Tensor[]";
                    break;
                case "normal":
                case "add":
                    func.CommentOut = true;
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
            }
        }

        protected string InferType(string value, string hint, Argument arg)
        {
            if (hint != null && hint.ToLower().Contains("number of "))
                return "int";
            value = value.Trim('(', ')').Replace("torch.", "");
            switch (value)
            {
                case "array_like":
                    return "NDarray";
                case "int":
                    return "int";
                case "float":
                    return "float";
                case "list of Tensor":
                case "Tensors...":
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
                    return "Shape";
                case "Tensor":
                    return "Tensor";
                case "LongTensor": return "Tensor<long>";
                case "IntTensor": return "Tensor<int>";
                case "FloatTensor": return "Tensor<float>";
                case "DoubleTensor": return "Tensor<double>";
                case "ByteTensor": return "Tensor<byte>";
                case "Tensors":
                    return "Tensor[]";
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
                    return "true";
                case "False":
                    return "false";
            }
            if (arg.Type == "float" && defaultValue != "null")
                return defaultValue + "f";
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
                    //yield return func.Clone(clone =>
                    //{
                    //    clone["start"].Type="int";
                    //    clone["end"].Type = "int";
                    //    clone["step"].Type = "int";
                    //});
                    //yield return func.Clone(clone =>
                    //{
                    //    clone["end"].Type = "int";
                    //    clone.Arguments.RemoveAt(0);
                    //    clone.Arguments.RemoveAt(1);
                    //});
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
            }
            yield return func;
        }


        public Dictionary<string, string> LoadDocs()
        {
            var docs = new Dictionary<string, string>();

            // torch.html
            string url = "https://pytorch.org/docs/stable/torch.html";

            HtmlDocument doc;

            if (File.Exists("torch.html"))
            {
                doc = new HtmlDocument();
                doc.Load("torch.html");
            }
            else
            {
                var web = new HtmlWeb();
                doc = web.Load(url);
                File.WriteAllText("torch.html", doc.Text);
            }

            docs["torch"] = doc.Text;

            return docs;
        }
    }
}
