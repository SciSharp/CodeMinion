using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Regen {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class ManuallySearchedAttribute : Attribute {
        public ManuallySearchedAttribute() { }
    }
}