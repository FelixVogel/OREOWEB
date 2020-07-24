using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Base
{
    public class BaseResource
    {
        private readonly string prefix;
        private readonly string suffix;

        public BaseResource(string prefix, string suffix)
        {
            this.prefix = prefix;
            this.suffix = suffix;
        }

        public string[] GenerateResourceList(ref string[] names, in string[] defaults)
        {
            string[] nitems;

            if (defaults != null)
            {
                nitems = new string[defaults.Length + names.Length];

                for (int i = 0, l = defaults.Length; i < l; i++)
                {
                    nitems[i] = prefix + defaults[i] + suffix;
                }

                for (int i = 0, l = names.Length, dl = defaults.Length; i < l; i++)
                {
                    nitems[i + dl] = prefix + names[i] + suffix;
                }
            }
            else
            {
                nitems = new string[names.Length];

                for (int i = 0, l = names.Length; i < l; i++)
                {
                    nitems[i] = prefix + names[i] + suffix;
                }
            }

            return nitems;
        }
    }
}
