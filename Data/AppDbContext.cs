using IncidentServiceAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IncidentServiceAPI.Data
{
    /// <summary>
    /// EF Core database context.
    /// Defines entity sets and configures schema constraints to enforce data integrity.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<AccountContact> AccountContacts { get; set; }

        /// <summary>
        /// Fluent configuration for keys, indexes, constraints, and relationships.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Name);

                entity.Property(a => a.Name)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(a => a.Name)
                      .IsUnique();
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Email);

                entity.Property(c => c.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(c => c.Email)
                      .IsUnique();

                entity.Property(c => c.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.LastName)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            modelBuilder.Entity<Incident>(entity =>
            {
                entity.HasKey(i => i.IncidentName);

                entity.Property(i => i.IncidentName)
                      .IsRequired()
                      .HasMaxLength(255)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("lower(replace(newid(), '-', ''))");

                entity.Property(i => i.Description)
                      .IsRequired();

                entity.HasOne(i => i.Account)
                      .WithMany(a => a.Incidents)
                      .HasForeignKey(i => i.AccountName)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AccountContact>(entity =>
            {
                entity.HasKey(ac => new { ac.AccountName, ac.ContactEmail });

                entity.HasOne(ac => ac.Account)
                      .WithMany(a => a.AccountContacts)
                      .HasForeignKey(ac => ac.AccountName)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ac => ac.Contact)
                      .WithMany(c => c.AccountContacts)
                      .HasForeignKey(ac => ac.ContactEmail)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}