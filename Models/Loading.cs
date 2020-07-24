using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Models
{
    public class Loading
    {
        private static int id = 0;

        public Loading()
        {
            Id = id += 1;

            DefaultActive = false;

            Display = "grid";
            CSSClasses = "";
            SuccessText = "";
            ErrorText = "";
        }

        public int Id { get; }
        public string Display { get; set; }
        public bool DefaultActive { get; set; }
        public string CSSClasses { get; set; }
        public string SuccessText { get; set; }
        public string ErrorText { get; set; }

        public string GetDisplayStyle()
        {
            return DefaultActive ? Display : "none";
        }
    }
}
