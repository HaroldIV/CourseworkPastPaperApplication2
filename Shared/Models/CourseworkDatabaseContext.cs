using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace CourseworkPastPaperApplication2.Shared;

public class PapersDbContext : DbContext
{
    public PapersDbContext()
    {
    }

    public PapersDbContext(DbContextOptions<PapersDbContext> options)
        : base(options)
    {
    }

    [DbFunction("utf8_to_string")]
    public static string SQLUtf8ToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    [DbFunction("soundex", IsBuiltIn = true)]
    public static string Soundex(string argument) => throw new InvalidOperationException(nameof(Soundex));

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<PaperResult> PaperResults { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<ExamBoard> ExamBoards { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql("Host=localhost;Database=CourseworkDatabase;Username=postgres;Password=oneplustwoequalsthreeoneplustwoequalsthree;Include Error Detail=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.HasPostgresExtension("fuzzystrmatch");

        modelBuilder.Entity<Student>(student =>
        {
            student.HasIndex(st => st.Id);
            student.HasKey(st => st.Id);

            student.Property(st => st.Name);
            student.Property(st => st.Password);

            student.Ignore(st => st.PasswordAsHex);

            student.HasMany(st => st.CurrentClasses).WithMany(@class => @class.Students);
            student.HasMany(st => st.Assignments).WithOne(a => a.Student).HasForeignKey(a => a.StudentId);
            student.HasMany(st => st.PaperResults).WithOne(p => p.Student).HasForeignKey(p => p.StudentId);
        });

        modelBuilder.Entity<Teacher>(teacher =>
        {
            teacher.HasIndex(teacher => teacher.Id);
            teacher.HasKey(teacher => teacher.Id);

            teacher.Property(teacher => teacher.Name);
            teacher.Property(teacher => teacher.Password);

            teacher.Ignore(st => st.PasswordAsHex);

            teacher.HasMany(teacher => teacher.Classes).WithOne(@class => @class.TeacherNavigation);
        });

        modelBuilder.Entity<Class>(@class =>
        {
            @class.HasIndex(cl => cl.Id);
            @class.HasKey(cl => cl.Id);

            @class.HasOne(cl => cl.TeacherNavigation).WithMany(teacher => teacher.Classes).HasForeignKey(cl => cl.TeacherId);
            @class.HasMany(cl => cl.Students).WithMany(st => st.CurrentClasses);
        });

        modelBuilder.Entity<Assignment>(assignment =>
        {
            assignment.HasIndex(a => a.Id);
            assignment.HasKey(a => a.Id);

            assignment.Property(a => a.Due);
            assignment.Property(a => a.Set);

            assignment.HasMany(a => a.PaperResults).WithOne(p => p.Assignment).HasForeignKey(p => p.AssignmentId);
            assignment.HasMany(a => a.Questions).WithOne(q => q.Assignment).HasForeignKey(q => q.AssignmentId);

            assignment.HasOne(a => a.Student).WithMany(st => st.Assignments).HasForeignKey(a => a.StudentId);
        });

        modelBuilder.Entity<PaperResult>(paperResult =>
        {
            paperResult.HasIndex(p => p.Id);
            paperResult.HasKey(p => p.Id);

            paperResult.Property(p => p.Score);

            paperResult.HasOne(p => p.Student).WithMany(st => st.PaperResults).HasForeignKey(p => p.StudentId);
            paperResult.HasOne(p => p.Assignment).WithMany(a => a.PaperResults).HasForeignKey(a => a.AssignmentId);
        });

        modelBuilder.Entity<Question>(question =>
        {
            question.HasIndex(q => q.Id);
            question.HasKey(q => q.Id);

            question.Property(q => q.Data);
            question.Property(q => q.ReadData);

            question.HasOne(q => q.Level).WithMany().IsRequired(false);
            question.HasOne(q => q.ExamBoard).WithMany().IsRequired(false);

            question.HasOne(q => q.Assignment).WithMany(a => a.Questions).HasForeignKey(q => q.AssignmentId);
        });

        modelBuilder.Entity<ExamBoard>(SetUpFilterOptions);
        modelBuilder.Entity<Level>(SetUpFilterOptions);
    }

    private static void SetUpFilterOptions<T>(EntityTypeBuilder<T> filterOption) where T : class, IFilterOption
    {
        filterOption.HasIndex(board => board.Id);
        filterOption.HasKey(board => board.Id);

        filterOption.Property(board => board.Name);
    }
}
