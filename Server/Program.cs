//using CourseworkPastPaperApplication.Shared;
using CourseworkPastPaperApplication2.Server;
using CourseworkPastPaperApplication2.Shared;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CourseworkPastPaperApplication2
{
    public class Program
    {
        /// This code checks if the program is being compiled in debug mode, if it is, instantiates a Faker object used for filling the database with fake data. 
#if DEBUG
        private static readonly Bogus.Faker faker = new Bogus.Faker();
#endif
        // Entry-point for the Server
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adds services to the container.
            // Generic boilerplate services to add. 
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Adds the database facade. 
            builder.Services.AddDbContext<PapersDbContext>(optionsAction: options =>
            {
                // If compiled in debug mode, adds extra logging which would normally slow down the program so is not used for Release mode. 
                // builder.Configuration.GetConnectionString returns the connection string of the database via the specified key (parameter) from the AppSettings.json file. 
#if DEBUG
                options.UseNpgsql(builder.Configuration.GetConnectionString("WebAPIDatabaseWithErrorDetail"));
                options.EnableSensitiveDataLogging()
                       .EnableDetailedErrors();
#else
                options.UseNpgsql(builder.Configuration.GetConnectionString("WebAPIDatabase"));
#endif
            });

            // AddScoped adds a new instance per used. 
            // Adds a new PasswordHasher which is used by the program to hash and verify passwords, string parameter is since the username is used to hash the password
            builder.Services.AddScoped<PasswordHasher<string>>();
            // User validation service. 
            builder.Services.AddScoped<UserValidator>();
            // Adds a user service which keeps track of a user. This is not used everywhere since it is preferred to place workload on the client instead of the server. 
            builder.Services.AddScoped<User>(x => new Teacher());

            // Configures the HttpJsonOptions to follow standard naming conventions. 
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.PropertyNameCaseInsensitive = true;
            });

            // Adds logging
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
                builder.Services.AddLogging();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            ConfigureHttp(app);

            app.UseHttpsRedirection();

            // Adds necessary files for Blazor
            app.UseBlazorFrameworkFiles();
            // Adds static files to be used in the program. 
            app.UseStaticFiles();

            app.UseRouting();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            // Maps all endpoints, for the incoming request to the function to be executed. 
            MapEndpoints(app);

            // Runs the app and blocks the thread until the app stops running. 
            app.Run();

            static void ConfigureHttp(WebApplication app)
            {
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();

                    return;
                }

                // Allows for breakpoints and better debugging if in debug mode. 
                app.UseWebAssemblyDebugging();

                // This method logs all incoming requests to the server as well as some information about them. 
                app.Use(async (context, next) =>
                {
                    // Log incoming request
                    app.Logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

                    // Call the next middleware in the pipeline
                    await next.Invoke();

                    // Log outgoing response
                    app.Logger.LogInformation($"Outgoing response: {context.Response.StatusCode}");
                });
            }
        }

        private static void MapEndpoints(WebApplication app)
        {
#if DEBUG
            MapDebugEndpoints(app);
#endif
            // Maps endpoints corresponding to the given subject
            // E.g. MapClassEndpoints maps all endpoints relating to retrieving, updating, etc. classes. 
            MapLoginEndpoints(app);
            MapSignUpEndpoints(app);
            MapStudentEndpoints(app);
            MapTeacherEndpoints(app);
            MapClassEndpoints(app);
            MapQuestionEndpoints(app);
            MapAssignmentEndpoints(app);
            MapPaperResultEndpoints(app);
        }

        private static void MapPaperResultEndpoints(WebApplication app)
        {
            app.MapPut("/PaperResult/Results", EndpointFunctions.UpdateResults);
        }

        private static void MapAssignmentEndpoints(WebApplication app)
        {
            app.MapGet("/Assignment/{assignmentId:guid}/Withh/", EndpointFunctions.GetAssignmentWith);

            app.MapGet("/Assignment/{assignmentId:guid}", EndpointFunctions.GetAssignment);

            app.MapDelete("/Assignment/{assignmentId:guid}", EndpointFunctions.RemoveAssignment);

            app.MapGet("/Assignment/{assignmentId:guid}/Results", EndpointFunctions.GetAssignmentResults);

            app.MapGet("/Assignment/{assignmentId}/Student/{studentId}/Result", EndpointFunctions.GetPaperResultsForSpecificStudentAssignment);
        }

        private static void MapQuestionEndpoints(WebApplication app)
        {
            app.MapPut("/Image", EndpointFunctions.ReadInImageFile);

            app.MapGet("/Options", EndpointFunctions.GetRadioOptions);

            app.MapPost("/Question", EndpointFunctions.GetQuestionsWithFilters);
        }

        private static void MapClassEndpoints(WebApplication app)
        {
            app.MapPost("/Class/{classId:guid}/Remove", EndpointFunctions.RemoveStudents);

            app.MapDelete("/Class/{classId:guid}", EndpointFunctions.RemoveClass);

            app.MapGet("/Classs/{classId:guid}/Students", EndpointFunctions.GetStudentsInClass);

            app.MapPost("/Class/{classId:guid}/Add", EndpointFunctions.AddStudentsToClass);

            app.MapGet("/Class/{classId:guid}/Assignments", EndpointFunctions.GetClassAssignments);

            app.MapPost("/Class/{classId:guid}/Assignments", EndpointFunctions.AddAssignmentToClass);
        }

        private static void MapTeacherEndpoints(WebApplication app)
        {
            app.MapGet("/Teacher/{teacherId:guid}/Classes", EndpointFunctions.GetTeacherClasses);
            app.MapPut("/Teacher/{teacherId:guid}/Classes", EndpointFunctions.AddTeacherClass);
        }

        private static void MapStudentEndpoints(WebApplication app)
        {
            app.MapGet("/Student/SoundsLike/{username}", EndpointFunctions.GetSoundsLike);
        }

        private static void MapSignUpEndpoints(WebApplication app)
        {
            app.MapPost("/SignUpTeacher", EndpointFunctions.SignUp<Teacher>);

            app.MapPost("/SignUpStudent", EndpointFunctions.SignUp<Student>);
        }

        private static void MapLoginEndpoints(WebApplication app)
        {
            app.MapPost("/ValidateTeacher", EndpointFunctions.ValidateUser<Teacher>);

            app.MapPost("/ValidateStudent", EndpointFunctions.ValidateUser<Student>);
        }

# if DEBUG
        // Maps all endpoints used in the debug build of the app, used for debugging and filling the database with fake data, etc. 
        private static void MapDebugEndpoints(WebApplication app)
        {
            // Endpoint to get all teachers
            app.MapGet("/Teachers", ([FromServices] PapersDbContext db) => db.Teachers)
                            .WithName("GetAllTeachers");

            // Get teacher with specific name
            app.MapGet("/Teachers/{name}", ([FromServices] PapersDbContext db, [FromRoute] string name) => from teacher in db.Teachers
                                                                                                           where teacher.Name == name
                                                                                                           select teacher);
            // Get all students
            app.MapGet("/Students", ([FromServices] PapersDbContext db) => db.Students)
                .WithName("GetAllStudents");

            // Get student with specific name
            app.MapGet("/Students/{name}", ([FromServices] PapersDbContext db, [FromRoute] string name) => from student in db.Students
                                                                                                           where student.Name == name
                                                                                                           select student);
            // Get students of teacher with specific name for teacher
            app.MapGet("/Teachers/{name}/Students", ([FromServices] PapersDbContext db, [FromRoute] string name) => db.Teachers.Where(teacher => teacher.Name == name)
                                                                                                                               .Select(teacher => teacher.Classes
                                                                                                                               .Select(@class => @class.Students)));
            // Gets all classes
            app.MapGet("/Classes", async ([FromServices] PapersDbContext db) =>
            {
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Class, ICollection<Student>> includableQueryable = db.Classes.Include(@class => @class.TeacherNavigation).Include(@class => @class.Students);

                await db.SaveChangesAsync();

                IEnumerable<Class> classes = JsonSerializer.Deserialize<IEnumerable<Class>>(JsonSerializer.Serialize(includableQueryable))!;

                Console.WriteLine(JsonSerializer.Serialize(includableQueryable));
                Console.WriteLine("DATA: " + JsonSerializer.Serialize(classes.Select(@class => @class.Students)));

                return includableQueryable;
            })
            .WithName("GetAllClasses");

            app.MapGet("/Assignments", ([FromServices] PapersDbContext db) => db.Assignments.Include(x => x.Questions));
            
            // This HTTP-GET method will fill the database with 1 class of 4 students and 1 teacher, generated by Faker then returns all teachers.
            app.MapGet("/FakeData", ([FromServices] PapersDbContext db) =>
            {
                Console.WriteLine("Faking data...");

                Teacher teacher = new Teacher { Name = faker.Name.FullName(), Password = faker.Random.Bytes(32) };
                Class @class = new Class { Id = Guid.NewGuid(), TeacherNavigation = teacher };

                teacher.Classes.Add(@class);

                List<Student> students = new List<Student>
                {
                    new Student { Name = faker.Name.FullName(), Password = faker.Random.Bytes(32) },
                    new Student { Name = faker.Name.FullName(), Password = faker.Random.Bytes(32) },
                    new Student { Name = faker.Name.FullName(), Password = faker.Random.Bytes(32) }
                };

                students.ForEach(st => @class.Students.Add(st));

                db.Teachers.Add(teacher);
                db.Classes.Add(@class);
                db.Students.AddRange(students);

                db.SaveChanges();
            });

            // Clears the database by executing a TRUNCATE statement for each table. 
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
        }
#endif
    }
}