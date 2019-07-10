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
                if (func.Args == null)
                    continue;
                GetArgs(func);

            }

            foreach (var item in module.Classes)
            {
                for (int i = 0; i < item.Functions.Length; i++)
                {
                    var func = module.Functions[i];
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
            for (int i = 0; i < cls.Args.Length; i++)
            {
                if (cls.Args[i] == "self")
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

        private static void ProcessComments(PyClass func)
        {
            if (func.DocStr == null)
                return;

            func.Parameters.Clear();
            if (func.DocStr.Contains("deprecated"))
            {
                return;
            }

            //func.Parameters.Reverse();

            string[] splitStr = func.DocStr.Split(new string[] { "----------", "-------" }, StringSplitOptions.RemoveEmptyEntries);
            func.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "").Replace("Parameters", "") : "";

            string[] argsComments = splitStr[1].Split(new string[] { "\n", "    ", "Inputs:" }, StringSplitOptions.RemoveEmptyEntries);
            int counter = -1;
            PyFuncArg parameter = null;
            foreach (var item in argsComments)
            {
                if (item.Contains(" : "))
                {
                    counter++;
                    parameter = new PyFuncArg();
                    string name = item.Split(" : ")[0].Trim();
                    parameter.Name = name;
                    string argTypeInfo = item.Split(" : ")[1];
                    if (name == "out" || name == "name")
                    {
                        continue;
                    }

                    if (argTypeInfo.Contains("{"))
                    {
                        parameter.DataType = ("enum$" + func.Name + name).Replace("_", "");
                        parameter.Enums = argTypeInfo.Replace("{", "").Split("}")[0].Split(",").Select(x => (x.Replace("'", "").Trim())).ToArray();
                    }
                    else
                    {
                        string[] argInfo = argTypeInfo.Split(" : ")[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        switch (argInfo[0].Trim())
                        {
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
                                break;
                            case "int (non-negative)":
                                parameter.DataType = "uint";
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
                                break;
                            case "float or None":
                                parameter.DataType = "float?";
                                break;
                            case "NDArray":
                                parameter.DataType = "NDArray";
                                break;
                            case "array-like":
                                parameter.DataType = "NDArray";
                                break;
                            case "NDArray[]":
                                parameter.DataType = "NDArray[]";
                                break;
                            case "Symbol":
                                parameter.DataType = "Symbol";
                                break;
                            case "Shape(tuple)":
                                parameter.DataType = "Shape";
                                break;
                            case "Shape or None":
                                parameter.DataType = "Shape";
                                break;
                            case "string":
                                parameter.DataType = "string";
                                break;
                            case "str":
                                parameter.DataType = "str";
                                break;
                            case "ParameterDict":
                                parameter.DataType = "Dictionary<string, object>";
                                break;
                            case "ParameterDict or None":
                                parameter.DataType = "Dictionary<string, object>";
                                break;
                            case "required":
                                parameter.DataType = "float";
                                break;
                            default:
                                throw new Exception("Not implemented");
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
                            else if (parameter.DataType == "str")
                                parameter.DefaultValue = val == "" ? "" : val;
                            else if (parameter.DataType == "Dictionary<string, object>")
                                parameter.DefaultValue = null;
                            else if (parameter.DataType == "bool?")
                            {
                                if (!argTypeInfo.Split("default=")[1].Trim().Contains("None"))
                                    parameter.DefaultValue = (val == "1" ? true : false);
                            }
                            else if (parameter.DataType == "int?")
                            {
                                if (!argTypeInfo.Split("default=")[1].Trim().Contains("None"))
                                    parameter.DefaultValue = Convert.ToInt32(val);
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
                                parameter.DefaultValue = null;
                            else if (parameter.DataType.StartsWith("enum$"))
                                parameter.DefaultValue = val;
                            else
                                throw new Exception("Not implemented");
                        }
                    }

                    func.Parameters.Add(parameter);
                }
                else
                {
                    parameter.ArgComment += "\n" + item.Trim();
                }
            }
        }

        private static void ProcessComments(PyFunction func)
        {
            if (func.DocStr == null)
                return;

            func.Parameters.Clear();
            if (func.DocStr.Contains("deprecated"))
            {
                func.Deprecated = true;
                return;
            }

            //func.Parameters.Reverse();

            string[] splitStr = func.DocStr.Split(new string[] { "----------", "-------" }, StringSplitOptions.RemoveEmptyEntries);
            func.DocStr = splitStr.Length > 0 ? splitStr[0].Replace("\n", "").Replace("Parameters", "") : "";

            string[] argsComments = splitStr[1].Split(new string[] { "\n", "    " }, StringSplitOptions.RemoveEmptyEntries);
            int counter = -1;
            PyFuncArg parameter = null;
            foreach (var item in argsComments)
            {
                if (item.Contains(" : "))
                {
                    counter++;
                    parameter = new PyFuncArg();
                    string name = item.Split(" : ")[0].Trim();
                    parameter.Name = name;
                    string argTypeInfo = item.Split(" : ")[1];
                    if(name == "out" || name == "name")
                    {
                        continue;
                    }

                    if (argTypeInfo.Contains("{"))
                    {
                        parameter.DataType = ("enum$" + func.Name + name).Replace("_", "");
                        parameter.Enums = argTypeInfo.Replace("{", "").Split("}")[0].Split(",").Select(x => (x.Replace("'", "").Trim())).ToArray();
                    }
                    else
                    {
                        string[] argInfo = argTypeInfo.Split(" : ")[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        switch (argInfo[0].Trim())
                        {
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
                                break;
                            case "int (non-negative)":
                                parameter.DataType = "uint";
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
                                break;
                            case "float or None":
                                parameter.DataType = "float?";
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
                            case "Shape(tuple)":
                                parameter.DataType = "Shape";
                                break;
                            case "Shape or None":
                                parameter.DataType = "Shape";
                                break;
                            case "string":
                                parameter.DataType = "string";
                                break;
                            case "required":
                                parameter.DataType = "float";
                                break;
                            default:
                                throw new Exception("Not implemented");
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
                            else if (parameter.DataType == "bool?")
                            {
                                if (!argTypeInfo.Split("default=")[1].Trim().Contains("None"))
                                    parameter.DefaultValue = (val == "1" ? true : false);
                            }
                            else if (parameter.DataType == "int?")
                            {
                                if (!argTypeInfo.Split("default=")[1].Trim().Contains("None"))
                                    parameter.DefaultValue = Convert.ToInt32(val);
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
                                parameter.DefaultValue = null;
                            else if (parameter.DataType.StartsWith("enum$"))
                                parameter.DefaultValue = val;
                            else
                                throw new Exception("Not implemented");
                        }
                    }

                    func.Parameters.Add(parameter);
                }
                else
                {
                    parameter.ArgComment += "\n" + item.Trim();
                }
            }

            if (splitStr.Length > 2)
            {
                string ret_name = splitStr[2].Split(" : ")[0];
                string[] retTypeInfo = splitStr[2].Split(" : ")[1].Split("\n");
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
    }
}
