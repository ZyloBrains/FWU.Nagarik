using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{

    public DbSet<Student> Students { get; set; }
    public DbSet<StudentMark> StudentMarks { get; set; }
    public DbSet<VerificationLog> VerificationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VerificationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
            entity.HasIndex(e => e.RegdNo);
            entity.HasIndex(e => e.VerifiedAt);
        });

        modelBuilder.Entity<StudentMark>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
            entity.HasIndex(e => e.RegdNo);
        });
    }
}