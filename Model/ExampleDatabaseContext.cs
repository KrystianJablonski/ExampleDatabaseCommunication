using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Model
{
    /// <summary>
    /// Context class representing connection to the database.
    /// </summary>
    public class ExampleDatabaseContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;

                if (settings != null)
                {
                    ConnectionStringSettings cs = settings["DatabaseConnection"];
                    if(cs != null && !string.IsNullOrEmpty(cs.ConnectionString))
                        optionsBuilder.UseSqlServer(cs.ConnectionString);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}
