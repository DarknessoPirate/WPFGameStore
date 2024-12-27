using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WPFGameShop.Models
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext()
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Load configuration from appsettings.json
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


                var configuration = builder.Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserGame> UserGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(eb =>
            {
                eb.HasIndex(u => u.Name).IsUnique();
                eb.HasIndex(u => u.Email).IsUnique();


                eb.HasMany(x => x.Games)
                .WithMany(x => x.Users)
                .UsingEntity<UserGame>(

                    w => w.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId),

                    w => w.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId),

                    w => w.Property(x => x.TransactionDate).HasDefaultValueSql("SYSDATETIME()")

                    );
            });

            modelBuilder.Entity<UserGame>(eb =>
            {
                eb.HasKey(x => new { x.UserId, x.GameId });
            });

            modelBuilder.Entity<Review>(eb =>
            {
                eb.HasOne(x => x.User)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.UserId);

                eb.HasOne(x => x.Game)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.GameId);    
            });
        }
    }
}
