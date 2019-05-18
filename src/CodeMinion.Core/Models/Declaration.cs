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

        public List<Argument> Arguments { get; set; }= new List<Argument>();
        public List<Argument> Returns { get; set; } = new List<Argument>();
        public bool IsDeprecated { get; set; }
        public bool ManualOverride { get; set; }
        public string[] Generics { get; set; } = null;

        public Declaration Clone()
        {
            return JObject.FromObject(this).ToObject<Declaration>();
        }
    }
}
