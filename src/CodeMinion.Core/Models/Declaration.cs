using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CodeMinion.Core.Models
{
    public class Declaration
    {
        /// <summary>
        /// Function name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Class name is the name of the containing API class (i.e. "numpy").
        /// If it is a nested class the class name contains all nesting levels (i.e. "numpy.core.records")
        /// </summary>
        public string ClassName { get; set; }
        public string GeneratedClassName { get; set; }

        public List<Argument> Returns { get; set; } = new List<Argument>();
        public bool IsDeprecated { get; set; }
        public bool ManualOverride { get; set; }
        public bool CommentOut { get; set; }
        public string SharpOnlyPostfix { get; set; }

        /// <summary>
        /// Break into the Debugger when generating this declaration
        /// </summary>
        public bool DebuggerBreak { get; set; }

        public string Description { get; set; }

        public virtual Declaration Clone()
        {
            return Clone<Declaration>();
        }

        public virtual T Clone<T>()
        {
            return JObject.FromObject(this).ToObject<T>();
        }

        public string Tag { get; set; }

        public virtual void Sanitize()
        {
            
        }
    }

    public class Function : Declaration
    {
        public List<Argument> Arguments { get; set; } = new List<Argument>();

        /// <summary>
        /// Generic type parameters of the function
        /// </summary>
        public string[] Generics { get; set; } = null;

        /// <summary>
        /// If this is set the member will be forwarded to the given static api object with self as first parameter
        /// </summary>
        public string ForwardToStaticImpl { get; set; }

        public virtual Function Clone(Action<Function> a)
        {
            var clone= Clone<Function>();
            a(clone);
            return clone;
        }

        public void ChangeArg(string name, string Type=null, string DefaultValue = null, bool? IsNullable = null)
        {
            var arg = Arguments.First(a => a.Name == name);
            if (Type != null) arg.Type = Type;
            if (DefaultValue != null) arg.DefaultValue = DefaultValue;
            if (IsNullable != null) arg.IsNullable = IsNullable.Value;
        }

        public override void Sanitize()
        {
            base.Sanitize();
            SanitizeArguments();
        }

        public void SanitizeArguments()
        {
            var all_named = false;
            foreach (var arg in Arguments)
            {
                if (arg.DefaultValue != null || arg.IsNamedArg)
                    all_named = true;
                if (all_named)
                    arg.IsNamedArg = true;
            }
        }

        public Argument this[string name] => Arguments.FirstOrDefault(x => x.Name == name);
    }

    public class Property : Declaration
    {        
        public bool HasSetter { get; set; }

    }
}
