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
                case "torch.nn.parallel.DistributedDataParallelCPU":
                    api.Ignore = true;
                    break;
                case "torch.nn.Parameter":
                    api.BaseClass = "PythonObject";
                    break;
                case "torch.nn.CTCLoss":
                    api.Constructors.Add(new Function()
                    {
                        Name = "torch.nn.CTCLoss",
                        IsConstructor = true,
                        Arguments =
                        {
                            new Argument() { Name = "blank", Type = "int", DefaultValue = "0"},
                            new Argument() { Name = "reduction", Type = "string", DefaultValue = "\"mean\""},
                            new Argument() { Name = "zero_infinity", Type = "bool", DefaultValue = "false"},
                        },
                    });
                    break;
                case "torch.nn.DataParallel":
                    api.Constructors.Add(new Function()
                    {
                        Name = "torch.nn.DataParallel",
                        IsConstructor = true,
                        Arguments =
                        {
                            new Argument() { Name = "module", Type = "Module"},
                            new Argument() { Name = "device_ids", Type = "Device[]", DefaultValue = "null", IsNamedArg = true},
                            new Argument() { Name = "output_device", Type = "Device", DefaultValue = "null", IsNamedArg = true},
                        },
                    });
                    break;
            }
            foreach (var constructor in api.Constructors.ToArray())
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
            f.ClassName = c.ClassName;
            switch (c.ClassName)
            {
                case "torch.nn.AvgPool1d":
                    f["kernel_size"].Type = "int";
                    f["stride"].SetNullableOptional("int");
                    f["padding"].SetNullableOptional("int");
                    f["ceil_mode"].SetNullableOptional("bool");
                    f["count_include_pad"].SetNullableOptional("bool");
                    break;
                case "torch.nn.AvgPool2d":
                case "torch.nn.AvgPool3d":
                    f["kernel_size"].Type = "int[]";
                    f["stride"].SetNullableOptional("int[]");
                    f["padding"].SetNullableOptional("int");
                    f["ceil_mode"].SetNullableOptional("bool");
                    f["count_include_pad"].SetNullableOptional( "bool");
                    break;
                case "torch.nn.BatchNorm1d":
                case "torch.nn.BatchNorm2d":
                case "torch.nn.BatchNorm3d":
                    f["num_features"].Type = "int";
                    f["eps"].SetType("double", "1.0e-5");
                    f["momentum"].SetNullableOptional("double", "0.1");
                    f["affine"].SetType("bool", "true");
                    f["track_running_stats"].SetType("bool", "true");
                    break;
                case "torch.nn.Bilinear":
                    f["in1_features"].Type = "int";
                    f["in2_features"].Type = "int";
                    f["out_features"].Type = "int";
                    f["bias"].SetType("bool", "true");
                    break;
                case "torch.nn.CELU":
                case "torch.nn.ELU":
                    f["alpha"].SetType("double", "1.0");
                    f["inplace"].SetType("bool", "false");
                    break;
                case "torch.nn.DataParallel":
                    if (f["device_ids"].Type == "Device[]")
                        break;
                    f["device_ids"].SetType( "int[]", "null");
                    f["output_device"].SetNullableOptional("int", "null");
                    break;
                case "torch.nn.Dropout":
                case "torch.nn.Dropout2d":
                case "torch.nn.Dropout3d":
                    f["p"].SetType("double", "0.5");
                    break;
                case "torch.nn.FractionalMaxPool2d":
                    f["kernel_size"].SetType("int");
                    f["output_size"].SetNullableOptional("int");
                    f["output_ratio"].SetNullableOptional("double");
                    f["return_indices"].SetType("bool", "false");
                    c.Constructors.Add(f.Clone(clone =>
                    {
                        f["kernel_size"].SetType("int[]");
                        f["output_size"].SetType("int[]", "null");
                        f["output_ratio"].SetType("double[]", "null");
                    }));
                    break;
                case "torch.nn.MultiheadAttention":
                    f["embed_dim"].Type = "int"; // <-- this is just a guess
                    f["num_heads"].Type = "int";
                    break;
                case "torch.nn.parallel.DistributedDataParallel":
                    f["device_ids"].Type = "int[]";
                    f["output_device"].SetNullableOptional( "int");
                    f["process_group"].Type = "PyObject";
                    c.Constructors.Add(c.Constructors[0].Clone(cc =>
                    {
                        cc["device_ids"].Type = "Device[]";
                        cc["output_device"].Type = "Device";
                    }));
                    break;
                case "torch.nn.RReLU":
                    f["lower"].SetNullableOptional("double","null");
                    f["upper"].SetNullableOptional("double", "null");
                    break;
                case "torch.nn.Threshold":
                    f["value"].Type = "double";
                    break;
                case "torch.nn.TripletMarginLoss":
                    f["p"].DefaultValue = "2";
                    break;
                case "torch.nn.Unfold":
                    f["stride"].DefaultValue = "null";
                    f["padding"].DefaultValue = "null";
                    f["padding"].Type = "int[]";
                    f["dilation"].DefaultValue = "null";
                    f["dilation"].Type = "int[]";
                    c.Constructors.Add(c.Constructors[0].Clone(cc =>
                    {
                        cc["stride"].SetType( "int", "1");
                        cc["kernel_size"].Type = "int";
                        cc["padding"].SetType("int", "0");
                        cc["dilation"].SetType("int", "1");
                    }));
                    return;
                case "torch.nn.Upsample":
                    f["size"].Type = "int[]";
                    f["scale_factor"].Type = "double[]";
                    f["align_corners"].DefaultValue = "false";
                    break;
                case "torch.nn.UpsamplingBilinear2d":
                case "torch.nn.UpsamplingNearest2d":
                    f["size"].Type = "int[]";
                    f["scale_factor"].Type = "double[]";
                    break;
                case "torch.nn.Hardtanh":
                    f["min_val"].Type = "double";
                    f["max_val"].Type = "double";
                    break;
                case "torch.nn.PairwiseDistance":
                    f["p"].Type = "double";
                    break;
                case "torch.nn.MaxUnpool1d":
                case "torch.nn.MaxPool1d":
                case "torch.nn.LPPool1d":
                    f["stride"].DefaultValue = "1";
                    break;
                case "torch.nn.LocalResponseNorm":
                    f["k"].Type = "double";
                    break;
            }
            foreach (var arg in f.Arguments)
            {
                switch (arg.Name)
                {
                    case "modules":
                        arg.Type = "params Module[]";
                        arg.DefaultValue = null;
                        arg.IsNullable = false;
                        break;
                    case "output_size":
                    case "num_features":
                    case "in_features":
                    case "out_features":
                        arg.Type = "int";
                        break;
                    case "cutoffs":
                        arg.Type = "int[]";
                        break;
                    case "return_indices":
                        arg.SetType("bool", "false");
                        break;
                    case "inplace":
                        arg.Type = "bool";
                        arg.DefaultValue = "false";
                        break;
                    case "eps":
                        arg.SetType("double", "1.0e-5");
                        break;
                    case "affine":
                        arg.SetType("bool", "true");
                        break;
                    case "args":
                    case "kwargs":
                        arg.Ignore = true;
                        break;
                    case "normalized_shape":
                        arg.SetType("Shape");
                        break;
                    case "kernel_size":
                    case "stride":
                        if (f.Name.EndsWith("1d"))
                            arg.Type = "int";
                        else
                            arg.Type = "int[]";
                        break;
                    case "ceil_mode":
                        if(string.IsNullOrWhiteSpace(arg.Type))
                            arg.SetType("bool", "false");
                        break;
                    case "padding":
                        arg.SetType("int", "0");
                        break;
                    case "dilation":
                        arg.SetType("int", "1");
                        break;
                    case "module":
                        arg.SetType("Module");
                        break;
                    case "reduction":
                        arg.Type = "string";
                        break;
                    case "process_group":
                        arg.Type = "PyObject";
                        break;
                    case "threshold":
                        arg.Type = "double";
                        break;
                }
                if (arg.Type.EndsWith("[]") && arg.DefaultValue != null)
                    arg.DefaultValue = null;
            }
        }

        private void PostProcess_NN_Func(ApiClass api, Function func)
        {
            foreach(var arg in func.Arguments)
                PostProcess(arg);
            var fullname = $"{api.ClassName.Split(".").Last()}.{func.Name}";
            switch (fullname)
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
                    if (fullname == "Module.named_modules")
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
                case "MultiheadAttention.forward":
                    func.Ignore = true;
                    break;
            }
            switch (func.Name)
            {
                case "predict":
                    func.Returns[0].Type = "Tensor";
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

        private IEnumerable<Declaration> InferOverloads_NN(ApiClass api, Declaration f)
        {
            switch (f.Name)
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
            var fullname = $"{api.ClassName.Split(".").Last()}.{f.Name}";
            switch (fullname)
            {

            }
            yield return f;
        }

    }
}
