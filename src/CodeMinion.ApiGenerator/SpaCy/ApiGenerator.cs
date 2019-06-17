using CodeMinion.Core;
using CodeMinion.Core.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator.SpaCy
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
                PrintModelJson = true,  // <-- if enabled prints the declaration model as JSON for debugging reasons
                NameSpace = "spacy",
                UsePythonIncluded = false,
                TestFilesPath = Path.Combine(test_dir, "SpaCy.UnitTest"),
                Usings =
                {
                    "using spacy;",
                },
                ToCsharpConversions =
                {
                },
                ToPythonConversions =
                {
                },
                SpecialConversionGenerators =
                {
                }
            };
        }

        private string BaseUrl = "https://spacy.io/api/";

        public string Generate()
        {
            // ParseStaticApi("token", stop_at: null);
            ParseDynamicApi("token", "Token");

            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            _generator.StaticApiFilesPath = Path.Combine(src_dir, "SpaCy");
            _generator.DynamicApiFilesPath = Path.Combine(src_dir, "SpaCy\\Models");
            //_generator.GenerateIntermediateJson();
            _generator.Generate();
            return "DONE";
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
                //ParseFunctionName(decl, node);
                //ParseDocString(decl, node);
                //if (ManualOverride.Contains(decl.Name)) continue;
                //if (!InMigrationApiList(decl.Name)) continue;
                //ParseReturnValue(decl, node);
                //ParseArguments(decl, node);
                //ParseDefaultValues(decl, node);
                //PostProcess(decl);

                if (stop_at != null && decl.Name == stop_at)
                    stopped = true;
                if (stopped)
                    decl.Ignore = stopped;
                //foreach (var d in InferOverloads(decl))
                    //api.Declarations.Add(d);

                // see if there are any examples which we can convert to test cases
                //var testcase = ParseTests(decl, node);
                //if (testcase != null)
                    //testfile.TestCases.Add(testcase);
            }
        }

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
    }
}
