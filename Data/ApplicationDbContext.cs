using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MediaWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace MediaWebApp.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext(options)
    {
        public DbSet<Joke> Joke { get; set; } = default!;
        public DbSet<MediaEntry> MediaEntries { get; set; } = default!;
        public DbSet<MediaList> MediaLists { get; set; } = default!;
        public DbSet<UserProfile> UserProfiles { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            if (Database.IsNpgsql())
            {
                builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
                builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
                builder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
                builder.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(128);
            }
            builder.Entity<MediaEntry>().HasIndex(e => e.OwnerId);
            builder.Entity<MediaEntry>().HasOne(e => e.List).WithMany(l => l.Entries)
                .HasForeignKey(e => e.ListId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<MediaEntry>().HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany().HasForeignKey(e => e.OwnerId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<MediaList>().HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany().HasForeignKey(l => l.OwnerId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<MediaList>().HasIndex(l => l.OwnerId);
            builder.Entity<MediaList>().ToTable("MediaCollections");
            builder.Entity<UserProfile>().HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithOne().HasForeignKey<UserProfile>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
