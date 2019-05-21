using System;
using System.Collections.Generic;
using System.Text;
using CodeMinion.Core.Helpers;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Represents the methods of a non-static class which should be generated. 
    /// </summary>
    public class DynamicApi
    {
        /// <summary>
        /// Class name is the name of a non-static class, i.e. NDArray
        /// </summary>
        public string ClassName { get; set; }
        
        /// <summary>
        /// API declarations
        /// </summary>
        public List<Declaration> Declarations { get; set; } = new List<Declaration>();

        /// <summary>
        /// Target directory for the generated files
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Additional name of a partial API file (required for splitting the API into multiple partial class files)
        /// </summary>
        public string PartialName { get; set; }

        ///// <summary>
        ///// These are generated into the constructor of the API implementation object
        ///// </summary>
        //public List<Action<CodeWriter>> InitializationGenerators { get; set; } = new List<Action<CodeWriter>>();

    }
}
