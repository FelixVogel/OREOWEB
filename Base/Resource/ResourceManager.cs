using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Base.Resource
{
    public class ResourceManager
    {
        public static readonly BaseResource CSS = new CSSResource();
        public static readonly BaseResource JS = new JSResource();
    }
}
