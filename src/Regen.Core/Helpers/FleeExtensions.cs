using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using Regen.Compiler;
using Regen.DataTypes;
using Regen.Flee.PublicTypes;
using Regen.Parser;

namespace Regen.Helpers {
    public static class FleeExtensions {
        public static MethodInfo[] Exclusions = typeof(object).GetMethods();

        public static void AddInstance(this ExpressionImports imports, object target, string @namespace, Func<MethodInfo, bool> selector = null, BindingFlags? flags = null) {
            if (target == null)
                return;

            imports.AddType(InstanceToStaticWrapper.Wrap(target), @namespace);
        }

        public static object UnpackReference(this VariableCollection vars, Data reference) {
            return _unpack(vars, reference);
        }

        public static object UnpackReference(this Data reference, VariableCollection vars) {
            return _unpack(vars, reference);
        }

        public static object UnpackReference(this ExpressionContext ctx, Data reference) {
            return _unpack(ctx.Variables, reference);
        }

        public static object UnpackReference(this Data reference, ExpressionContext ctx) {
            return _unpack(ctx.Variables, reference);
        }

        private static object _unpack(VariableCollection vars, object reference) {
            object ret = reference;
            while (ret is ReferenceData e)
            {
                ret = vars[e.EmitExpressive()];
            }

            return ret;
        }
    }


    public static class InstanceToStaticWrapper {
        private static volatile int _index;

        /// <summary>
        ///     Generates a static wrapper class to an instance class object.
        /// </summary>
        /// <param name="_target">The instance class to wrap</param>
        /// <returns>The type of the static class wrapper</returns>
        /// <remarks>https://gist.github.com/ReubenBond/1bf2b1bf92ab02dc31242462f7bf7958</remarks>
        public static Type Wrap(object _target) {
            var type = _target.GetType();
            int index = Interlocked.Increment(ref _index);
            var ns = Regex.Replace(type.Assembly.FullName, Regexes.SelectNamespaceFromAssemblyName, $"$1.Generated{index}");
            var name = type.FullName + "_StaticRegen" + Guid.NewGuid().ToString("N");
            ModuleBuilder moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.Run).DefineDynamicModule(ns);
            TypeBuilder wrapperBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract, null, new Type[0]);

            var methods = type.GetMethods();
            bool containsSelf = methods.Any(m => m.Name.Equals("self", StringComparison.OrdinalIgnoreCase) && m.ReturnType == type);
            var targetField = CreateWrappedField(_target, wrapperBuilder);

            //Get types except for exclusions
            var approvedTypes = methods.Where(mi => FleeExtensions.Exclusions.All(e => e.Name != mi.Name) && !mi.Name.StartsWith("_")).ToList();

            //if theres not Self() in target, clear any invalid target that will deny compilation.
            if (!containsSelf)
                approvedTypes.RemoveAll(mi => mi.Name.Equals("self", StringComparison.OrdinalIgnoreCase) && mi.ReturnType != type);
            foreach (MethodInfo method in approvedTypes) {
                CreateProxyMethod(wrapperBuilder, method, targetField);
            }

            if (!containsSelf) {
                CreateSelfMethod(wrapperBuilder, targetField);
            }


            //build the output type
            Type wrapperType = wrapperBuilder.CreateType();

            //set the target to the private field
            const string setter = "_setNonStatic";
            wrapperType.GetMethod(setter, BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] {_target});

            return wrapperType;
        }

        private static FieldBuilder CreateWrappedField(object target, TypeBuilder wrapperBuilder) {
            var fld = wrapperBuilder.DefineField("nonStatic", target.GetType(), FieldAttributes.Private | FieldAttributes.Static);
            var initMethod = wrapperBuilder.DefineMethod(
                "_setNonStatic",
                MethodAttributes.Static | MethodAttributes.Private,
                CallingConventions.Standard,
                typeof(void),
                new[] {target.GetType()});
            var il = initMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stsfld, fld);
            il.Emit(OpCodes.Ret);


            return fld;
        }

        private static void CreateProxyMethod(TypeBuilder wrapperBuilder, MethodInfo method, FieldBuilder target) {
            var parameters = method.GetParameters();
            var ret = method.ReturnType == typeof(void) ? typeof(object) : method.ReturnType;
            var methodBuilder = wrapperBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig, ret, parameters.Select(p => p.ParameterType).ToArray());
            var gen = methodBuilder.GetILGenerator();

            gen.Emit(OpCodes.Ldsfld, target);
            for (int i = 0; i < parameters.Length; i++)
                gen.Emit(OpCodes.Ldarg, i);
            gen.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType == typeof(void))
                gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ret);
        }

        private static void CreateSelfMethod(TypeBuilder wrapperBuilder, FieldBuilder target) {
            var ret = target.FieldType;
            var methodBuilder = wrapperBuilder.DefineMethod("Self", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig, ret, Type.EmptyTypes);
            var gen = methodBuilder.GetILGenerator();

            gen.Emit(OpCodes.Ldsfld, target);
            gen.Emit(OpCodes.Ret);
        }
    }

    public static class StaticToInstanceWrapper {
        /// <summary>
        ///     Generates an instance class object to an static class.
        /// </summary>
        /// <param name="type">The type of the static class to wrap</param>
        /// <returns>An instance of the wrapped static class.</returns>
        /// <remarks>https://gist.github.com/ReubenBond/1bf2b1bf92ab02dc31242462f7bf7958</remarks>
        public static object Wrap(Type type) {
            string ns = type.Assembly.FullName;
            var name = type.FullName + "_StaticRegen" + Guid.NewGuid().ToString("N");
            ModuleBuilder moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.Run).DefineDynamicModule(ns);
            TypeBuilder wrapperBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public, null, new Type[0]);
            foreach (MethodInfo method in type.GetMethods().Where(mi => FleeExtensions.Exclusions.All(e => e.Name != mi.Name))) {
                CreateProxyMethod(wrapperBuilder, method);
            }

            Type wrapperType = wrapperBuilder.CreateType();
            object instance = Activator.CreateInstance(wrapperType);
            return instance;
        }

        private static void CreateProxyMethod(TypeBuilder wrapperBuilder, MethodInfo method) {
            var parameters = method.GetParameters();

            var methodBuilder = wrapperBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
            var gen = methodBuilder.GetILGenerator();

            for (int i = 1; i < parameters.Length + 1; i++) {
                gen.Emit(OpCodes.Ldarg, i);
            }

            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);
        }
    }
}