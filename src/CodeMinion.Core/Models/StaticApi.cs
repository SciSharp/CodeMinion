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
        public string ImplName { get; set; } = "PyTorch";

        /// <summary>
        /// API declarations
        /// </summary>
        public List<Declaration> Declarations { get; set; } = new List<Declaration>();

        /// <summary>
        /// The python module this API represents
        /// </summary>
        public string PythonModule { get; set; } = "torch";

        /// <summary>
        /// Target directory for the generated files
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Additional name of a partial API file (required for splitting the API into multiple partial class files)
        /// </summary>
        public string PartialName { get; set; }
    }
}
