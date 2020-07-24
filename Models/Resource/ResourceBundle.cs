using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Models.Resource
{
    /// <summary>
    /// This class represents a resource boundle that is present in the rendered html
    /// </summary>
    public class ResourceBundle
    {
        /// <summary>
        /// Gets all resources currently held in this bundle
        /// </summary>
        public string[] Resources { get; set; }
    }
}
