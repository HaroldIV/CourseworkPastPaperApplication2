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

// This class is a facade over the database and provides functionality for querying, modifying, etc. 
// This is largely provider independent to allow for swapping to other SQL (or even No-SQL) providers with minimal changes required. 
public class PapersDbContext : DbContext
{
    public PapersDbContext()
    {
    }

    // Initialises with database initialisation options. 
    public PapersDbContext(DbContextOptions<PapersDbContext> options)
        : base(options)
    {
    }

    // Links to a custom SQL function that converts UTF8 bytes to a string. 
    [DbFunction("utf8_to_string")]
    public static string SQLUtf8ToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    // Links to a builtin SQL function that retrieves the Soundex (phonic representation) of a string. 
    // Does not return anything in the C# code since this should only be used in SQL queries. 
    [DbFunction("soundex", IsBuiltIn = true)]
    public static string Soundex(string argument) => throw new InvalidOperationException(nameof(Soundex));

    // Each DbSet<{T}> represents the table, {T}
    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<PaperResult> PaperResults { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<ExamBoard> ExamBoards { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql("Host=localhost;Database=CourseworkDatabase;Username=postgres;Password=oneplustwoequalsthreeoneplustwoequalsthree;Include Error Detail=true");

    // Builds the facades internal representation of the database to allow it to correctly transform queries to the database. 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Specifies the schema of the database. 
        modelBuilder.HasDefaultSchema("public");

        // Adds the fuzzystrmatch extension to the database which contains the Soundex function. 
        modelBuilder.HasPostgresExtension("fuzzystrmatch");

        // Configures the Student entity. 
        modelBuilder.Entity<Student>(student =>
        {
            // Indicates that Id is used as both a key and an index. 
            student.HasIndex(st => st.Id);
            student.HasKey(st => st.Id);

            // Lists the properties of Student. 
            student.Property(st => st.Name);
            student.Property(st => st.Password);

            // Indicates to ignore the PasswordAsHex property. 
            student.Ignore(st => st.PasswordAsHex);

            // Indicates a many-to-many relation between students and classes. 
            student.HasMany(st => st.CurrentClasses).WithMany(@class => @class.Students);
            // Indicates a one-to-many relation between students and paper results with paper results having a key of StudentId
            student.HasMany(st => st.PaperResults).WithOne(p => p.Student).HasForeignKey(p => p.StudentId);
        });

        // Configures the Teacher entity. 
        modelBuilder.Entity<Teacher>(teacher =>
        {
            // Indicates that Id is used as both a key and an index. 
            teacher.HasIndex(teacher => teacher.Id);
            teacher.HasKey(teacher => teacher.Id);

            // Lists the properties of Teacher. 
            teacher.Property(teacher => teacher.Name);
            teacher.Property(teacher => teacher.Password);

            // Indicates to ignore the PasswordAsHex property. 
            teacher.Ignore(st => st.PasswordAsHex);

            // Indicates a one-to-many relation with classes. 
            teacher.HasMany(teacher => teacher.Classes).WithOne(@class => @class.TeacherNavigation);
        });

        // Configures the Class entity. 
        modelBuilder.Entity<Class>(@class =>
        {
            // Indicates that Id is used as both a key and an index. 
            @class.HasIndex(cl => cl.Id);
            @class.HasKey(cl => cl.Id);

            // Shows the property of Class. 
            @class.Property(cl => cl.Name);

            // Configures Class to have a many-to-one relation with Teacher with a foreign key of TeacherId
            @class.HasOne(cl => cl.TeacherNavigation).WithMany(teacher => teacher.Classes).HasForeignKey(cl => cl.TeacherId);
            // Configures Class to have a many-to-many relation with Student
            @class.HasMany(cl => cl.Students).WithMany(st => st.CurrentClasses);

            // Configures  Class to have a one-to-many relation with Assignment
            @class.HasMany(cl => cl.Assignments).WithOne(a => a.Class);
        });

        // Configures the Assignment entity. 
        modelBuilder.Entity<Assignment>(assignment =>
        {
            // Indicates that Id is used as both a key and an index. 
            assignment.HasIndex(a => a.Id);
            assignment.HasKey(a => a.Id);

            // Lists the properties of Assignment. 
            assignment.Property(a => a.Due);
            assignment.Property(a => a.Set);
            assignment.Property(a => a.Name);

            // Configures Assignment to have a many-to-many relation with Question with the Question class having no navigation collection property. 
            // That decision was based off of the lack of need for such a property. 
            assignment.HasMany(a => a.Questions).WithMany();

            // Configures Assignment to have a many-to-one relation with Class
            assignment.HasOne(a => a.Class).WithMany(cl => cl.Assignments);
        });

        // Configures the PaperResult entity. 
        modelBuilder.Entity<PaperResult>(paperResult =>
        {
            // Indicates that Id is used as both a key and an index. 
            paperResult.HasIndex(p => p.Id);
            paperResult.HasKey(p => p.Id);

            // Shows the property of Class. 
            paperResult.Property(p => p.Score);

            // Indicates that PaperResult has a many-to-one relation with Student with a foreign key of StudentId
            paperResult.HasOne(p => p.Student).WithMany(st => st.PaperResults).HasForeignKey(p => p.StudentId);
            // Indicates that PaperResult has a many-to-one relation with Assignment with a foreign key of AssignmentId
            paperResult.HasOne(p => p.Assignment).WithMany().HasForeignKey(p => p.AssignmentId);
        });

        // Configures the Question entity. 
        modelBuilder.Entity<Question>(question =>
        {
            // Indicates that Id is used as both a key and an index. 
            question.HasIndex(q => q.Id);
            question.HasKey(q => q.Id);

            // Lists the properties of Question. 
            question.Property(q => q.Data);
            question.Property(q => q.ReadData);
            question.Property(q => q.FileName);
            question.Property(q => q.Marks);

            // Configures an optional many-to-one relation with the filter options (separately)
            question.HasOne(q => q.Level).WithMany().IsRequired(false);
            question.HasOne(q => q.ExamBoard).WithMany().IsRequired(false);
        });

        // Configures the ExamBoard and Level entities. 
        modelBuilder.Entity<ExamBoard>(SetUpFilterOptions);
        modelBuilder.Entity<Level>(SetUpFilterOptions);
    }

    // Configures Filter Option entities. 
    private static void SetUpFilterOptions<T>(EntityTypeBuilder<T> filterOption) where T : class, IFilterOption
    {
        // Indicates that Id is used as both a key and an index. 
        filterOption.HasIndex(option => option.Id);
        filterOption.HasKey(option => option.Id);

        // Configures the Name property of the filter options.
        filterOption.Property(option => option.Name);
    }
}