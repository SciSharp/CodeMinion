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

        private string codeDir = Directory.GetCurrentDirectory() + "../MxNet/";
        private string testDir = Directory.GetCurrentDirectory() + "../MxNet/UnitTest/";
        private string templateClass = File.ReadAllText("./mxnet/Tmpl/ClassTmpl.txt");
        private string templateMethod = File.ReadAllText("./mxnet/Tmpl/MethodTmpl.txt");

        public string Generate()
        {
            string result = "";
            string json = "";
            using(var gil = Py.GIL())
            {
                dynamic exporter = PythonEngine.ModuleFromString("exporter", File.ReadAllText("./mxnet/ExportSignatureToJson.py"));
                json = exporter.generate().ToString();
            }

            var library = PyLibrary.LoadJson(json);

            GenerateCode("gluon.nn", library);

            return result;
        }

        private void GenerateCode(string moduleName, PyLibrary library)
        {
            string ns = "mxnet." + moduleName;
            string srcFolder = codeDir + moduleName;
            var applicationModule = library.Modules.FirstOrDefault(x => (x.Name == ns));
            ModuleExt.InferArg(applicationModule);
            if(!Directory.Exists(codeDir + moduleName))
            {
                Directory.CreateDirectory(srcFolder);
            }

            foreach (var item in applicationModule.Classes)
            {
                string classString = BuildClassFile(item, moduleName);
            }

            StringBuilder functions = new StringBuilder();
            foreach (var item in applicationModule.Functions)
            {
                string func = BuildFunction(item);
                functions.AppendLine(func);
            }
        }

        private string BuildClassFile(PyClass cls, string moduleName)
        {
            string result = templateClass;
            result = result.Replace("[MODULE]", moduleName);
            result = result.Replace("[CLSNAME]", cls.Name);

            StringBuilder args = new StringBuilder();
            StringBuilder parameters = new StringBuilder();
            foreach (var item in cls.Parameters)
            {
                string type = item.DataType != null ? item.DataType : "object";
                string defaulValue = item.HaveDefault ? " = " + item.DefaultValue : "";
                args.AppendFormat("{0} {1}{2},", type, item.Name, defaulValue);

                parameters.AppendLine(string.Format("parameters[\"{0}\"] = {0};", item.Name));
            }

            if(args.ToString().LastIndexOf(',') > 0)
            {
                result = result.Replace("[ARGS]", args.ToString().Remove(args.ToString().LastIndexOf(',')));
            }

            result = result.Replace("[PARAMETERS]", parameters.ToString());
            result = result.Replace("[COMMENTS]", cls.DocStr);

            return result;
        }

        private string BuildFunction(PyFunction func)
        {
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
