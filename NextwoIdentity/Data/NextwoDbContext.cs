using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextwoIdentity.Models;
using System.Reflection.Metadata;

namespace NextwoIdentity.Data
{
    public class NextwoDbContext:IdentityDbContext
    {
        public NextwoDbContext(DbContextOptions<NextwoDbContext> options):base(options) {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
