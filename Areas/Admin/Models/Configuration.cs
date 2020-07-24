using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Areas.Admin.Models
{
    public class Configuration
    {
        public int Id { get; set; }
        public byte[] RSA_pub { get; set; }
        public byte[] RSA_pri { get; set; }
    }
}
