using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Data;

public class ContactDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>()
            .Property(c => c.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Contact>()
            .Property(c => c.LastName)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Contact>()
            .Property(c => c.EmailAddress)
            .IsRequired();

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.EmailAddress)
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .Property(c => c.TelephoneNumber)
            .IsRequired();
    }
}
