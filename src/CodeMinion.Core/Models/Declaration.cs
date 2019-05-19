using System.Collections.Generic;
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

        public List<Argument> Returns { get; set; } = new List<Argument>();
        public bool IsDeprecated { get; set; }
        public bool ManualOverride { get; set; }
        public bool CommentOut { get; set; }

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

    }

    public class Function : Declaration
    {
        public List<Argument> Arguments { get; set; } = new List<Argument>();
        public string[] Generics { get; set; } = null;

    }

    public class Property : Declaration
    {        
        public bool HasSetter { get; set; }

    }
}
