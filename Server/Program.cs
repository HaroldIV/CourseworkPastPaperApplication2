//using CourseworkPastPaperApplication.Shared;
using CourseworkPastPaperApplication2.Server;
using CourseworkPastPaperApplication2.Shared;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            builder.Services.AddScoped<PasswordHasher<string>>();
            builder.Services.AddScoped<UserValidator>();
            builder.Services.AddScoped<User>(x => new Teacher());

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
                builder.Services.AddLogging();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();

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

            MapEndpoints(app);

            app.Run();
        }

        private static void MapEndpoints(WebApplication app)
        {
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

            app.MapPost("/ValidateTeacher", ValidateUser<Teacher>);

            app.MapPost("/ValidateStudent", ValidateUser<Student>);

            app.MapPost("/SignUpTeacher", SignUp<Teacher>);

            app.MapPost("/SignUpStudent", SignUp<Student>);

            app.MapGet("/Student/SoundsLike/{username}", GetSoundsLike);

            app.MapGet("/Teacher/{teacherId:guid}/Classes", GetTeacherClasses);

            app.MapPut("/Class/{classId:guid}/Remove", RemoveStudents);

            app.MapPost("/Teacher/{teacherId:guid}/Classes", AddTeacherClass);

            app.MapDelete("/Class/{classId:guid}", RemoveClass);

#if DEBUG
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
        }

        private static async Task RemoveClass([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            Class @class = new Class { Id = classId };

            db.Classes.Attach(@class);

            db.Classes.Remove(@class);

            await db.SaveChangesAsync();
        }

        private static async Task AddTeacherClass([FromRoute] Guid teacherId, [FromServices] PapersDbContext db, [FromBody] IEnumerable<Student> studentsToAdd)
        {
            Teacher? teacher = await db.Teachers.FindAsync(teacherId);

            if (teacher is null)
            {
                return;
            }

            Class @class = new Class();

            foreach (var student in studentsToAdd)
            {
                @class.Students.Add(student);
            }

            teacher.Classes.Add(@class);

            await db.SaveChangesAsync();
        }

        private static async Task RemoveStudents([FromServices] PapersDbContext db, [FromBody] IEnumerable<Student> students, [FromRoute] Guid classId)
        {
            Class? classToRemoveFrom = await db.Classes.FirstOrDefaultAsync(@class => @class.Id == classId);

            if (classToRemoveFrom is null)
            {
                return; 
            }

            db.Students.AttachRange(students);

            foreach (Student student in students)
                classToRemoveFrom.Students.Remove(student);

            await db.SaveChangesAsync();
        }

        private static async Task<IEnumerable<Class>?> GetTeacherClasses([FromServices] PapersDbContext db, [FromRoute] Guid teacherId)
        {
            Teacher? teacher = (await db.Teachers.Include(teacher => teacher.Classes).ThenInclude(_class => _class.Students).FirstOrDefaultAsync(teacher => teacher.Id == teacherId));
            return teacher?.Classes;
        }

        private static IQueryable<Student> GetSoundsLike([FromServices] PapersDbContext db, [FromRoute] string username)
        {
            //return
            //        from student in db.Students
            //        let soundsLike = EF.Functions.FuzzyStringMatchSoundex(username)
            //        where EF.Functions.FuzzyStringMatchSoundex(student.Name) == soundsLike
            //        select student;

            var matches = db.Students.Where(student => PapersDbContext.Soundex(student.Name) == PapersDbContext.Soundex(username) || EF.Functions.ILike(student.Name, $"%{username}%"));
            return matches;
        }

        private static async Task<Teacher?> GetTeacherById([FromServices] PapersDbContext db, [FromRoute] Guid Id)
        {
            return await db.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == Id);
        }

        private static async Task<IResult> ValidateTeacher([FromServices] User userInstance, [FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user)
        {
            ValidationResult validationResult = await validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                Console.WriteLine(JsonSerializer.Serialize(validationResult.Errors.ToDictionary()));

                return Results.ValidationProblem(validationResult.Errors.ToDictionary());
            }

            var queriedTeacher = await db.Teachers.Where(teacher => teacher.Name == user.Name)
                                               .Select(teacher => new { teacher.Id, user.Name, Password = PapersDbContext.SQLUtf8ToString(teacher.Password) })
                                               .AsAsyncEnumerable()
                                               .FirstOrDefaultAsync(teacher => (hasher.VerifyHashedPassword(teacher.Name, teacher.Password, user.Password) & (PasswordVerificationResult.Success | PasswordVerificationResult.SuccessRehashNeeded)) != 0);


            if (queriedTeacher is null)
            {
                return Results.NotFound(user);
            }

            userInstance.Id = queriedTeacher.Id;
            userInstance.Name = queriedTeacher.Name;
            userInstance.PasswordAsHex = queriedTeacher.Password;

            return Results.Ok(userInstance);
        }

        private static async Task<IResult> ValidateUser<TUser>([FromServices] User userInstance, [FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user) where TUser : User
        {
            ValidationResult validationResult = await validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                Console.WriteLine(JsonSerializer.Serialize(validationResult.Errors.ToDictionary()));

                return Results.ValidationProblem(validationResult.Errors.ToDictionary());
            }

            DbSet<TUser> userSet = GetSet<TUser>(typeof(TUser), db);

            var queriedUser = await userSet.Where(queryUser => queryUser.Name == user.Name)
                                               .Select(queryUser => new { queryUser.Id, queryUser.Name, Password = PapersDbContext.SQLUtf8ToString(queryUser.Password) })
                                               .AsAsyncEnumerable()
                                               .FirstOrDefaultAsync(queryUser => (hasher.VerifyHashedPassword(queryUser.Name, queryUser.Password, user.Password) & (PasswordVerificationResult.Success | PasswordVerificationResult.SuccessRehashNeeded)) != 0);
           
            if (queriedUser is null)
            {
                return Results.NotFound(user);
            }

            userInstance.Id = queriedUser.Id;
            userInstance.Name = queriedUser.Name;
            userInstance.PasswordAsHex = queriedUser.Password;

            return Results.Ok(userInstance);
        }

        /// <summary>
        /// Adds a teacher from an unencrypted user.
        /// </summary>
        /// <returns>ValidationProblem | Conflict | 500 | Created</returns>
        public static async Task<IResult> SignUpTeacher([FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user)
        {
            ValidationResult validationResult = await validator.ValidateAsync(user);

            Task<bool> inDatabase = db.Teachers.AnyAsync(teacher => teacher.Name == user.Name);

            if (!validationResult.IsValid)
            {
                Console.WriteLine(JsonSerializer.Serialize(validationResult.Errors.ToDictionary()));

                return Results.ValidationProblem(validationResult.Errors.ToDictionary());
            }

            if (await inDatabase)
            {
                return Results.Conflict(user);
            }

            Teacher teacher = new Teacher
            {
                Name = user.Name,
                PasswordAsHex = hasher.HashPassword(user.Name, user.Password)
            };

            await db.Teachers.AddAsync(teacher);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.StatusCode(500);
            }

            return Results.Created(nameof(GetTeacherById), teacher);
        }

        public static async Task<IResult> SignUp<TUser>([FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user) where TUser : User
        {
            ValidationResult validationResult = await validator.ValidateAsync(user);

            DbSet<TUser> userSet = GetSet<TUser>(typeof(TUser), db);

            Task<bool> inDatabase = userSet.AnyAsync(queryUser => queryUser.Name == user.Name);

            if (!validationResult.IsValid)
            {
                Console.WriteLine(JsonSerializer.Serialize(validationResult.Errors.ToDictionary()));

                return Results.ValidationProblem(validationResult.Errors.ToDictionary());
            }

            if (await inDatabase)
            {
                return Results.Conflict(user);
            }

            Teacher teacher = new Teacher
            {
                Name = user.Name,
                PasswordAsHex = hasher.HashPassword(user.Name, user.Password)
            };

            var a = hasher.VerifyHashedPassword(teacher.Name, teacher.PasswordAsHex, user.Password);

            await db.Teachers.AddAsync(teacher);

            Console.WriteLine(user.Password);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.StatusCode(500);
            }

            return Results.Created(nameof(GetTeacherById), teacher);
        }

        private static DbSet<TUser> GetSet<TUser>(Type type, PapersDbContext db) where TUser : User => (DbSet<TUser>)(object)(type == typeof(Teacher) ? db.Teachers : db.Students);


        private static bool CheckValidVerificationResult(PasswordVerificationResult result) => result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;

        private static IEnumerable<Teacher> GetTeachers([FromServices] PapersDbContext db) => db.Teachers;
        private static IEnumerable<Student> GetStudents([FromServices] PapersDbContext db) => db.Students;
        private static IEnumerable<Class> GetClasses([FromServices] PapersDbContext db) => db.Classes;
    }

    public static class Extensions
    {
        public static Dictionary<string, string[]> ToDictionary(this List<ValidationFailure> errors)
        {
            var result = new Dictionary<string, List<string>>();

            foreach (ValidationFailure error in errors)
            {
                if (result.TryAdd(error.PropertyName, new List<string>() { error.ErrorMessage }))
                {
                    continue;
                }

                result[error.PropertyName].Add(error.ErrorMessage);
            }

            return result.ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value.ToArray()
                );
        }
    }
}