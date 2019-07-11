using CodeMinion.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    var func = module.Functions[i];
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

            if (cls.Name == "AvgPool1D")
            {
                string str = "";
            }

            string[] splitStr = cls.DocStr.Split(new string[] { "----------", "-------" }, StringSplitOptions.RemoveEmptyEntries);
            cls.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "").Replace("Parameters", "") : "";

            if (splitStr.Length == 1)
                return;

            cls.Parameters = ProcessArgs(cls.Name, splitStr[1]);
        }

        private static void ProcessComments(PyFunction func)
        {
            if (func.Name.StartsWith("_"))
                return;

            if (func.DocStr == null)
                return;

            func.Parameters.Clear();
            if (func.DocStr.Contains("deprecated"))
            {
                func.Deprecated = true;
                return;
            }

            string argSeperator = " : ";
            if (func.Name.Contains("AvgPool"))
            {
                argSeperator = ": ";
            }

            string[] splitStr = func.DocStr.Split(new string[] { "----------", "-------", "Inputs:" }, StringSplitOptions.RemoveEmptyEntries);
            func.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "").Replace("Parameters", "") : "";

            if (splitStr.Length == 1)
                return;

            func.Parameters = ProcessArgs(func.Name, splitStr[1]);

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
                    case "NDArray or list of NDArrays":
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
            if(objname.Contains("AvgPool") || objname.Contains("MaxPool") || objname.Contains("ReflectionPad"))
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
                            case "NDArray":
                                parameter.DataType = "NDArray";
                                break;
                            case "NDArray[]":
                                parameter.DataType = "NDArray[]";
                                break;
                            case "Symbol":
                                parameter.DataType = "Symbol";
                                break;
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
                            case "str or Initializer":
                                parameter.DataType = "StringOrInitializer";
                                parameter.HaveDefault = true;
                                parameter.DefaultValue = "null";
                                break;
                            case "str or function":
                                parameter.DataType = "object";
                                break;
                            default:
                                throw new Exception("Not implemented: " + argInfo[0].Trim());
                        }
                    }

                    if (argTypeInfo.Contains("optional"))
                    {
                        parameter.HaveDefault = true;

                        if (argTypeInfo.Contains("default="))
                        {
                            string val = argTypeInfo.Split("default=")[1].Trim().Replace("'", "");
                            if (parameter.DataType == "float")
                                parameter.DefaultValue = Convert.ToSingle(val);
                            else if (parameter.DataType == "double")
                                parameter.DefaultValue = Convert.ToDouble(val);
                            else if (parameter.DataType == "int")
                                parameter.DefaultValue = Convert.ToInt32(val);
                            else if (parameter.DataType == "uint")
                                parameter.DefaultValue = Convert.ToUInt32(val);
                            else if (parameter.DataType == "long")
                                parameter.DefaultValue = Convert.ToInt64(val);
                            else if (parameter.DataType == "ulong")
                                parameter.DefaultValue = Convert.ToUInt64(val);
                            else if (parameter.DataType == "bool")
                                parameter.DefaultValue = val == "1" ? true : false;
                            else if (parameter.DataType == "string")
                                parameter.DefaultValue = val == "" ? "" : val;
                            else if (parameter.DataType == "Dictionary<string, object>")
                                parameter.DefaultValue = "null";
                            else if (parameter.DataType == "DType")
                                parameter.DefaultValue = "null";
                            else if (parameter.DataType == "Initializer")
                                parameter.DefaultValue = "null";
                            else if (parameter.DataType == "StringOrInitializer")
                                parameter.DefaultValue = "null";
                            else if (parameter.DataType == "bool?")
                            {
                                if (!argTypeInfo.Split("default=")[1].Trim().Contains("None"))
                                    parameter.DefaultValue = (val == "1" ? true : false);
                            }
                            else if (parameter.DataType == "int?")
                            {
                                if (!argTypeInfo.Split("default=")[1].Trim().Contains("None"))
                                    parameter.DefaultValue = val != "" ? Convert.ToInt32(val) : 0;
                            }
                            else if (parameter.DataType == "int[]")
                            {
                                parameter.DefaultValue = "null";
                            }
                            else if (parameter.DataType == "float?")
                            {
                                if (!val.Contains("None"))
                                    parameter.DefaultValue = Convert.ToSingle(val);
                            }
                            else if (parameter.DataType == "double?")
                            {
                                if (!val.Contains("None"))
                                    parameter.DefaultValue = Convert.ToDouble(val);
                            }
                            else if (parameter.DataType == "Shape")
                                parameter.DefaultValue = "null";
                            else if (parameter.DataType.StartsWith("enum$"))
                                parameter.DefaultValue = val;
                            else
                                throw new Exception("Not implemented");
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
    }
}
