using Microsoft.EntityFrameworkCore;
using Nomad.Models;

namespace Nomad.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }
    }
}
