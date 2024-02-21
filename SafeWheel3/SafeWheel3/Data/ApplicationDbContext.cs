using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeWheel3.Models;

namespace SafeWheel3.Data
{
    public class ApplicationDbContext : IdentityDbContext <ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Anunt> Anunturi { get; set; }
        public DbSet<Marca> Marci { get; set; }
    }
}


