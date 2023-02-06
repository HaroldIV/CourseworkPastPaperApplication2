using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseworkPastPaperApplication2.Shared;

public partial class PapersDbContext : DbContext
{
    public PapersDbContext()
    {
    }

    public PapersDbContext(DbContextOptions<PapersDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<PaperResult> PaperResults { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql("Host=localhost;Database=CourseworkDatabase;Username=postgres;Password=oneplustwoequalsthreeoneplustwoequalsthree;Include Error Detail=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasIndex(e => e.StudentPassword, "IX_Assignments_StudentPassword");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.StudentPasswordNavigation).WithMany(p => p.Assignments).HasForeignKey(d => d.StudentPassword);
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasIndex(e => e.TeacherPassword, "IX_Classes_TeacherPassword");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.TeacherNavigation).WithMany(p => p.Classes).HasForeignKey(d => d.TeacherPassword);

            entity.HasMany(d => d.Students).WithMany(p => p.CurrentClasses)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentInClass",
                    r => r.HasOne<Student>().WithMany().HasForeignKey("StudentsPassword"),
                    l => l.HasOne<Class>().WithMany().HasForeignKey("CurrentClassesId"),
                    j =>
                    {
                        j.HasKey("CurrentClassesId", "StudentsPassword");
                        j.ToTable("StudentInClass");
                        j.HasIndex(new[] { "StudentsPassword" }, "IX_StudentInClass_StudentsPassword");
                    });
        });

        modelBuilder.Entity<PaperResult>(entity =>
        {
            entity.HasIndex(e => e.AssignmentId, "IX_PaperResults_AssignmentId");

            entity.HasIndex(e => e.StudentPassword, "IX_PaperResults_StudentPassword");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Assignment).WithMany(p => p.PaperResults).HasForeignKey(d => d.AssignmentId);

            entity.HasOne(d => d.StudentPasswordNavigation).WithMany(p => p.PaperResults).HasForeignKey(d => d.StudentPassword);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasIndex(e => e.AssignmentId, "IX_Questions_AssignmentId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Assignment).WithMany(p => p.Questions).HasForeignKey(d => d.AssignmentId);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Password);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Password);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
