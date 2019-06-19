using System;
using System.Collections.Generic;
using System.Text;

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
}
