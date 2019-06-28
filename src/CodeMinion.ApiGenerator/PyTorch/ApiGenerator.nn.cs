using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeMinion.Core.Models;

namespace CodeMinion.ApiGenerator.PyTorch
{
    public partial class ApiGenerator
    {
        // nn generator


        private void PostProcessNN_Class(ApiClass api)
        {
            api.BaseClass = "Module";
            switch (api.ClassName)
            {
                case "torch.nn.Module":
                case "torch.nn.Sequential":
                case "torch.nn.ModuleDict":
                case "torch.nn.ModuleList":
                case "torch.nn.ParameterList":
                case "torch.nn.ParameterDict":
                    api.Ignore = true;
                    break;
                case "torch.nn.Parameter":
                    api.BaseClass = "PythonObject";
                    break;
            }
            foreach (var constructor in api.Constructors)
                PostProcessConstructor(api, constructor);
            var decls = api.Declarations.ToArray();
            api.Declarations = new List<Declaration>();
            foreach (var func in decls)
            {
                if (func is Function)
                {
                    PostProcess_NN_Func(api, func as Function);
                    foreach (var overload in InferOverloads_NN(api, func))
                        api.Declarations.Add(overload);
                }
                else
                {
                    PostProcess_NN_Prop(api, func as Property);
                    api.Declarations.Add(func);
                }
            }

        }

        private void PostProcessConstructor(ApiClass c, Function f)
        {
            foreach (var arg in f.Arguments)
            {
                switch (arg.Name)
                {
                    case "modules":
                        arg.Type = "params Module[]";
                        arg.DefaultValue = null;
                        arg.IsNullable = false;
                        break;
                }
            }

            switch (c.ClassName)
            {
                //case "torch.nn.ModuleList":
                //    break;
            }
        }

        private void PostProcess_NN_Func(ApiClass api, Function func)
        {
            foreach(var arg in func.Arguments)
                PostProcess(arg);
            var name = $"{api.ClassName.Split(".").Last()}.{func.Name}";
            switch (name)
            {
                case "Module.apply":
                    func["fn"].Type = "Action<Module>";
                    break;
                case "Module.forward":
                    func.Ignore = true;
                    break;
                case "Module.load_state_dict":
                    func.Returns[0].Type="string[]";
                    func.Returns.Add(new Argument() { Type = "string[]" });
                    break;
                case "Module.named_buffers":
                case "Module.named_children":
                case "Module.named_modules":
                case "Module.named_parameters":
                    func.Returns[0].Type = "IEnumerable<KeyValuePair<string, Tensor>>";
                    if (name == "Module.named_modules")
                    {
                        func["memo"].Type = "HashSet<object>";
                    }
                    break;
                case "Module.register_backward_hook":
                    func["hook"].Type = "Func<Module, Tensor[], Tensor[], Tensor>";
                    break;
                case "Module.register_forward_hook":
                    func["hook"].Type = "Action<Module, Tensor[], Tensor[]>";
                    break;
                case "Module.register_forward_pre_hook":
                    func["hook"].Type = "Action<Module, Tensor[]>";
                    break;
                case "Module.state_dict":
                    func["destination"].Type = "Hashtable";
                    break;
            }
        }

        private void PostProcess_NN_Prop(ApiClass api, Property property)
        {
            if (property.Type==null && property.DefaultValue!=null)
                property.Type = InferPropTypeFromDefaultValue(property);
        }

        private string InferPropTypeFromDefaultValue(Property property)
        {
            switch (property.DefaultValue.ToLower())
            {
                case "false":
                case "true":
                    return "bool";
            }
            return null;
        }

        private IEnumerable<Declaration> InferOverloads_NN(ApiClass api, Declaration func)
        {
            switch (func.Name)
            {
                case "to":
                    yield return new Function() { Name = "to", Arguments =
                        {
                            new Argument() { Name = "device", Type = "Device", },
                            new Argument() { Name = "dtype", Type = "Dtype", },
                            new Argument() { Name = "non_blocking", Type = "bool", DefaultValue = "false"},
                        },
                        Returns = { new Argument() {  Type = "Module" } }
                    };
                    yield return new Function() { Name = "to", Arguments =
                        {
                            new Argument() { Name = "dtype", Type = "Dtype", },
                            new Argument() { Name = "non_blocking", Type = "bool", DefaultValue = "false"},
                        },
                        Returns = { new Argument() { Type = "Module" } }
                    };
                    yield return new Function() { Name = "to", Arguments =
                        {
                            new Argument() { Name = "tensor", Type = "Tensor", },
                            new Argument() { Name = "non_blocking", Type = "bool", DefaultValue = "false"},
                        },
                        Returns = { new Argument() { Type = "Module" } }
                    };
                    yield break;
            }
            yield return func;
        }

    }
}
