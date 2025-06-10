using Microsoft.EntityFrameworkCore;
using PersonManagement.Domain.Entities;

namespace PersonManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Person entity
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.PersonId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Age).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Add index for better query performance
                entity.HasIndex(e => new { e.FirstName, e.LastName });
            });

            // Seed initial data
            modelBuilder.Entity<Person>().HasData(
                new Person
                {
                    PersonId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 30,
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new Person
                {
                    PersonId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Age = 25,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );
        }
    }
}