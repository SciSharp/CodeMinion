﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Function argument for the method
    /// </summary>
    public class PyFuncArg
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [have default].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [have default]; otherwise, <c>false</c>.
        /// </value>
        public bool HaveDefault { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public dynamic DefaultValue { get; set; }
    }
}