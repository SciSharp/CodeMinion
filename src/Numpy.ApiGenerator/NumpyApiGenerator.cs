using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMinion.Core;
using CodeMinion.Core.Models;
using CodeMinion.Parser;
using HtmlAgilityPack;

namespace Numpy.ApiGenerator
{
    public class NumpyApiGenerator
    {
        private CodeGenerator _generator;
        public NumpyApiGenerator()
        {
            _generator = new CodeGenerator
            {
                //PrintModelJson=true,  // <-- if enabled prints the declaration model as JSON for debugging reasons
                NameSpace = "Numpy",
                Usings = { "using NumSharp;" },
                ToPythonConversions = {
                    "case NumSharp.Shape o: return ToTuple(o.Dimensions);",
                    "case PythonObject o: return o.PyObject;",
                },
                ToCsharpConversions =
                {
                    "case \"Dtype\": return (T)(object)new Dtype(pyobj);",
                    "case \"NDarray\": return (T)(object)new NDarray(pyobj);",
                },
            };
        }

        public void Generate()
        {
            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            // ----------------------------------------------------
            // array creation
            // ----------------------------------------------------
            var array_creation_api = new StaticApi()
            {
                PartialName = "array_creation", // name-part of the partial class file
                StaticName = "np", // name of the static API class
                ImplName = "NumPy", // name of the singleton that implements the static API behind the scenes
                PythonModule = "numpy", // name of the Python module that the static api wraps 
                OutputPath = Path.Combine(src_dir, "Numpy"),
                InitializationGenerators = { SpecialGenerators.InitNumpyGenerator },
            };
            _generator.StaticApis.Add(array_creation_api);
            ParseArrayCreationApi(array_creation_api);
            // ----------------------------------------------------
            // dtype
            // ----------------------------------------------------
            var dtype_api = new StaticApi()
            {
                PartialName = "dtype",
                StaticName = "np", // name of the static API class
                ImplName = "NumPy", // name of the singleton that implements the static API behind the scenes
                PythonModule = "numpy", // name of the Python module that the static api wraps 
                OutputPath = Path.Combine(src_dir, "Numpy"),
            };
            _generator.StaticApis.Add(dtype_api);
            ParseDtypeApi(dtype_api);
            // ----------------------------------------------------
            // ndarray
            // ----------------------------------------------------
            var ndarray_api = new DynamicApi()
            {
                ClassName = "NDarray",
                OutputPath = Path.Combine(src_dir, "Numpy/Models"),
            };
            _generator.DynamicApis.Add(ndarray_api);
            ParseNdarrayApi(ndarray_api);
            // ----------------------------------------------------
            // generate all
            // ----------------------------------------------------
            _generator.Generate();
        }

        private void ParseNdarrayApi(DynamicApi api)
        {
            var docs = LoadDocs("arrays.ndarray.html");
            foreach (var html_doc in docs)
            {
                var doc = html_doc.Doc;
                // declaration
                var h1 = doc.DocumentNode.Descendants("h1").FirstOrDefault();
                if (h1 == null)
                    continue;
                var dl = doc.DocumentNode.Descendants("dl").FirstOrDefault();
                if (dl == null || dl.Attributes["class"]?.Value != "method") continue;
                var class_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descclassname").InnerText;
                var func_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descname").InnerText;
                var decl = new Function() { Name = func_name, ClassName = class_name.TrimEnd('.') };
                // function description
                var dd = dl.Descendants("dd").FirstOrDefault();
                decl.Description = ParseDescription(dd);
                var table = doc.DocumentNode.Descendants("table")
                    .FirstOrDefault(x => x.Attributes["class"]?.Value == "docutils field-list");
                if (table == null)
                    continue;
                // arguments
                ParseArguments(html_doc, table, decl);

                // return type(s)
                ParseReturnTypes(html_doc, table, decl);

                PostProcess(decl);
                // if necessary create overloads
                foreach (var d in InferOverloads(decl))
                    api.Declarations.Add(d);
            }
        }

        private void ParseDtypeApi(StaticApi api)
        {
            var doc = GetHtml("arrays.scalars.html");
            foreach (var tr in doc.Doc.DocumentNode.Descendants("tr"))
            {
                if (tr.Descendants("td").Count()!=3)
                    continue;
                var span = tr.Descendants("span").FirstOrDefault();
                if (span==null)
                    continue;
                var td = tr.Descendants("td").Skip(1).FirstOrDefault();
                api.Declarations.Add(new Property() { Name = span.InnerText, Description = td?.InnerText, Returns = { new Argument(){ Type = "Dtype" }}});
            }
        }

        private void ParseArrayCreationApi(StaticApi api)
        {
            var docs = LoadDocs("routines.array-creation.html");
            foreach (var html_doc in docs)
            {
                var doc = html_doc.Doc;
                // declaration
                var h1 = doc.DocumentNode.Descendants("h1").FirstOrDefault();
                if (h1 == null)
                    continue;
                var dl = doc.DocumentNode.Descendants("dl").FirstOrDefault();
                if (dl == null || dl.Attributes["class"]?.Value != "function") continue;
                var class_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descclassname").InnerText;
                var func_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descname").InnerText;
                var decl = new Function() { Name = func_name, ClassName = class_name.TrimEnd('.') };
                // function description
                var dd = dl.Descendants("dd").FirstOrDefault();
                decl.Description = ParseDescription(dd);
                var table = doc.DocumentNode.Descendants("table")
                    .FirstOrDefault(x => x.Attributes["class"]?.Value == "docutils field-list");
                if (table == null)
                    continue;
                // arguments
                ParseArguments(html_doc, table, decl);

                // return type(s)
                ParseReturnTypes(html_doc, table, decl);

                PostProcess(decl);
                // if necessary create overloads
                foreach (var d in InferOverloads(decl))
                    api.Declarations.Add(d);
            }
        }

        private string ParseDescription(HtmlNode dd)
        {
            if (dd == null)
                return null;
            var desc = string.Join("\r\n\r\n", dd.ChildNodes.Where(n => n.Name == "p").Select(p => p.InnerText).TakeWhile(s => !s.StartsWith("Examples")));
            return desc;
        }

        private void ParseArguments(HtmlDoc html_doc, HtmlNode table, Function decl)
        {
            var tr = table.Descendants("tr").FirstOrDefault();
            if (tr == null)
                return;
            foreach (var dt in tr.Descendants("dt"))
            {
                var arg = new Argument();
                var strong = dt.Descendants("strong").FirstOrDefault();
                if (strong == null)
                    continue;
                arg.Name = strong.InnerText;
                if (arg.Name.ToLower() == "none")
                    continue; // there are no arguments fro this method
                var type_description = dt.Descendants("span")
                    .First(span => span.Attributes["class"]?.Value == "classifier").InnerText;
                var type = type_description.Split(",").FirstOrDefault();
                arg.Type = InferType(type, arg);
                if (type_description.Contains("optional"))
                {
                    arg.IsNamedArg = true;
                    arg.IsNullable = true;
                }
                if (type_description.Contains("default:"))
                    arg.DefaultValue = InferDefaultValue(type_description.Split(",")
                        .First(x => x.Contains("default: ")).Replace("default: ", ""));
                var dd = dt.NextSibling?.NextSibling;
                arg.Description = ParseDescription(dd);
                PostProcess(arg);
                decl.Arguments.Add(arg);
            }
        }

        private void PostProcess(Argument arg)
        {
            if (arg.Name == "order")
                arg.DefaultValue = null;
            if (arg.Name == "axes")
            {
                arg.Type = "int[]";
                arg.DefaultValue = "null";
                return;
            }
            if (arg.Name.StartsWith("*"))
                arg.Name = arg.Name.TrimStart('*');
            switch (arg.Type)
            {
                //case "int[]":
                //case "Hashtable":
                //    arg.IsValueType = false;
                //    break;
                case "Dtype":
                    arg.IsValueType = false;
                    break;
            }
        }

        private void PostProcess(Function decl)
        {
            if (decl.Arguments.Any(a => a.Type == "buffer_like"))
                decl.CommentOut = true;
            // iterable object            
            if (decl.Arguments.Any(a => a.Type.Contains("<T>")))
            {
                decl.Generics = new string[] { "T" };
                if (decl.Returns[0].Type == "NDarray") // TODO: this feels like a hack. make it more robust if necessary
                    decl.Returns[0].Type = "NDarray<T>";
            }
            if (decl.Returns.Any(a => a.Type == "T" || a.Type.Contains("<T>")))
            {
                decl.Generics = new string[] { "T" };
            }
            switch (decl.Name)
            {
                case "array":
                case "itemset":
                case "tostring":
                case "tobytes":
                case "view":
                case "resize":
                    decl.ManualOverride = true; // do not generate an implementation
                    break;
                case "arange":
                    decl.Arguments[0].IsNullable = false;
                    decl.Arguments[0].DefaultValue = "0";
                    decl.Arguments[2].DefaultValue = "1";
                    decl.Arguments[2].IsNullable = false;
                    decl.Arguments[3].IsNullable = false;
                    decl.Arguments[3].IsNamedArg = true;
                    break;
                case "logspace":
                case "geomspace":
                    decl.Arguments.First(a => a.Type == "Dtype").IsNullable = false;
                    decl.Arguments.First(a => a.Type == "Dtype").DefaultValue = "null";
                    decl.Arguments.First(a => a.Type == "Dtype").IsNamedArg = true;
                    break;
                case "meshgrid":
                case "mat":
                case "bmat":
                    decl.CommentOut = true;
                    break;
            }
        }

        private void ParseReturnTypes(HtmlDoc html_doc, HtmlNode table, Declaration decl)
        {
            var tr = table.Descendants("tr").FirstOrDefault(x => x.InnerText.StartsWith("Returns:"));
            if (tr == null)
                return;
            foreach (var dt in tr.Descendants("dt"))
            {
                var arg = new Argument();
                var strong = dt.Descendants("strong").FirstOrDefault();
                if (strong != null)
                    arg.Name = strong.InnerText;
                if (arg.Name.ToLower()=="none")
                    continue;
                var type_description = dt.Descendants("span")
                    .First(span => span.Attributes["class"]?.Value == "classifier").InnerText;
                var type = type_description.Split(",").FirstOrDefault();
                arg.Type = InferType(type, arg);
                var dd = dt.NextSibling?.NextSibling;
                arg.Description = ParseDescription(dd);
                decl.Returns.Add(arg);
            }
        }

        private IEnumerable<Function> InferOverloads(Function decl)
        {
            // without args we don't need to consider possible overloads
            if (decl.Arguments.Count == 0)
            {
                yield return decl;
                yield break;
            }
            if (decl.Name == "arange")
            {
                foreach (var d in ExpandArange(decl))
                    yield return d;
                yield break;
            }
            // array_like
            if (decl.Arguments.Any(a => a.Type == "array_like"))
            {
                foreach (var type in "NDarray T[] T[,]".Split())
                {
                    var clone_decl = decl.Clone<Function>();
                    clone_decl.Arguments.ForEach(a =>
                    {
                        if (a.Type == "array_like")
                            a.Type = type;
                    });
                    if (type.StartsWith("T["))
                    {
                        clone_decl.Generics = new string[] { "T" };
                        if (clone_decl.Returns[0].Type == "NDarray") // TODO: this feels like a hack. make it more robust if necessary
                            clone_decl.Returns[0].Type = "NDarray<T>";
                    }
                    yield return clone_decl;
                }
                yield break;
            }
            // number
            if (decl.Arguments.Any(a => a.Type == "number"))
            {
                foreach (var type in "byte short int long float double".Split())
                {
                    var clone_decl = decl.Clone<Function>();
                    clone_decl.Arguments.ForEach(a =>
                    {
                        if (a.Type == "number")
                            a.Type = type;
                    });
                    yield return clone_decl;
                }
                yield break;
            }
            if (decl.Name == "bmat")
            {
                decl.Arguments[0].Type = "string";
                yield return decl;
                var clone_decl = decl.Clone<Function>();
                clone_decl.Arguments[0].Type = "T[]";
                clone_decl.Generics = new[] { "T" };
                clone_decl.Returns[0].Type = "matrix<T>";
                yield return clone_decl;
                yield break;

            }
            yield return decl;
        }

        // special treatment for np.arange which is a "monster"
        private IEnumerable<Function> ExpandArange(Function decl)
        {
            // numpy.arange([start, ]stop, [step, ]dtype=None)
            var dtype = decl.Arguments.Last();
            dtype.IsNullable = true;
            dtype.IsNamedArg = true;
            if (decl.Arguments.Any(a => a.Type == "number"))
            {
                foreach (var type in "byte short int long float double".Split())
                {
                    // start, stop
                    var clone_decl = decl.Clone<Function>();
                    clone_decl.Arguments.ForEach(a =>
                    {
                        if (a.Type == "number")
                            a.Type = type;
                    });
                    clone_decl.Arguments[0].IsNamedArg = false;
                    clone_decl.Arguments[0].IsNullable = false;
                    clone_decl.Arguments[0].DefaultValue = null;
                    yield return clone_decl;
                    // [start=0] <-- remove start from arg list
                    clone_decl = clone_decl.Clone<Function>(); // <---- clone from the clone, as it has the correct type
                    clone_decl.Arguments.RemoveAt(0);
                    yield return clone_decl;
                }
                yield break;
            }
        }

        private string InferDefaultValue(string default_value)
        {
            // string
            //if (Regex.IsMatch(default_value, @"$‘(.+)’"))
            //    return $"\"{ default_value.Trim('\'', '‘', '’') }\""; //   ‘C’ => "C"
            return default_value;
        }

        private string InferType(string type, Argument arg)
        {
            switch (arg.Name)
            {
                case "shape": return "NumSharp.Shape";
                case "dtype": return "Dtype";
                case "order": return "string";
            }
            switch (type)
            {
                case "data-type": return "Dtype";
                case "ndarray": return "NDarray";
                case "scalar": return "ValueType";
                case "file": return "string";
                case "str": return "string";
                case "string or list": return "string";
                case "file or str": return "string";
                case "str or sequence of str": return "string[]";
                case "array of str or unicode-like": return "string[]";
                case "callable": return "Delegate";
                case "any": return "object";
                case "iterable object": return "IEnumerable<T>";
                case "dict": return "Hashtable";
                case "int or sequence": return "int[]";
                case "int or sequence of ints": return "int[]";
                case "boolean": return "bool";
                case "integer": return "int";
                case "Standard Python scalar object": return "T";
                case "Arguments (variable number and type)": return "params int[]";
                case "list": return "List<T>";
            }
            if (type.StartsWith("ndarray"))
                return "NDarray";
            if (type.StartsWith("{‘"))
                return "string";
            return type;
        }

        string BaseUrl = "https://docs.scipy.org/doc/numpy-1.16.1/reference/";

        public List<HtmlDoc> LoadDocs(string overview_url)
        {
            var docs = new List<HtmlDoc>();
            var doc = GetHtml(overview_url);
            LoadDocsFromOverviewPage(doc.Doc, docs);
            return docs;
        }

        private void LoadDocsFromOverviewPage(HtmlDocument doc, List<HtmlDoc> docs)
        {
            var nodes = doc.DocumentNode.Descendants("a")
                .Where(x => x.Attributes["class"]?.Value == "reference internal")
                .ToList();
            foreach (var node in nodes)
            {
                var relative_link = node.Attributes["href"].Value;
                if (!relative_link.StartsWith("generated"))
                    continue;
                var uri = relative_link.Split("#").First();
                docs.Add(GetHtml(uri));
            }
        }

        HtmlDoc GetHtml(string relative_url)
        {
            Console.WriteLine("Loading: " + relative_url);
            var doc = new HtmlDoc();
            doc.Filename = relative_url.Replace("/", "_");
            if (File.Exists(doc.Filename))
            {
                doc.Doc = new HtmlDocument();
                doc.Doc.Load(doc.Filename);
                doc.Text = doc.Doc.Text;
                return doc;
            }
            var web = new HtmlWeb();
            doc.Doc = web.Load(BaseUrl + relative_url);
            doc.Text = doc.Doc.Text;
            File.WriteAllText(doc.Filename, doc.Text);
            return doc;
        }
    }
}

