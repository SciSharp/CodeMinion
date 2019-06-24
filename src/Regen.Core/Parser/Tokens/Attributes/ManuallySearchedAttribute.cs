using System;

namespace Regen.Parser {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class ManuallySearchedAttribute : Attribute {
        public ManuallySearchedAttribute() { }
    }
}