using System;
using System.Reflection;

namespace Regen.Helpers {
    public class ManualAssemblyResolver : IDisposable {
        public ManualAssemblyResolver(Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            _assemblies = new[] {assembly};
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public ManualAssemblyResolver(params Assembly[] assemblies) {
            if (assemblies == null)
                throw new ArgumentNullException("assemblies");

            if (assemblies.Length == 0)
                throw new ArgumentException("Assemblies should be not empty.", "assemblies");

            _assemblies = assemblies;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public void Dispose() {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
            foreach (Assembly assembly in _assemblies) {
                if (args.Name == assembly.FullName) {
                    return assembly;
                }
            }

            return null;
        }

        private readonly Assembly[] _assemblies;
    }
}