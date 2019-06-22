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

        private void PostProcess_NN_Func(ApiClass api, Function func)
        {
            var name = $"{api.ClassName.Split(".").Last()}.{func.Name}";
            switch (name)
            {
                case "Module.apply":
                    func["fn"].Type = "Action<Module>";
                    break;
                //case "Module.buffers":
                //    func.Returns.Add(new Argument(){ Type = "IEnumerable<Tensor>"});
                //    break;
                //case "Module.children":
                //    func.Arguments.Clear();
                //    func.Returns.Add(new Argument() { Type = "IEnumerable<Module>" });
                //    break;

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
            yield return func;
        }

    }
}
