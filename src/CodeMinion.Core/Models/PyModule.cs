using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Models
{
    public class PyModule
    {
        public string Name { get; set; }

        public PyClass[] Classes { get; set; }

        public PyFunction[] Functions { get; set; }

        public string DocStr { get; set; }

        public void InferArg()
        {
            for (int i = 0; i < Functions.Length; i++)
            {
                var func = Functions[i];
                if (func.Args == null)
                    continue;

                for (int j = func.Args.Length-1; j >= 0; j--)
                {
                    PyFuncArg parameter = new PyFuncArg()
                    {
                        Name = func.Args[j]
                    };

                    if (func.Defaults == null)
                        continue;

                    if(func.Defaults.Length > j)
                    {
                        parameter.HaveDefault = true;
                        parameter.DefaultValue = func.Defaults[j].Trim().Replace("'", "");
                    }

                    func.Parameters.Add(parameter);
                }

            }

            foreach (var item in Classes)
            {
                for (int i = 0; i < item.Functions.Length; i++)
                {
                    var func = Functions[i];
                    if (func.Args == null)
                        continue;

                    for (int j = func.Args.Length-1; j >= 0; j--)
                    {
                        PyFuncArg parameter = new PyFuncArg()
                        {
                            Name = func.Args[j]
                        };

                        if (func.Defaults == null)
                            continue;

                        if (func.Defaults.Length > j)
                        {
                            parameter.HaveDefault = true;
                            parameter.DefaultValue = func.Defaults[j];
                        }

                        func.Parameters.Add(parameter);
                    }

                }
            }
        }
    }

    public class PyClass
    {
        public string Name { get; set; }

        public PyFunction[] Functions { get; set; }

        public string DocStr { get; set; }
    }

    public class PyFunction
    {
        public PyFunction()
        {
            Parameters = new List<PyFuncArg>();
        }

        public string Name { get; set; }

        public string[] Args { get; set; }

        public string[] Defaults { get; set; }

        public string DocStr { get; set; }

        public List<PyFuncArg> Parameters { get; set; }
    }

    public class PyFuncArg
    {
        public string Name { get; set; }

        public Type DataType { get; set; }

        public bool HaveDefault { get; set; }

        public dynamic DefaultValue { get; set; }
    }
}
