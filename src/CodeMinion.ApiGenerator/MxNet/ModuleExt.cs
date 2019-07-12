using CodeMinion.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMinion.ApiGenerator.MxNet
{
    public class ModuleExt
    {
        /// <summary>
        /// Infers the argument.
        /// </summary>
        public static void InferArg(PyModule module)
        {
            for (int i = 0; i < module.Functions.Length; i++)
            {
                var func = module.Functions[i];
                //if (func.Args == null)
                //    continue;
                ProcessComments(func);

            }

            foreach (var item in module.Classes)
            {
                if (item.Name == "HybridBlock")
                    continue;

                for (int i = 0; i < item.Functions.Length; i++)
                {
                    var func = item.Functions[i];
                    ProcessComments(func);
                    //if (func.Args == null)
                    //    continue;

                    //GetArgs(func);
                }

                if (item.Args == null)
                    continue;

                ProcessComments(item);
            }
        }

        private static void GetArgs(PyFunction func)
        {
            for (int j = func.Args.Length - 1; j >= 0; j--)
            {
                if (func.Args[j] == "self" || func.Args[j] == "prefix" || func.Args[j] == "params")
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
            for (int i = 0; i < cls.Args.Length; i++)
            {
                if (cls.Args[i].Trim() == "self" || cls.Args[i].Trim() == "prefix" || cls.Args[i].Trim() == "params")
                    continue;

                PyFuncArg parameter = new PyFuncArg()
                {
                    Name = cls.Args[i]
                };

                if (cls.Defaults != null)
                {
                    if (cls.Defaults.Length > i)
                    {
                        parameter.HaveDefault = true;
                        parameter.DefaultValue = cls.Defaults[i].Trim().Replace("'", "");
                        parameter.DefaultValue = parameter.DefaultValue.ToString() == "None" ? "null" : parameter.DefaultValue;
                    }
                }

                cls.Parameters.Add(parameter);
            }

            ProcessComments(cls);
        }

        private static void ProcessComments(PyClass cls)
        {
            if (cls.Name.StartsWith("_"))
                return;

            if (cls.DocStr == null)
                return;

            cls.Parameters.Clear();
            if (cls.DocStr.Contains("deprecated"))
            {
                return;
            }

            if (cls.Name == "ROIAlign")
            {
                string str = "";
            }

            string[] splitStr = cls.DocStr.Split(new string[] { "----------", "-------" }, StringSplitOptions.RemoveEmptyEntries);
            cls.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "").Replace("Parameters", "") : "";

            if (splitStr.Length == 1)
                return;

            cls.Parameters = ProcessArgs(cls.Name, splitStr[1]);
            for(int i=0;i<cls.Parameters.Count;i++)
            {
                if (cls.Defaults == null)
                    continue;

                if (cls.Defaults.Length == 0)
                    continue;

                int index = cls.Args.ToList().IndexOf(cls.Parameters[i].Name);
                string defaultVal = cls.Defaults.Length < index ? cls.Defaults[index] : "";
                ProcessDefaults(cls.Parameters[i], defaultVal);
            }
        }

        private static void ProcessComments(PyFunction func)
        {
            if (func.Name.StartsWith("_"))
                return;

            if (func.DocStr == null)
                return;

            func.Parameters.Clear();
            if (func.Name == "foreach")
            {
                string str = "";
            }

            Regex regex = new Regex(@"(\(.*?,.*?\))");

            if (func.Defaults !=null && func.Defaults.Length == 1)
            {
                string defaultRaw = func.Defaults[0].Remove(func.Defaults[0].LastIndexOf(')')).Remove(0, 1);
                var matches = regex.Matches(defaultRaw);
                foreach (Match item in matches)
                {
                    defaultRaw = defaultRaw.Replace(item.Value, item.Value.Replace("(", "").Replace(")", "").Replace(", ", "~"));
                }

                func.Defaults = defaultRaw.Split(",");

                func.Defaults.Reverse();
            }

            if (func.DocStr.Contains("deprecated"))
            {
                func.Deprecated = true;
                return;
            }

            string argSeperator = " : ";
            if (func.Name.Contains("AvgPool") || func.Name.Contains("MaxPool") || func.Name.Contains("ReflectionPad") 
                    || func.Name == "cond" || func.Name == "foreach" || func.Name == "isfinite" || func.Name == "isinf" || func.Name == "isnan"
                    || func.Name == "rand_zipfian")
            {
                argSeperator = ": ";
            }

            string docString = func.DocStr.Replace("References\n----------", "");
            string[] splitStr = docString.Split(new string[] { "----------", "-------", "Inputs:" }, StringSplitOptions.RemoveEmptyEntries);
            func.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "").Replace("Parameters", "") : "";

            if (splitStr.Length == 1)
                return;

            if (docString.Contains("Parameters\n----------"))
                func.Parameters = ProcessArgs(func.Name, splitStr[1]);
            else
                func.Parameters = new List<PyFuncArg>();

            for (int i = 0; i < func.Parameters.Count; i++)
            {
                if (func.Defaults == null)
                    continue;

                if (func.Defaults.Length == 0)
                    continue;

                int index = func.Defaults.Length - i;
                
                string defaultVal = func.Defaults.Length > i ? func.Defaults[index] : "";
                ProcessDefaults(func.Parameters[i], defaultVal);
            }

            if (splitStr.Length > 2)
            {
                string ret_name = splitStr[2].Split(argSeperator)[0];
                string[] retTypeInfo = splitStr[2].Split(argSeperator)[1].Split("\n");
                switch (retTypeInfo[0].Trim())
                {
                    case "boolean":
                        func.ReturnType = "bool";
                        break;
                    case "float":
                        func.ReturnType = "float";
                        break;
                    case "double":
                        func.ReturnType = "double";
                        break;
                    case "int":
                        func.ReturnType = "int";
                        break;
                    case "Context":
                        func.ReturnType = "Context";
                        break;
                    case "NDArray":
                    case "NDArray or list of NDArrays":
                    case "an NDArray or nested lists of NDArrays, representing the result of computation.":
                    case "an NDArray or nested lists of NDArrays.":
                        func.ReturnType = "NDArray";
                        break;
                    default:
                        throw new Exception("Not implemented");
                        //func.ReturnType = retTypeInfo[0].Trim();
                        //break;
                }

                if (retTypeInfo.Length > 1)
                {
                    func.ReturnArg = retTypeInfo[1].Trim();
                }
            }
        }

        private static List<PyFuncArg> ProcessArgs(string objname, string comment)
        {
            List<PyFuncArg> result = new List<PyFuncArg>();
            if(comment.Contains("Inputs:"))
            {
                comment = comment.Split("Inputs:")[0];
            }

            string argSeperator = " : ";
            if(objname.Contains("AvgPool") || objname.Contains("MaxPool") || objname.Contains("ReflectionPad") || objname == "cond" 
                || objname == "foreach" || objname == "isfinite" || objname == "isinf" || objname == "isnan" || objname == "rand_zipfian")
            {
                argSeperator = ": ";
            }

            string[] argsComments = comment.Split(new string[] { "\n", "    " }, StringSplitOptions.RemoveEmptyEntries);
            int counter = -1;
            PyFuncArg parameter = null;
            foreach (var item in argsComments)
            {
                if (item.Contains(argSeperator))
                {
                    counter++;
                    parameter = new PyFuncArg();
                    string name = item.Split(argSeperator)[0].Trim();
                    parameter.Name = name;

                    if (parameter.Name == "self" || parameter.Name == "prefix" || parameter.Name == "params")
                        continue;

                    string argTypeInfo = item.Split(argSeperator)[1];
                    if (name == "out" || name == "name")
                    {
                        continue;
                    }

                    if (argTypeInfo.Contains("{"))
                    {
                        parameter.DataType = ("enum$" + objname + name).Replace("_", "");
                        parameter.Enums = argTypeInfo.Replace("{", "").Split("}")[0].Split(",").Select(x => (x.Replace("'", "").Trim())).ToArray();
                    }
                    else
                    {
                        string[] argInfo = argTypeInfo.Split(argSeperator)[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        switch (argInfo[0].Trim().Replace("`", ""))
                        {
                            case "bool":
                                parameter.DataType = "bool";
                                break;
                            case "boolean":
                                parameter.DataType = "bool";
                                break;
                            case "boolean or None":
                                parameter.DataType = "bool?";
                                break;
                            case "float":
                                parameter.DataType = "float";
                                break;
                            case "int":
                                parameter.DataType = "int";
                                break;
                            case "int or None":
                                parameter.DataType = "int?";
                                parameter.HaveDefault = true;
                                break;
                            case "int (non-negative)":
                                parameter.DataType = "uint";
                                break;
                            case "int or tuple of int":
                                parameter.DataType = "int[]";
                                break;
                            case "tuple of int":
                                parameter.DataType = "int[]";
                                break;

                            case "int or tuple/list of 1 int":
                            case "int or list/tuple of 1 ints":
                            case "int or a tuple/list of 1 int":
                                parameter.DataType = "int[]";
                                break;

                            case "int or tuple/list of 2 int":
                            case "int or list/tuple of 2 ints":
                            case "int or a tuple/list of 2 int":
                                parameter.DataType = "int[]";
                                break;

                            case "int or tuple/list of 3 int":
                            case "int or list/tuple of 3 ints":
                            case "int or a tuple/list of 3 int":
                                parameter.DataType = "int[]";
                                break;

                            case "long":
                                parameter.DataType = "long";
                                break;
                            case "long (non-negative)":
                                parameter.DataType = "ulong";
                                break;
                            case "double":
                                parameter.DataType = "double";
                                break;
                            case "double or None":
                                parameter.DataType = "double?";
                                parameter.HaveDefault = true;
                                break;
                            case "float or None":
                                parameter.DataType = "float?";
                                parameter.HaveDefault = true;
                                break;
                            case "tuple of floats":
                                parameter.DataType = "float[]";
                                break;
                            case "NDArray":
                            case "a MXNet NDArray representing a scalar.":
                            case "an NDArray or a list of NDArrays.":
                            case "an NDArray or nested lists of NDArrays.":
                                parameter.DataType = "NDArray";
                                break;
                            case "NDArray[]":
                                parameter.DataType = "NDArray[]";
                                break;
                            case "Symbol":
                            case "Symbol or list of Symbol":
                                parameter.DataType = "Symbol";
                                break;
                            case "Shape(tuple)":
                                parameter.DataType = "Shape";
                                break;
                            case "Shape or None":
                                parameter.DataType = "Shape";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "string":
                                parameter.DataType = "string";
                                break;
                            case "str":
                                parameter.DataType = "string";
                                break;
                            case "str or None":
                                parameter.DataType = "string";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "";
                                break;
                            case "ParameterDict":
                                parameter.DataType = "Dictionary<string, object>";
                                break;
                            case "ParameterDict or None":
                                parameter.DataType = "Dictionary<string, object>";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "required":
                                parameter.DataType = "float";
                                break;
                            case "numpy.dtype or str":
                                parameter.DataType = "DType";
                                break;
                            case "str or np.dtype":
                                parameter.DataType = "DType";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "Initializer":
                                parameter.DataType = "Initializer";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "callable":
                            case "a Python function.":
                                parameter.DataType = "object";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "str or Initializer":
                                parameter.DataType = "StringOrInitializer";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "np.ndarray or None":
                                parameter.DataType = "Numpy.NDarray";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "str or function":
                                parameter.DataType = "object";
                                break;
                            case "Context":
                                parameter.DataType = "Context";
                                break;
                            case "np":
                                parameter.DataType = "Context";
                                break;
                            case "optional":
                                if(parameter.Name == "variances")
                                {
                                    parameter.DataType = "float[]";
                                }
                                else if (parameter.Name == "sizes" || parameter.Name == "steps" || parameter.Name == "offsets")
                                {
                                    parameter.DataType = "int";
                                }
                                else if (parameter.Name == "ratios" || parameter.Name == "scales" || parameter.Name == "low" || parameter.Name == "high")
                                {
                                    parameter.DataType = "float";
                                }
                                else
                                {
                                    throw new Exception("Not implemented: " + argInfo[0].Trim());
                                }
                                break;
                            default:
                                throw new Exception("Not implemented: " + argInfo[0].Trim());
                        }
                    }

                    result.Add(parameter);
                }
                else
                {
                    parameter.ArgComment += "\n" + item.Trim();
                }
            }

            return result;
        }

        private static void ProcessDefaults(PyFuncArg parameter, string defaultVal)
        {
            defaultVal = defaultVal.Replace("'", "").Replace("`", "").Trim();
            if (defaultVal == "")
                return;

            if (defaultVal == "_Null")
                defaultVal = "None";

            if (parameter.DataType == "float")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "float?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = Convert.ToSingle(defaultVal);
            }
            else if (parameter.DataType == "double")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "double?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = Convert.ToDouble(defaultVal);
            }
            else if (parameter.DataType == "int")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "int?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = Convert.ToInt32(defaultVal);
            }
            else if (parameter.DataType == "uint")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "uint?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = Convert.ToUInt32(defaultVal);
            }
            else if (parameter.DataType == "long")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "long?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = Convert.ToInt64(defaultVal);
            }
            else if (parameter.DataType == "ulong")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "ulong?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = Convert.ToUInt64(defaultVal);
            }
            else if (parameter.DataType == "bool")
            {
                if (defaultVal == "None")
                {
                    parameter.DataType = "bool?";
                    parameter.DefaultValue = "null";
                }
                else
                    parameter.DefaultValue = defaultVal == "True" ? true : false;
            }
            else if (parameter.DataType == "string")
                parameter.DefaultValue = defaultVal == "" ? "\"\"" : "\"" + defaultVal + "\"";
            else if (parameter.DataType == "Dictionary<string, object>")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "DType")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "Initializer")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "StringOrInitializer")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "NDArray")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "Symbol")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "Context")
                parameter.DefaultValue = "null";
            else if (parameter.DataType == "bool?")
            {
                if (defaultVal != "None")
                    parameter.DefaultValue = (defaultVal == "True" ? true : false);
            }
            else if (parameter.DataType == "int?")
            {
                if (defaultVal != "None")
                    parameter.DefaultValue = defaultVal != "" ? Convert.ToInt32(defaultVal) : 0;
            }
            else if (parameter.DataType == "int[]")
            {
                parameter.DefaultValue = "null";
                if (defaultVal != "None")
                    parameter.DefaultValue = defaultVal.Split("~").Select(x => (Convert.ToInt32(x))).ToArray();
            }
            else if (parameter.DataType == "float[]")
            {
                parameter.DefaultValue = "null";
                if (defaultVal != "None")
                    parameter.DefaultValue = defaultVal.Split("~").Select(x => (Convert.ToSingle(x))).ToArray();
            }
            else if (parameter.DataType == "float?")
            {
                if (defaultVal != "None")
                    parameter.DefaultValue = Convert.ToSingle(defaultVal);
            }
            else if (parameter.DataType == "double?")
            {
                if (!defaultVal.Contains("None"))
                    parameter.DefaultValue = Convert.ToDouble(defaultVal);
            }
            else if (parameter.DataType == "Shape")
                parameter.DefaultValue = "null";
            else if (parameter.DataType.StartsWith("enum$"))
                parameter.DefaultValue = defaultVal;
            else
                throw new Exception("Not implemented");
        }
    }
}
