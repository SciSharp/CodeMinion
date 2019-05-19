﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeMinion.Core;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;

namespace Torch.ApiGenerator
{
    public class TorchApiGenerator
    {
        private CodeGenerator _generator;
        public TorchApiGenerator()
        {
            _generator = new CodeGenerator
            {
                //PrintModelJson=true,  // <-- if enabled prints the declaration model as JSON for debugging reasons
                NameSpace = "Torch",
                Usings = {"using NumSharp;"},
                ToCsharpConversions =
                {
                    "case \"Tensor\": return (T)(object)new Tensor(pyobj);",
                },
                ToPythonConversions =
                {
                    "case NumSharp.Shape o: return ToTuple(o.Dimensions);",
                    "case Tensor o: return o.PyObject;",
                    "case NumSharp.NDArray o: return NDArrayToPython(o);",
                },
                SpecialConversionGenerators =
                {
                    SpecialGenerators.GenNDArrayToPython,
                }
            };
        }

        // generate these API calls
        protected bool InMigrationApiList(string apiName)
        {
            var apis = new string[] { "empty", "tensor" };
            return apis.Contains(apiName);
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
                var doc = new HtmlDocument();
                doc.LoadHtml(html.Value);

                /*<dt id="torch.is_tensor">
                    <code class="descclassname">torch.</code><code class="descname">is_tensor</code><span class="sig-paren">(</span><em>obj</em><span class="sig-paren">)</span><a class="reference internal" href="_modules/torch.html#is_tensor"><span class="viewcode-link">[source]</span></a><a class="headerlink" href="#torch.is_tensor" title="Permalink to this definition">¶</a></dt>
                        <dd><p>Returns True if <cite>obj</cite> is a PyTorch tensor.</p>
                        <dl class="field-list simple">
                        <dt class="field-odd">Parameters</dt>
                        <dd class="field-odd"><p><strong>obj</strong> (<em>Object</em>) – Object to test</p>
                        </dd>
                    </dl>
                </dd>*/
                var nodes = doc.DocumentNode.Descendants("dl")
                     .Where(x => x.Attributes["class"]?.Value == "function")
                     .ToList();

                foreach (var node in nodes)
                {
                    var decl = new Declaration();
                    ParseFunctionName(decl, node);
                    if (ManualOverride.Contains(decl.Name)) continue;
                    if (!InMigrationApiList(decl.Name)) continue;
                    SetReturnType(decl, node);
                    ParseArguments(decl, node);

                    foreach(var d in InferOverloads(decl))
                        api.Declarations.Add(d);
                }
            }

            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            api.OutputPath = Path.Combine(src_dir, "Torch");

            _generator.Generate();
            return "DONE";
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

            if(node.Element("dt").InnerText.Contains("&#x2192; Tensor"))
            {
                decl.Returns.Add(new Argument { Type = "Tensor" });
            }
        }

        private void ParseArguments(Declaration decl, HtmlNode node)
        {
            decl.Arguments = new List<Argument>();
            var p_nodes = node.Descendants("dd").First().Descendants("dl").FirstOrDefault();
            if (p_nodes == null) return;

            var p_node = p_nodes.Descendants("dd").First();
            if (p_node.InnerHtml == "")
            {
                Console.WriteLine($"Skipped {decl.Name}");
                return;
            }

            if (p_node.Element("ul") != null) // multiple parameters
            {
                foreach(var li in p_node.Element("ul").Elements("li"))
                {
                    var arg = new Argument();

                    // precision – Number of digits of precision for floating point output(default = 4).
                    var p_desc = li.InnerText;
                    arg.Name = p_desc.Split(' ')[0];

                    var type_part = Regex.Match(p_desc, @"\(\S+, optional\)")?.Value; //(torch.dtype, optional)
                    if (!string.IsNullOrEmpty(type_part))
                    {
                        arg.Type = InferDataType(type_part.Split(',')[0].Substring(1).Trim(), null, arg);
                        arg.IsNullable = true;
                        arg.IsNamedArg = true;
                    }

                    type_part = Regex.Match(p_desc, @"\(int...\)")?.Value; //(int...)
                    if (!string.IsNullOrEmpty(type_part))
                        arg.Type = InferDataType("int...", null, arg);

                    var default_part = Regex.Match(p_desc, @"\(default = \d+\)")?.Value; //(default = 4)
                    if (!string.IsNullOrEmpty(default_part))
                    {
                        arg.DefaultValue = default_part.Split('=')[1].Replace(")", string.Empty);
                        var hint = p_desc.Split('–')[1];
                        // infer data type
                        if (string.IsNullOrEmpty(arg.Type))
                            arg.Type = InferDataType(arg.DefaultValue, hint, arg);
                        arg.IsNamedArg = true;
                    }

                    if (string.IsNullOrEmpty(arg.Type)) {
                        var hint = p_desc.Split('–')[1];
                        arg.Type = InferDataType(Regex.Match(p_desc, @"\(\S+\)")?.Value, hint, arg);
                    }

                    PostProcess(arg);
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
                    arg.Type = InferDataType(type_part.Replace(":", string.Empty), p_desc, arg);
                if (string.IsNullOrEmpty(arg.Type))
                    arg.Type = p_desc.Split('–')[0].Split(' ')[1].Replace("(", string.Empty).Replace(")", string.Empty);
                //var desc = p_desc.Split('–')[1].Trim();

                decl.Arguments.Add(arg);
            }
        }

        private void PostProcess(Argument arg)
        {
            switch (arg.Type) {
                case "dtype":
                case "device":
                case "layout":
                    arg.IsValueType = true;
                    break;
            }
        }

        private IEnumerable<Declaration> InferOverloads(Declaration decl)
        {
            // without args we don't need to consider possible overloads
            if (decl.Arguments.Count == 0)
            {
                yield return decl;
                yield break;
            }
            // array_like
            if (decl.Arguments.Any(a => a.Type == "(array_like)"))
            {
                foreach (var type in "NDarray T[]".Split())
                {
                    var clone_decl = decl.Clone();
                    clone_decl.Arguments.ForEach(a =>
                    {
                        if (a.Type == "array_like")
                            a.Type = type;
                    });
                    if (type == "T[]")
                        clone_decl.Generics = new string[] { "T" };
                    yield return clone_decl;
                }
                yield break;
            }
            yield return decl;
        }

        protected string InferDataType(string value, string hint, Argument arg)
        {
            if (hint!=null&& hint.ToLower().Contains("number of "))
                return "int";
            switch (value)
            {
                case "(array_like)":
                    return "(array_like)"; // keep it like that so we can generate overloads
                case "(int)":
                    return "int";
                case "(list of Tensor)":
                    return "Tensor[]";
                case "IntArrayRef":
                    if (arg.Name == "size")
                        return "NumSharp.Shape"; // <-- int[] size usually means Shape of the tensor. 
                    return "int[]";
                // torch types
                case "int...":
                    return "NumSharp.Shape";
                case "Tensor":
                    return "Tensor";
                default:
                    // Console.WriteLine("MapType doesn't handle type: " + arg.type);
                    if(value != null && value.StartsWith("torch."))
                        return value.Replace("torch.", string.Empty);
                    return value;
            }
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
