//using CourseworkPastPaperApplication.Shared;
using CourseworkPastPaperApplication2.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CourseworkPastPaperApplication2
{
    public class Program
    {
#if DEBUG
        private static readonly Bogus.Faker faker = new Bogus.Faker();
#endif

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<PapersDbContext>(optionsAction: options =>
            {
                options.UseNpgsql(@"Host=localhost;Database=CourseworkDatabase;Username=postgres;Password=oneplustwoequalsthreeoneplustwoequalsthree;Include Error Detail=true");
#if DEBUG
                options.EnableSensitiveDataLogging()
                       .EnableDetailedErrors();
#endif
            });
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

            app.MapGet("/Teachers", ([FromServices] PapersDbContext db) => db.Teachers)
                .WithName("GetAllTeachers");

            app.MapGet("/Teachers/{name}", ([FromServices] PapersDbContext db, [FromRoute] string name) => from teacher in db.Teachers
                                                                                                           where teacher.Name == name
                                                                                                           select teacher);

            app.MapGet("/Students", ([FromServices] PapersDbContext db) => db.Students)
                .WithName("GetAllStudents");

            app.MapGet("/Students/{name}", ([FromServices] PapersDbContext db, [FromRoute] string name) => from student in db.Students
                                                                                                           where student.Name == name
                                                                                                           select student);

            app.MapGet("/Teachers/{name}/Students", ([FromServices] PapersDbContext db, [FromRoute] string name) => db.Teachers.Where(teacher => teacher.Name == name)
                                                                                                                               .Select(teacher => teacher.Classes
                                                                                                                               .Select(@class => @class.Students)));

            app.MapGet("/Classes", ([FromServices] PapersDbContext db) =>
            {
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Class, ICollection<Student>> includableQueryable = db.Classes.Include(@class => @class.TeacherNavigation).Include(@class => @class.Students);

                IEnumerable<Class> classes = JsonSerializer.Deserialize<IEnumerable<Class>>(JsonSerializer.Serialize(includableQueryable))!;

                Console.WriteLine(JsonSerializer.Serialize(includableQueryable));
                Console.WriteLine("DATA: " + JsonSerializer.Serialize(classes.Select(@class => @class.Students)));

                return includableQueryable;
            })
            .WithName("GetAllClasses");

#if DEBUG
            // This HTTP-GET method will fill the database with 1 class of 4 students and 1 teacher, generated by Faker then returns all teachers.
            app.MapGet("/FakeData", ([FromServices] PapersDbContext db) =>
            {
                Console.WriteLine("Faking data...");

                Teacher teacher = new Teacher { Name = faker.Name.FullName(), Password = faker.Random.Number(0, int.MaxValue) };
                Class @class = new Class { Id = Guid.NewGuid(), TeacherNavigation = teacher };

                teacher.Classes.Add(@class);

                List<Student> students = new List<Student>
                {
                    new Student { Name = faker.Name.FullName(), Password = faker.Random.Number(0, int.MaxValue) },
                    new Student { Name = faker.Name.FullName(), Password = faker.Random.Number(0, int.MaxValue) },
                    new Student { Name = faker.Name.FullName(), Password = faker.Random.Number(0, int.MaxValue) }
                };

                students.ForEach(st => @class.Students.Add(st));

                db.Teachers.Add(teacher);
                db.Classes.Add(@class);
                db.Students.AddRange(students);

                db.SaveChanges();
            });

            app.MapGet("/Clear", ([FromServices] PapersDbContext db) =>
            {
                IEnumerable<string> tables = from type in db.Model.GetEntityTypes()
                                             select type.GetTableName();

                foreach (string table in tables.Distinct())
                {
                    db.Database.ExecuteSqlRaw($"""TRUNCATE TABLE {db.Model.GetDefaultSchema()}."{table}" CASCADE;""");
                }

                db.SaveChanges();
            });
#endif

            app.Run();
        }

        private static IEnumerable<Teacher> GetTeachers([FromServices] PapersDbContext db) => db.Teachers;
        private static IEnumerable<Student> GetStudents([FromServices] PapersDbContext db) => db.Students;
        private static IEnumerable<Class> GetClasses([FromServices] PapersDbContext db) => db.Classes;
    }
}