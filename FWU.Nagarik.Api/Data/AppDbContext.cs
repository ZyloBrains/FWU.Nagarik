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
    public DbSet<Transcript> Transcripts { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

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

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
        });

        modelBuilder.Entity<Transcript>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegdNo).IsRequired();
            entity.HasIndex(e => e.RegdNo);
            entity.HasIndex(e => new { e.RegdNo, e.IssueSerialNo, e.SubjectCode }).IsUnique();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.ClientKeyId, e.Timestamp });
            entity.HasIndex(e => new { e.Action, e.Timestamp });
        });
    }
}
