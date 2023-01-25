using Microsoft.EntityFrameworkCore;

namespace CourseworkPastPaperApplication2.Shared
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public DateTime Set { get; set; }
        public DateTime Due { get; set; }

        public ICollection<Question> Questions { get; set; }
    }

    public class Teacher
    {
        public string Name { get; set; }
        public long Password { get; set; }

        public ICollection<Class> Classes { get; set; }
    }

    public class Student
    {
        public long Password { get; set; }
        public string Name { get; set; }

        public Class CurrentClass { get; set; }
        public Guid ClassId { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<PaperResult> PaperResults { get; set; }
    }

    public class Class
    {
        public Guid Id { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
    }

    public class QuestionsInPastPaper
    {

    }

    public class Question
    {
        public Guid Id { get; set; }
        public byte[] Data { get; set; }
        public string ReadData { get; set; }
    }

    public class PaperResult
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public Assignment Assignment { get; set; }
    }

    public class PapersDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public Microsoft.EntityFrameworkCore.DbSet<Student> Students { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Teacher> Teachers { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Assignment> Assignments { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Question> Questions { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Class> Classes { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<PaperResult> PaperResults { get; set; }

        public PapersDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public PapersDbContext() : base(new DbContextOptionsBuilder().UseNpgsql("").Options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            #region Student
            modelBuilder.Entity<Student>()
                .HasKey(student => student.Password);

            modelBuilder.Entity<Student>()
                .Property(student => student.Name)
                .IsRequired();
            
            modelBuilder.Entity<Student>()
                .HasOne(student => student.CurrentClass)
                .WithMany(@class => @class.Students)
                .HasForeignKey(student => student.ClassId);

            modelBuilder.Entity<Student>()
                .HasMany(student => student.PaperResults);

            modelBuilder.Entity<Student>()
                .ToTable("Students");
            #endregion

            #region Teacher
            modelBuilder.Entity<Teacher>()
                .HasKey(teacher => teacher.Password);

            modelBuilder.Entity<Teacher>()
                .Property(teacher => teacher.Name)
                .IsRequired();

            modelBuilder.Entity<Teacher>()
                .HasMany(teacher => teacher.Classes)
                .WithMany(@class => @class.Teachers);

            modelBuilder.Entity<Teacher>()
                .ToTable("Teachers");
            #endregion

            #region Assignment
            modelBuilder.Entity<Assignment>()
                .HasKey(assignment => assignment.Id);

            modelBuilder.Entity<Assignment>()
                .Property(assignment => assignment.Set)
                .IsRequired();

            modelBuilder.Entity<Assignment>()
                .Property(assignment => assignment.Due)
                .IsRequired();

            modelBuilder.Entity<Assignment>()
                .HasMany(assignment => assignment.Questions);
            #endregion

            #region Class
            modelBuilder.Entity<Class>()
                .HasKey(@class => @class.Id);

            modelBuilder.Entity<Class>()
                .HasMany(@class => @class.Students);
            #endregion

            #region PaperResult
            modelBuilder.Entity<PaperResult>()
                .HasKey(paperResult => paperResult.Id);

            modelBuilder.Entity<PaperResult>()
                .Property(paperResult => paperResult.Score)
                .IsRequired();

            modelBuilder.Entity<PaperResult>()
                .HasOne(paperResult => paperResult.Assignment);
            #endregion

            #region Question
            modelBuilder.Entity<Question>()
                .HasKey(question => question.Id);

            modelBuilder.Entity<Question>()
                .Property(question => question.Data)
                .IsRequired();

            modelBuilder.Entity<Question>()
                .Property(question => question.ReadData)
                .IsRequired();
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}