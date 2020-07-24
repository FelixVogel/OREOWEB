using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Vogel.Areas.Admin.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public long Lmd { get; set; }

        // -- 

        private static readonly string Charset = "abcdefghijklmnopqrstuvwxzy0123456789ABCDEFGHIJKLMNOPQRSTUVWXZY";

        public static string GenerateToken()
        {
            string token = "tkn_";

            Random rng = new Random(DateTime.UtcNow.Second);

            for (int i = 4; i < 32; i++)
            {
                token += Charset[rng.Next(Charset.Length)];
            }

            return token;
        }

        public static long CurrentTimestamp()
        {
            return DateTime.UtcNow.Ticks / 10_000;
        }
    }
}
