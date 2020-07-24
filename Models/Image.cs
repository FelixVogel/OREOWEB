using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Base64 { get; set; }

        /// <summary>
        /// Get the effective source of this image
        /// </summary>
        /// <returns>The effective source, future to be base64 only</returns>
        public string GetEffectiveSource() => Base64 ?? "/images/" + Url;
    }
}
