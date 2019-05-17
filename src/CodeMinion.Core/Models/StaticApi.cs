using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Represents a static API class that should be generated
    /// </summary>
    public class StaticApi
    {
        /// <summary>
        /// Static name is the name of a static class that forwards to a singleton instance of the API implementation
        /// </summary>
        public string StaticName { get; set; } = "torch";

        /// <summary>
        /// The static class forwards to this Singleton instance which is the API implementation 
        /// </summary>
        public string SingletonName { get; set; } = "PyTorch";

        public List<Declaration> Declarations { get; set; } = new List<Declaration>();
        public string PythonModule { get; set; } = "torch";
    }
}
