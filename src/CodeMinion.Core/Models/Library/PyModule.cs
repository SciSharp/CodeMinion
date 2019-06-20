using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Python module information
    /// </summary>
    public class PyModule
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the classes.
        /// </summary>
        /// <value>
        /// The classes.
        /// </value>
        public PyClass[] Classes { get; set; }

        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        public PyFunction[] Functions { get; set; }

        /// <summary>
        /// Gets or sets the document string.
        /// </summary>
        /// <value>
        /// The document string.
        /// </value>
        public string DocStr { get; set; }

        /// <summary>
        /// Infers the argument.
        /// </summary>
        public void InferArg()
        {
            for (int i = 0; i < Functions.Length; i++)
            {
                var func = Functions[i];
                if (func.Args == null)
                    continue;
                GetArgs(func);

            }

            foreach (var item in Classes)
            {
                for (int i = 0; i < item.Functions.Length; i++)
                {
                    var func = Functions[i];
                    if (func.Args == null)
                        continue;

                    GetArgs(func);
                }

                if (item.Args == null)
                    continue;

                GetArgs(item);
            }
        }

        private static void GetArgs(PyFunction func)
        {
            for (int j = func.Args.Length - 1; j >= 0; j--)
            {
                if (func.Args[j] == "self")
                    continue;

                PyFuncArg parameter = new PyFuncArg()
                {
                    Name = func.Args[j]
                };

                if (func.Defaults != null)
                {
                    if (func.Defaults.Length > j)
                    {
                        parameter.HaveDefault = true;
                        parameter.DefaultValue = func.Defaults[j].Trim().Replace("'", "");
                        parameter.DefaultValue = parameter.DefaultValue.ToString() == "None" ? "null" : parameter.DefaultValue;
                    }
                }

                func.Parameters.Add(parameter);
            }

            ProcessComments(func);
        }

        private static void GetArgs(PyClass cls)
        {
            for (int j = cls.Args.Length - 1; j >= 0; j--)
            {
                if (cls.Args[j] == "self")
                    continue;

                PyFuncArg parameter = new PyFuncArg()
                {
                    Name = cls.Args[j]
                };

                if (cls.Defaults != null)
                {
                    if (cls.Defaults.Length > j)
                    {
                        parameter.HaveDefault = true;
                        parameter.DefaultValue = cls.Defaults[j].Trim().Replace("'", "");
                        parameter.DefaultValue = parameter.DefaultValue.ToString() == "None" ? "null" : parameter.DefaultValue;
                    }
                }

                cls.Parameters.Add(parameter);
            }

            ProcessComments(cls);
        }

        private static void ProcessComments(PyClass cls)
        {
            if (cls.DocStr == null)
                return;

            string[] splitStr = cls.DocStr.Split(new string[] { "# " }, StringSplitOptions.RemoveEmptyEntries);
            cls.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "") : "";

            foreach (var item in splitStr)
            {
                if(item.StartsWith("Arguments"))
                {
                    string[] args = cls.Args.Select(x => (x + ": ")).ToArray().Where(x => (!x.Contains("kwargs"))).ToArray();
                    string arg = item.Replace("Arguments", "").Trim().Replace("\n", "").Replace("        ", "");
                    string[] pargs = arg.Split(args, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < cls.Parameters.Count; i++)
                    {
                        cls.Parameters[i].ArgComment = pargs[i];
                    }
                }
            }
        }

        private static void ProcessComments(PyFunction func)
        {
            if (func.DocStr == null)
                return;

            string[] splitStr = func.DocStr.Split(new string[] { "# " }, StringSplitOptions.RemoveEmptyEntries);
            func.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "") : "";

            foreach (var item in splitStr)
            {
                if (item.StartsWith("Arguments"))
                {
                    string[] args = func.Args.Select(x => (x + ": ")).ToArray();
                    string arg = item.Replace("Arguments", "").Trim().Replace("\n", "").Replace("        ", "");
                    string[] pargs = arg.Split(args, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < func.Parameters.Count; i++)
                    {
                        func.Parameters[i].ArgComment = pargs[i];
                    }
                }

                if(item.StartsWith("Returns"))
                {
                    func.ReturnArg = item.Replace("Returns", "").Trim().Replace("\n", "");
                }
            }
        }
    }
}
