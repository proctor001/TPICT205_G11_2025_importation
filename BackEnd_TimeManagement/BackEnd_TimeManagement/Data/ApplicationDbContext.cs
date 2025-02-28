using Microsoft.EntityFrameworkCore;
using BackEnd_TimeManagement.Models;

namespace BackEnd_TimeManagement.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Etudiants> Etudiants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Forcer l'unicité des emails
            modelBuilder.Entity<Etudiants>().HasIndex(e => e.Email).IsUnique();
        }
    }
}