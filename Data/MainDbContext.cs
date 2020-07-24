using Vogel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Vogel.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        public DbSet<Oreo> Oreo { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Rating> Rating { get; set; }

        public Image GetImage(in Oreo oreo) => Image.Find(oreo.Image) ?? Image.Find(-1);

        public string GetImageSource(in Oreo oreo) => GetImage(oreo).GetEffectiveSource();

        public float GetRating(Oreo oreo)
        {
            var Rating = this.Rating.Find(oreo.Rating);
            if (Rating != null)
            {
                if (Rating.Total < 1) return 0;
                else return (float)Decimal.Round(Rating.Score / (decimal)Rating.Total, 2);
            }
            else
            {
                return 0;
            }
        }
    }
}
