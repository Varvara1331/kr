using Microsoft.EntityFrameworkCore;
using demo.Models;

namespace demo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TemporaryLink> TemporaryLinks { get; set; }
        public DbSet<VpnConfig> VpnConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemporaryLink>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<TemporaryLink>()
                .HasIndex(t => t.EmployeeEmail)
                .IsUnique();

            modelBuilder.Entity<TemporaryLink>()
                .HasIndex(t => t.Password)
                .IsUnique();

            modelBuilder.Entity<VpnConfig>()
               .HasIndex(v => v.Name)
               .IsUnique();

            modelBuilder.Entity<VpnConfig>()
                .HasIndex(v => v.IsDefault)
                .HasFilter("IsDefault = 1");
        }
    }
}