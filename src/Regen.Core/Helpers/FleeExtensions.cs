using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Flee.PublicTypes;

namespace Regen.Helpers {
    public static class FleeExtensions {
        public static MethodInfo[] Exclusions = typeof(object).GetMethods();

        public static void AddInstance(this ExpressionImports imports, object target, string @namespace, Func<MethodInfo, bool> selector = null, BindingFlags? flags = null) {
            if (target == null)
                return;
            
            imports.AddType(InstanceToStaticWrapper.Wrap(target), @namespace);
        }
    }


    public static class MyStatic {
        private static MyNonStatic NonStatic;

        public static int Add(int a, int b) {
            return NonStatic.Add(a, b);
        }
    }

    public class MyNonStatic {
        public int Add(int a, int b) {
            return a + b;
        }
    }

    public static class InstanceToStaticWrapper {
        /// <summary>
        ///     Generates a static wrapper class to an instance class object.
        /// </summary>
        /// <param name="_target">The instance class to wrap</param>
        /// <returns>The type of the static class wrapper</returns>
        /// <remarks>https://gist.github.com/ReubenBond/1bf2b1bf92ab02dc31242462f7bf7958</remarks>
        public static Type Wrap(object _target) {
            var type = _target.GetType();
            string ns = type.Assembly.FullName;
            var name = type.FullName + "_StaticRegen" + Guid.NewGuid().ToString("N");
            ModuleBuilder moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.Run).DefineDynamicModule(ns);
            TypeBuilder wrapperBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract, null, new Type[0]);
            const string setter = "_setNonStatic";
            var fld = wrapperBuilder.DefineField("nonStatic", type, FieldAttributes.Private | FieldAttributes.Static);

            var initMethod = wrapperBuilder.DefineMethod(
                "_setNonStatic",
                MethodAttributes.Static | MethodAttributes.Private,
                CallingConventions.Standard,
                typeof(void),
                new[] {type});
            var il = initMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stsfld, fld);
            il.Emit(OpCodes.Ret);

            foreach (MethodInfo method in type.GetMethods().Where(mi => FleeExtensions.Exclusions.All(e => e.Name != mi.Name) && !mi.Name.StartsWith("_"))) {
                CreateProxyMethod(wrapperBuilder, method, fld);
            }

            Type wrapperType = wrapperBuilder.CreateType();

            wrapperType.GetMethod(setter, BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] {_target});

            return wrapperType;
        }

        private static void CreateProxyMethod(TypeBuilder wrapperBuilder, MethodInfo method, FieldBuilder fld) {
            var parameters = method.GetParameters();
            var ret = method.ReturnType == typeof(void) ? typeof(object) : method.ReturnType;
            var methodBuilder = wrapperBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig, ret, parameters.Select(p => p.ParameterType).ToArray());
            var gen = methodBuilder.GetILGenerator();

            gen.Emit(OpCodes.Ldsfld, fld);
            for (int i = 0; i < parameters.Length; i++)
                gen.Emit(OpCodes.Ldarg, i);
            gen.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType == typeof(void))
                gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ret);
        }
    }

    public class yoho {
        public object kek() {
            var t = 1 + 1;
            return null;
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