//using CourseworkPastPaperApplication.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using CourseworkPastPaperApplication2.Shared;

namespace CourseworkPastPaperApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<PapersDbContext>(optionsAction: options => options.UseNpgsql(@"Host=localhost;Database=TestDB;Username=postgres;Password=oneplustwoequalsthreeoneplustwoequalsthree;Include Error Detail=true"));
            //builder.Services.AddScoped<UserValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.MapGet("/Hello", GetTeachers);

            app.MapGet("/FakeData", ([FromServices] PapersDbContext db) => 
            {
                Console.WriteLine("Made it!");

                var classes = new List<Class>()
                {
                    new Class { Id = Guid.NewGuid(), Students = null, Teachers = null }
                };

                List<Teacher> teachers = new()
                {
                    new Teacher { Name = Faker.Name.FullName(), Password = Faker.RandomNumber.Next(), Classes = classes },
                    new Teacher { Name = Faker.Name.FullName(), Password = Faker.RandomNumber.Next(), Classes = classes },
                    new Teacher { Name = Faker.Name.FullName(), Password = Faker.RandomNumber.Next(), Classes = classes },
                    new Teacher { Name = Faker.Name.FullName(), Password = Faker.RandomNumber.Next(), Classes = classes }
                };

                List<Student> students = new()
                {
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() },
                    new Student { Name = Faker.Name.FullName(), CurrentClass = classes[0], ClassId = classes[0].Id, Assignments = new List<Assignment>(), PaperResults = new List<PaperResult>(), Password = Faker.RandomNumber.Next() }

                };

                classes[0].Teachers = teachers;
                classes[0].Students = students;

                db.Classes.AddRange(classes);
                db.Students.AddRange(students);
                db.Teachers.AddRange(teachers);

                db.SaveChanges();
                return db.Teachers;
            });

            //app.MapGet("/teachers", ([FromServices] DataContext db) => db.Teachers).WithName("Teachers");
            //app.MapGet("/students", ([FromServices] DataContext db) => db.Students);
            //app.MapGet("/teachers/{username}", ([FromServices] DataContext db, string username) => from teacher in db.Teachers
            //                                                                                       where teacher.Username == username
            //                                                                                       select teacher);

            //app.MapPost("/teachers", async ([FromServices] UserValidator validator, [FromServices] DataContext db, [FromBody] Teacher teacher) =>
            //{
            //    FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(teacher);

            //    if (!validationResult.IsValid)
            //    {
            //        return Results.BadRequest(validationResult);
            //    }

            //    db.Teachers.Add(teacher);

            //    await db.SaveChangesAsync();

            //    return Results.Created($"/teachers/{teacher.Password}", teacher);
            //}).WithName("PostTeacher");

            app.Run();
        }

        private static IEnumerable<Teacher> GetTeachers([FromServices] PapersDbContext db)
        {
            var a = db.Teachers;

            return a;
        }
    }
    
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

        public PapersDbContext() : base(new DbContextOptionsBuilder().UseNpgsql(@"Host=localhost;Database=TestDB;Username=postgres;Password=oneplustwoequalsthreeoneplustwoequalsthree;Include Error Detail=true").Options)
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