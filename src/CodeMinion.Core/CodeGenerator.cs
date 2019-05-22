using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeMinion.Core.Attributes;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeMinion.Core
{
    public class CodeGenerator
    {
        public CodeGenerator()
        {
            LoadTemplates();
        }

        public List<StaticApi> StaticApis { get; set; } = new List<StaticApi>();
        public List<DynamicApi> DynamicApis { get; set; } = new List<DynamicApi>();
        public bool PrintModelJson { get; set; } = false;
        public string NameSpace { get; set; } = "Numpy";
        public HashSet<string> Usings { get; set; } = new HashSet<string>()
        {
            @"using System;",
            @"using System.Collections;",
            @"using System.Collections.Generic;",
            @"using System.IO;",
            @"using System.Linq;",
            @"using System.Runtime.InteropServices;",
            @"using System.Text;",
            @"using Python.Runtime;",
            @"using Python.Included;",
        };

        protected Dictionary<string, FunctionBodyTemplate> _templates;
        protected virtual void LoadTemplates()
        {
            _templates = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.GetCustomAttribute<TemplateAttribute>() != null)
                .Select(x => (FunctionBodyTemplate)Activator.CreateInstance(x)).ToDictionary(x =>
                    x.GetType().GetCustomAttribute<TemplateAttribute>().ApiFunction);
        }

        // generate an entire API function declaration
        protected virtual void GenerateApiFunction(Declaration decl, CodeWriter s)
        {
            if (decl.DebuggerBreak)
                Debugger.Break();
            if (decl.CommentOut)
                s.Out("/*");
            GenerateDocString(decl, s);
            var retval = GenerateReturnType(decl);
            switch (decl)
            {
                case Function func:
                    var arguments = GenerateArguments(func);
                    var passed_args = GeneratePassedArgs(func);
                    var generics = func.Generics == null ? "" : $"<{string.Join(",", func.Generics)}>";
                    s.Out($"public {retval} {EscapeName(decl.Name)}{generics}({arguments})");
                    s.Block(() =>
                    {
                        GenerateFunctionBody(func, s);
                    });
                    break;
                case Property prop:
                    s.Out($"public {retval} {EscapeName(decl.Name)}");
                    s.Block(() =>
                    {
                        s.Out("get", () =>
                        {
                            GeneratePropertyGetter(prop, s);
                        });
                    });
                    break;
            }
            if (decl.CommentOut)
                s.Out("*/");
            if (PrintModelJson)
            {
                s.Out("// the declaration model:");
                s.Out("/*");
                s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                s.Out("*/");
            }
            s.Break();
        }

        protected virtual void GenerateStaticApiRedirection(StaticApi api, Declaration decl, CodeWriter s)
        {
            if (decl.DebuggerBreak)
                Debugger.Break();
            if (decl.CommentOut)
                s.Out("/*");
            GenerateDocString(decl, s);
            var retval = GenerateReturnType(decl);
            switch (decl)
            {
                case Function func:
                    var arguments = GenerateArguments(func);
                    var passed_args = GeneratePassedArgs(func);
                    var generics = func.Generics == null ? "" : $"<{string.Join(",", func.Generics)}>";
                    s.Out($"public static {retval} {EscapeName(decl.Name)}{generics}({arguments})");
                    s.Indent(() => s.Out(
                        $"=> {api.ImplName}.Instance.{EscapeName(decl.Name)}({passed_args});"));
                    break;
                case Property prop:
                    s.Out($"public static {retval} {EscapeName(decl.Name)}");
                    s.Indent(() => s.Out(
                        $"=> {api.ImplName}.Instance.{EscapeName(decl.Name)};"));
                    break;
            }
            if (decl.CommentOut)
                s.Out("*/");
            s.Break();
        }

        // generate the argument list between the parentheses of a generated API function
        protected virtual string GenerateArguments(Function decl)
        {
            var s = new StringBuilder();
            int i = 0;
            foreach (var arg in decl.Arguments)
            {
                if (arg.Type == "string")
                    arg.IsValueType = false;
                // TODO modifier (if any)
                // parameter type
                CheckArgument(arg);
                s.Append(arg.Type);
                if (arg.IsNullable && arg.IsValueType)
                    s.Append("?");
                s.Append(" ");
                // parameter name
                s.Append(EscapeName(arg.Name));
                if (!string.IsNullOrWhiteSpace(arg.DefaultValue))
                    s.Append($" = {MapDefaultValue(arg)}");
                else if (arg.IsNullable)
                    s.Append($" = null");
                i++;
                if (i < decl.Arguments.Count)
                    s.Append(", ");
            }
            return s.ToString();
        }

        private string GeneratePassedArgs(Function decl)
        {
            var s = new StringBuilder();
            int i = 0;
            foreach (var arg in decl.Arguments)
            {
                // TODO modifier (if any)
                var argname = EscapeName(arg.Name);
                if (arg.IsNamedArg)
                    s.Append($"{argname}:");
                s.Append(argname);
                i++;
                if (i < decl.Arguments.Count)
                    s.Append(", ");
            }
            return s.ToString();
        }

        //protected virtual IEnumerable<Declaration> ExpandOverloads(Declaration decl)
        //{
        //    // todo: let's hope there are not multiple expansions in one declaration, or else this will get complicated
        //    if (decl.Arguments.Any(a => a.Type == "(array_like)"))
        //    {
        //        foreach (var type in "NumSharp.NDArray T[]".Split())
        //        {
        //            var clone_decl = decl.Clone();
        //            clone_decl.Arguments.ForEach(a =>
        //            {
        //                if (a.Type == "(array_like)")
        //                    a.Type = type;
        //            });
        //            if (type == "T[]")
        //                clone_decl.Generics = new string[] { "T" };
        //            yield return clone_decl;
        //        }
        //        yield break;
        //    }
        //    yield return decl;
        //}


        // maps None to null, etc
        protected virtual string MapDefaultValue(Argument arg)
        {
            switch (arg.DefaultValue)
            {
                case "None": return "null";
                case "True": return "true";
                case "False": return "false";
            }
            return arg.DefaultValue;
        }

        // list of c# keywords that are not allowed as variable names or parameter names
        protected readonly HashSet<string> _disallowed_names = new HashSet<string>()
        {
            "abstract", "as", "base", "bool", "break",
            "byte", "case", "catch", "char", "checked",
            "class",   "const",   "continue", "decimal", "default",
            "delegate",    "do", "double",  "else", "enum",
            "event",   "explicit", "extern", "false", "finally",
            "fixed", "float",   "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace",
            "new", "null", "object", "operator",    "out",
            "override", "params",  "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string",
            "struct",  "switch", "this",    "throw", "true",
            "try", "typeof", "uint",    "ulong",   "unchecked",
            "unsafe", "ushort", "using", "var", "virtual",
            "void", "volatile", "while",
            "add", "alias",   "async", "await", "dynamic",
            "get", "global",  "nameof",  "partial", "remove",
            "set", "value", "when",    "where", "yield",
            "ascending", "by",  "descending", "equals",  "from",
            "group",   "in", "into", "join",    "let",
            "on",  "orderby", "select",  "where"
        };

        // escape a varibale name if it violates C# syntax
        protected virtual string EscapeName(string name)
        {
            if (_disallowed_names.Contains(name))
                return "@" + name;
            return name;
        }

        // generates the return type declaration of a generated API function declaration
        protected virtual string GenerateReturnType(Declaration decl)
        {
            if (decl.Returns == null || decl.Returns.Count == 0)
                return "void";
            else if (decl.Returns.Count == 1)
            {
                CheckArgument(decl.Returns[0]);
                return decl.Returns[0].Type;
            }
            else
            {
                throw new NotImplementedException("Return tuple");
            }
        }

        // argument type consistency checks
        protected virtual void CheckArgument(Argument arg)
        {
            switch (arg.Type)
            {
                // basic types
                case "bool":
                case "int":
                case "long":
                case "double":
                case "float":
                    arg.IsValueType = true;
                    break;
                case "object":
                case "string":
                    arg.IsValueType = false;
                    break;
                    // sequence types
            }
        }

        /// <summary>
        /// Generate the xml doc string from the description(s)
        /// </summary>
        /// <param name="decl"></param>
        /// <param name="s"></param>
        protected virtual void GenerateDocString(Declaration decl, CodeWriter s)
        {
            if (string.IsNullOrWhiteSpace(decl.Description))
                return;
            s.Out("/// <summary>");
            foreach (var line in Regex.Split(decl.Description, @"\r?\n"))
                s.Out("/// " + line);
            s.Out("/// </summary>");
            if (decl is Function)
            {
                var func = decl as Function;
                foreach (var arg in func.Arguments)
                {
                    if (string.IsNullOrWhiteSpace(arg.Description))
                        continue;
                    s.Out($"/// <param name=\"{EscapeName(arg.Name)}\">");
                    foreach (var line in Regex.Split(arg.Description, @"\r?\n"))
                        s.Out("/// " + line);
                    s.Out("/// </param>");
                }
            }
            if (decl.Returns.All(rv => string.IsNullOrWhiteSpace(rv.Description)))
                return;
            s.Out("/// <returns>");
            if (decl.Returns.Count == 1)
                foreach (var line in Regex.Split(decl.Returns[0].Description, @"\r?\n"))
                    s.Out("/// " + line);
            else
            {
                s.Out("/// A tuple of:");
                foreach (var rv in decl.Returns)
                {
                    s.Out("/// " + rv.Name);
                    foreach (var line in Regex.Split(rv.Description, @"\r?\n"))
                        s.Out("/// " + line);

                }
            }
            s.Out("/// </returns>");
        }

        // generates only the body of the API function declaration
        protected virtual void GenerateFunctionBody(Function func, CodeWriter s)
        {
            if (_templates.ContainsKey(func.Name))
            {
                // use generator template instead
                _templates[func.Name].GenerateBody(func, s);
                return;
            }
            s.Out("//auto-generated code, do not change");
            if (func.Arguments.Any())
            {
                // first generate the positional args
                s.Out($"var pyargs=ToTuple(new object[]");
                s.Block(() =>
                {
                    foreach (var arg in func.Arguments.Where(a => a.IsNamedArg == false))
                    {
                        s.Out($"{EscapeName(arg.Name)},");
                    }
                }, "{", "});");
                // then generate the named args
                s.Out($"var kwargs=new PyDict();");
                foreach (var arg in func.Arguments.Where(a => a.IsNamedArg == true))
                {
                    var name = EscapeName(arg.Name);
                    s.Out($"if ({name}!=null) kwargs[\"{arg.Name}\"]=ToPython({name});");
                }
                // then call the function
                s.Out($"dynamic py = self.InvokeMethod(\"{func.Name}\", pyargs, kwargs);");
            }
            else
            {
                // call function with no arguments
                s.Out($"dynamic py = self.InvokeMethod(\"{func.Name}\");");
            }
            // return the return value if any
            if (func.Returns.Count == 0)
                return;
            if (func.Returns.Count == 1)
                s.Out($"return ToCsharp<{func.Returns[0].Type}>(py);");
            else
            {
                throw new NotImplementedException("return a tuple or array of return values");
            }
        }

        private void GeneratePropertyGetter(Property prop, CodeWriter s)
        {
            //if (_templates.ContainsKey(prop.Name))
            //{
            //    // use generator template instead
            //    _templates[prop.Name].GenerateBody(prop, s);
            //    return;
            //}
            s.Out("//auto-generated code, do not change");
            // call proption with no arguments
            s.Out($"dynamic py = self.GetAttr(\"{prop.Name}\");");
            // return the return value if any
            if (prop.Returns.Count == 0)
                return;
            if (prop.Returns.Count == 1)
                s.Out($"return ToCsharp<{prop.Returns[0].Type}>(py);");
            else
            {
                throw new NotImplementedException("return a tuple or array of return values");
            }
        }

        public virtual void GenerateStaticApi(StaticApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.Out($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public static partial class {api.StaticName}");
                s.Block(() =>
                {
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            GenerateStaticApiRedirection(api, decl, s);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");

                        }
                    }
                    s.Break();
                });
            });
        }

        private void GenerateUsings(CodeWriter s)
        {
            foreach (var @using in Usings)
            {
                s.AppendLine(@using);
            }
            s.AppendLine();
        }

        public virtual void GenerateApiImpl(StaticApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public partial class {api.ImplName}");
                s.Block(() =>
                {
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            if (!decl.ManualOverride)
                                GenerateApiFunction(decl, s);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");

                        }
                    }
                });
            });
        }

        public virtual void GenerateDynamicApi(DynamicApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public partial class {api.ClassName}");
                s.Block(() =>
                {
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            if (!decl.ManualOverride)
                                GenerateApiFunction(decl, s);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");
                        }
                    }
                });
            });
        }

        protected void WriteFile(string path, Action<CodeWriter> generate_action)
        {
            var s = new CodeWriter();
            try
            {
                generate_action(s);
            }
            catch (Exception e)
            {
                s.AppendLine("/*");
                s.AppendLine("\r\n --------------- generator exception ---------------------");
                s.AppendLine(e.Message);
                s.AppendLine(e.StackTrace);
                s.AppendLine("*/");
            }
            File.WriteAllText(path, s.ToString());
        }

        public void Generate()
        {
            // generate all static apis that have been configured
            var generated_implementations = new HashSet<string>();
            foreach (var api in StaticApis)
            {
                // generate static apis
                if (!generated_implementations.Contains(api.ImplName))
                {
                    var conv_file = Path.Combine(api.OutputPath, $"{api.ImplName}.conv.gen.cs");
                    WriteFile(conv_file, s => { GenerateApiImplConversions(api, s); });
                }

                generated_implementations.Add(api.ImplName);
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(api.OutputPath, $"{api.StaticName + partial}.gen.cs");
                var impl_file = Path.Combine(api.OutputPath, $"{api.ImplName + partial}.gen.cs");
                WriteFile(api_file, s => { GenerateStaticApi(api, s); });
                WriteFile(impl_file, s => { GenerateApiImpl(api, s); });
            }
            bool generated_pyobject = false;
            foreach (var api in DynamicApis)
            {
                if (!generated_pyobject)
                {
                    // PythonObject functions:
                    var pyobj_file = Path.Combine(api.OutputPath, $"PythonObject.gen.cs");
                    WriteFile(pyobj_file, s => { GeneratePythonObjectConversions(s); });
                    generated_pyobject = true;
                }
                // generate dynamic apis
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(api.OutputPath, $"{api.ClassName + partial}.gen.cs");
                WriteFile(api_file, s => { GenerateDynamicApi(api, s); });
            }
        }

        private void GenerateApiImplConversions(StaticApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public partial class {api.ImplName} : IDisposable");
                s.Block(() =>
                {
                    s.Break();
                    s.AppendLine($"private static Lazy<{api.ImplName}> _instance = new Lazy<{api.ImplName}>(() => new {api.ImplName}());");
                    s.AppendLine($"public static {api.ImplName} Instance => _instance.Value;");
                    s.Break();
                    s.Out("private Lazy<PyObject> _pyobj = new Lazy<PyObject>(() =>", () =>
                    {
                        s.Out("PyObject mod = null;");
                        s.Out("try", () =>
                            {
                                s.Out("mod = InstallAndImport();");
                            });
                        s.Out("catch (Exception)", () =>
                        {
                            s.Out("// retry to fix the installation by forcing a repair.");
                            s.Out("mod = InstallAndImport(force: true);");
                        });
                        s.Out("return mod;");
                    });
                    s.Out(");");
                    s.Break();
                    s.Out("private static PyObject InstallAndImport(bool force = false)", () =>
                    {
                        s.Out("var installer = new Installer();");
                        s.Out("installer.SetupPython(force).Wait();");
                        foreach (var generator in api.InitializationGenerators)
                            generator(s);
                        //s.Out("installer.InstallWheel(typeof(NumPy).Assembly, \"numpy -1.16.3-cp37-cp37m-win_amd64.whl\", force).Wait();");
                        s.Out("PythonEngine.Initialize();");
                        s.Out($"var mod = Py.Import(\"{api.PythonModule}\");");
                        s.Out("return mod;");
                    });
                    s.Break();
                    s.Out("public dynamic self => _pyobj.Value;");
                    s.Out("private bool IsInitialized => _pyobj != null;");
                    s.Break();
                    s.Out("private NumPy() { }");
                    s.Break();
                    s.Out("public void Dispose()", () =>
                    {
                        s.Out("self?.Dispose();");
                    });
                    s.Break();
                    GenToTuple(s);
                    GenToPython(s);
                    GenToCsharp(s);
                    GenSpecialConversions(s);
                });
            });
        }

        private void GeneratePythonObjectConversions(CodeWriter s)
        {
            GenerateUsings(s);
            s.Out($"namespace {NameSpace}", () =>
            {
                s.Out($"public partial class PythonObject", () =>
                {
                    s.Break();
                    GenToTuple(s);
                    GenToPython(s);
                    GenToCsharp(s);
                    GenSpecialConversions(s);
                });
            });
        }
        private void GenToTuple(CodeWriter s)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out("protected PyTuple ToTuple(Array input)", () =>
            {
                s.Out("var array = new PyObject[input.Length];");
                s.Out("for (int i = 0; i < input.Length; i++)", () =>
                {
                    s.Out("array[i]=ToPython(input.GetValue(i));");
                });
                s.Out("return new PyTuple(array);");
            });
        }

        public HashSet<string> ToCsharpConversions { get; set; } = new HashSet<string>();

        private void GenToCsharp(CodeWriter s)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out("protected T ToCsharp<T>(dynamic pyobj)", () =>
            {
                s.Out("switch (typeof(T).Name)", () =>
                {
                    s.Out("// types from 'ToCsharpConversions'");
                    foreach (var @case in ToCsharpConversions)
                    {
                        s.Out(@case);
                    }
                    s.Out("default: return (T)pyobj;");
                });
            });
        }

        public HashSet<string> ToPythonConversions { get; set; } = new HashSet<string>();

        private void GenToPython(CodeWriter s)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out("protected PyObject ToPython(object obj)", () =>
            {
                s.Out("if (obj == null) return null;");
                s.Out("switch (obj)", () =>
                {
                    s.Out("// basic types");
                    s.Out("case int o: return new PyInt(o);");
                    s.Out("case float o: return new PyFloat(o);");
                    s.Out("case double o: return new PyFloat(o);");
                    s.Out("case string o: return new PyString(o);");
                    // case dtype o: return o.ToPython();
                    s.Out("// sequence types");
                    s.Out("case Array o: return ToTuple(o);");
                    s.Out("// special types from 'ToPythonConversions'");
                    foreach (var @case in ToPythonConversions)
                    {
                        s.Out(@case);
                    }
                    s.Out("default: throw new NotImplementedException($\"Type is not yet supported: { obj.GetType().Name}. Add it to 'ToPythonConversions'\");");
                });
            });
        }

        public List<Action<CodeWriter>> SpecialConversionGenerators { get; set; } = new List<Action<CodeWriter>>();

        private void GenSpecialConversions(CodeWriter s)
        {
            foreach (var generator in SpecialConversionGenerators)
            {
                s.Break();
                s.Out("//auto-generated: SpecialConversions");
                generator(s);
            }
        }

    }
}
