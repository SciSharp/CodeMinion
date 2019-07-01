using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMinion.Core.Models
{
    public class Function : Declaration
    {
        public List<Argument> Arguments { get; set; } = new List<Argument>();

        public bool IsConstructor { get; set; }

        /// <summary>
        /// Generic type parameters of the function
        /// </summary>
        public string[] Generics { get; set; } = null;

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
            foreach (var arg in Arguments.ToArray())
            {
                if (arg.Ignore)
                    Arguments.Remove(arg);
                if (arg.DefaultValue != null || arg.IsNamedArg)
                    all_named = true;
                if (all_named)
                    arg.IsNamedArg = true;
                if (arg.Name == "self")
                    arg.Name = "self_";
            }
            if (Arguments.Count == 1)
            {
                var arg = Arguments[0];
                if (arg.Type != null && arg.Type.EndsWith("[]") && !arg.Type.StartsWith("params"))
                {
                    arg.Type = "params " + arg.Type;
                    arg.IsNullable = false;
                    arg.DefaultValue = null;
                }
            }
        }

        public Argument this[string name] => Arguments.FirstOrDefault(x => x.Name == name);

        public void MakeGeneric(string type_param)
        {
            Generics = new[] {type_param};
        }
    }
}