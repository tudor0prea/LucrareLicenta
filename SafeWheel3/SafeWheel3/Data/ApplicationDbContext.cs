using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeWheel3.Models;

namespace SafeWheel3.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Anunt> Anunturi { get; set; }
        public DbSet<Marca> Marci { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<AnuntBookmark> AnuntBookmarks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<AnuntBookmark>()
            .HasKey(ab => new { ab.Id, ab.AnuntId, ab.BookmarkId });
            // definire relatii cu modelele Bookmark si Anunt(FK)
            modelBuilder.Entity<AnuntBookmark>()
            .HasOne(ab => ab.Anunt)
            .WithMany(ab => ab.AnuntBookmarks)
            .HasForeignKey(ab => ab.AnuntId);
            modelBuilder.Entity<AnuntBookmark>()
            .HasOne(ab => ab.Bookmark)
            .WithMany(ab => ab.AnuntBookmarks)
            .HasForeignKey(ab => ab.BookmarkId);
        }

    
    }
}


