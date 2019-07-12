using CodeMinion.Core;
using CodeMinion.Core.Models;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Torch.ApiGenerator;
using System.Linq;

namespace CodeMinion.ApiGenerator.MxNet
{
    class ApiGenerator : ICodeGenerator
    {
        private CodeGenerator _generator;

        private string codeDir = AppDomain.CurrentDomain.BaseDirectory + "../../../../MxNet/";
        private string testDir = Directory.GetCurrentDirectory() + "../MxNet/UnitTest/";
        private string templateClass = File.ReadAllText("./mxnet/Tmpl/ClassTmpl.txt");
        private string templateMethod = File.ReadAllText("./mxnet/Tmpl/MethodTmpl.txt");
        private string templateModule = File.ReadAllText("./mxnet/Tmpl/ModuleTmpl.txt");

        public string Generate()
        {
            string result = "";
            string json = "";
            string moduleName = "image";
            using (var gil = Py.GIL())
            {
                dynamic exporter = PythonEngine.ModuleFromString("exporter", File.ReadAllText("./mxnet/ExportSignatureToJson.py").Replace("[MODULE]", "mxnet." + moduleName));
                json = exporter.generate().ToString();
            }

            var library = PyLibrary.LoadJson(json);

            foreach (var item in library.Modules)
            {
                if (item.Name.Contains("._") || item.Name.Contains(".gen_"))
                    continue;

                GenerateCode(item, "/image/", "image");
            }

            return result;
        }

        private void GenerateCode(PyModule module, string path, string ns)
        {
            string srcFolder = codeDir + path;
            ModuleExt.InferArg(module);
            if(!Directory.Exists(srcFolder))
            {
                Directory.CreateDirectory(srcFolder);
            }

            string result = templateModule;
            result = result.Replace("[MODULE]", module.Name.Split(".").Last());
            result = result.Replace("[NAMESPACE]", ns);

            foreach (var item in module.Classes)
            {
                if (item.Name.StartsWith("_"))
                    continue;

                string[] splitStr = item.Name.Split(".");
                string clsName = splitStr.Last();

                string classString = BuildClassFile(item, module.Name, ns);

                StringBuilder classFunctions = new StringBuilder();
                foreach (var func in item.Functions)
                {
                    string funcStr = BuildFunction(func);
                    if (funcStr != "")
                        classFunctions.AppendLine(funcStr);
                }

                classString = classString.Replace("[METHODS]", classFunctions.ToString());

                File.WriteAllText(srcFolder + item.Name.Replace("mxnet.", "").Split(".").Last() + ".cs", classString);
            }

            StringBuilder functions = new StringBuilder();
            bool haveFuns = false;
            foreach (var item in module.Functions)
            {
                string func = BuildFunction(item);
                if (func != "")
                {
                    functions.AppendLine(func);
                    haveFuns = true;
                }
            }

            if (haveFuns)
            {
                result = result.Replace("[METHODS]", functions.ToString());
                File.WriteAllText(srcFolder + module.Name.Replace("mxnet.", "").Split(".").Last() + ".cs", result);
            }
        }

        private string BuildClassFile(PyClass cls, string moduleName, string ns)
        {
            if (cls.Name.StartsWith("_"))
                return "";

            string result = templateClass;
            result = result.Replace("[MODULE]", string.Join(".", moduleName.Replace("mxnet.", "").Split(".").SkipLast(1).ToArray()));
            result = result.Replace("[CLSNAME]", cls.Name);
            result = result.Replace("[NAMESPACE]", ns);

            StringBuilder args = new StringBuilder();
            StringBuilder parameters = new StringBuilder();
            foreach (var item in cls.Parameters)
            {
                string type = item.DataType != null ? item.DataType : "Unknown";
                string defaulValue = item.HaveDefault ? " = " + item.DefaultValue : "";
                args.AppendFormat("{0} {1}{2},", type, item.Name, defaulValue);

                parameters.AppendLine(string.Format("\t\tParameters[\"{0}\"] = {0};", item.Name));
            }

            if(args.ToString().LastIndexOf(',') > 0)
            {
                result = result.Replace("[ARGS]", args.ToString().Remove(args.ToString().LastIndexOf(',')));
            }
            else
            {
                result = result.Replace("[ARGS]", "");
            }

            result = result.Replace("[PARAMETERS]", parameters.ToString());
            //result = result.Replace("[COMMENTS]", cls.DocStr);

            return result;
        }

        private string BuildFunction(PyFunction func)
        {
            if (func.Name.StartsWith("_"))
                return "";

            string result = templateMethod;
            func.Parameters.Reverse();
            result = result.Replace("[METHODNAME]", func.Name);

            StringBuilder args = new StringBuilder();
            StringBuilder parameters = new StringBuilder();
            foreach (var item in func.Parameters)
            {
                string type = item.DataType != null ? item.DataType : "object";
                string defaulValue = item.HaveDefault ? " = " + item.DefaultValue : "";
                args.AppendFormat("{0} {1}{2},", type, item.Name, defaulValue);

                parameters.AppendLine(string.Format("parameters[\"{0}\"] = {0};", item.Name));
            }

            if (args.ToString().LastIndexOf(',') > 0)
            {
                result = result.Replace("[ARGS]", args.ToString().Remove(args.ToString().LastIndexOf(',')));
            }
            else
            {
                result = result.Replace("[ARGS]", "");
            }

            result = result.Replace("[PARAMETERS]", parameters.ToString());
            result = result.Replace("[COMMENTS]", func.DocStr);

            return result;
        }

        private PyLibrary InferDTypes(PyLibrary library)
        {
            return library;
        }

        string BaseUrl = "https://keras.io/";

        public Dictionary<string, string> LoadDocs()
        {
            throw new NotImplementedException();
        }
    }
}
