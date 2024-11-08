using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVRecruitment.Models;

public partial class CvrecruitmentContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public CvrecruitmentContext()
    {
    }

    public CvrecruitmentContext(DbContextOptions<CvrecruitmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyImage> CompanyImages { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<MySkill> MySkills { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Recruitment> Recruitments { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<WorkExperience> WorkExperiences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__Certific__BBF8A7C1D0296A80");

            entity.Property(e => e.CertificateName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CertificateUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Organization)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__UserI__5629CD9C");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971CAC8BA259A9");

            entity.ToTable("Company");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyCountry)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanySize)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Logo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.WorkingDay)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasMany(d => d.Skills).WithMany(p => p.Companies)
                .UsingEntity<Dictionary<string, object>>(
                    "KeySkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__KeySkills__Skill__59FA5E80"),
                    l => l.HasOne<Company>().WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__KeySkills__Compa__5BE2A6F2"),
                    j =>
                    {
                        j.HasKey("CompanyId", "SkillId").HasName("PK__KeySkill__406D15B471541684");
                        j.ToTable("KeySkills");
                    });
        });

        modelBuilder.Entity<CompanyImage>(entity =>
        {
            entity.HasKey(e => e.CompanyImageId).HasName("PK__CompanyI__D7A1961C5A069327");

            entity.ToTable("CompanyImage");

            entity.Property(e => e.File)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyImages)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CompanyIm__Compa__5CD6CB2B");
        });

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.Cvid).HasName("PK__CV__A04CFFA3A6554FEB");

            entity.ToTable("CV");

            entity.Property(e => e.Cvid).HasColumnName("CVId");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.File)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Template).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__TemplateId__5FB337D6");

            entity.HasOne(d => d.User).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__UserId__5812160E");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PK__Educatio__4BBE3805208D23C4");

            entity.ToTable("Education");

            entity.Property(e => e.AdditionalDetail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FromMonth).HasColumnType("datetime");
            entity.Property(e => e.FromYear).HasColumnType("datetime");
            entity.Property(e => e.Major)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.School)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ToMonth).HasColumnType("datetime");
            entity.Property(e => e.ToYear).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Educations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Education__UserI__52593CB8");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__Job__056690C2C636F866");

            entity.ToTable("Job");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.JobName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PostedDay).HasColumnType("datetime");
            entity.Property(e => e.WorkStyle)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__UserI__5629CDkug");

            entity.HasMany(d => d.Skills).WithMany(p => p.Jobs)
                .UsingEntity<Dictionary<string, object>>(
                    "JobSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__JobSkills__Skill__5AEE82B9"),
                    l => l.HasOne<Job>().WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__JobSkills__JobId__5DCAEF64"),
                    j =>
                    {
                        j.HasKey("JobId", "SkillId").HasName("PK__JobSkill__689C99DA79B69042");
                        j.ToTable("JobSkills");
                    });
        });

        modelBuilder.Entity<MySkill>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SkillId }).HasName("PK__MySkills__7A72C55435ED2F6E");

            entity.Property(e => e.Level)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Skill).WithMany(p => p.MySkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MySkills__SkillI__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.MySkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MySkills__UserId__5441852A");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Project__761ABEF05448DADD");

            entity.ToTable("Project");

            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProjectUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Projects)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Project__UserId__5535A963");
        });

        modelBuilder.Entity<Recruitment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.JobId }).HasName("PK__Recruitm__27DEA540591D55FB");

            entity.ToTable("Recruitment");

            entity.Property(e => e.FileCv)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("FileCV");
            entity.Property(e => e.SentAt).HasColumnType("datetime");

            entity.HasOne(d => d.Job).WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recruitme__JobId__5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recruitme__UserI__571DF1D5");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__Skills__DFA09187C2594975");

            entity.Property(e => e.SkillName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__Template__F87ADD271B1B38EC");

            entity.ToTable("Template");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.File)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {

            entity.ToTable("User");

            //entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.AboutMe)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PersonalLink)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WorkExperience>(entity =>
        {
            entity.HasKey(e => e.Weid).HasName("PK__WorkExpe__FA3100514870CC87");

            entity.ToTable("WorkExperience");

            entity.Property(e => e.Weid).HasColumnName("WEId");
            entity.Property(e => e.Company)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.WorkExperiences)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkExper__UserI__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
