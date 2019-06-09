using System;
using System.Collections.Generic;
using System.Text;
using CodeMinion.Core.Helpers;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Represents the methods of a non-static class which should be generated. 
    /// </summary>
    public class DynamicApi : Api
    {
        /// <summary>
        /// Class name is the name of a non-static class, i.e. NDArray
        /// </summary>
        public string ClassName { get; set; }
        

    }
}
