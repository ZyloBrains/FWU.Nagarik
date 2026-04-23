using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{

    public DbSet<Student> Students { get; set; }
    public DbSet<StudentRequest> StudentRequests { get; set; }
    public DbSet<VerificationLog> VerificationLogs { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<SyncRecord> SyncRecords { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Transcript> Transcripts { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
        });

        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        modelBuilder.Entity<VerificationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
            entity.HasIndex(e => e.RegdNo);
            entity.HasIndex(e => e.VerifiedAt);
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Key).IsRequired();
            entity.HasIndex(e => e.Key).IsUnique();
        });

        modelBuilder.Entity<SyncRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityName).IsRequired();
            entity.HasIndex(e => e.EntityName).IsUnique();
        });

        modelBuilder.Entity<Institution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
            entity.HasIndex(e => e.RegdNo).IsUnique();
        });

        modelBuilder.Entity<Transcript>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
            entity.HasIndex(e => e.RegdNo);
            entity.HasIndex(e => e.IssueSerialNo).IsUnique();
            entity.HasOne(e => e.Student)
                  .WithMany()
                  .HasForeignKey(e => e.RegdNo)
                  .HasPrincipalKey(s => s.RegdNo);
            entity.HasOne(e => e.Institution)
                  .WithMany()
                  .HasForeignKey(e => e.InstitutionId);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Transcript)
                  .WithMany(t => t.Semesters)
                  .HasForeignKey(e => e.TranscriptId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Semester)
                  .WithMany(s => s.Subjects)
                  .HasForeignKey(e => e.SemesterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}