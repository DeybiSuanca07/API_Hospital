using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.Priority).HasConversion<string>().HasMaxLength(20);
                entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            });
        }
    }
}