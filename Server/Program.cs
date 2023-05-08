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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using Microsoft.Extensions.FileProviders;
using CourseworkPastPaperApplication2.Client;

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

            app.MapPost("/ValidateTeacher", ValidateUser<Teacher>);

            app.MapPost("/ValidateStudent", ValidateUser<Student>);

            app.MapPost("/SignUpTeacher", SignUp<Teacher>);

            app.MapPost("/SignUpStudent", SignUp<Student>);

            app.MapGet("/Student/SoundsLike/{username}", GetSoundsLike);

            app.MapGet("/Teacher/{teacherId:guid}/Classes", GetTeacherClasses);

            app.MapPut("/Class/{classId:guid}/Remove", RemoveStudents);

            app.MapPost("/Teacher/{teacherId:guid}/Classes", AddTeacherClass);

            app.MapDelete("/Class/{classId:guid}", RemoveClass);

            app.MapGet("/Classs/{classId:guid}/Students", GetStudentsInClass);

            app.MapPut("/Class/{classId:guid}/Add", AddStudentsToClass);

            app.MapPost("/Image", ReadInImageFile);

            app.MapGet("/Options", GetRadioOptions);

            app.MapPost("/Question", GetQuestionsWithFilters);

            app.MapGet("/Class/{classId:guid}/Assignments", GetClassAssignments);

            app.MapGet("/Assignment/{assignmentId:guid}/Withh/", GetAssignmentWith);

            app.MapPost("/Class/{classId:guid}/Assignments", AddAssignmentToClass);

            app.MapGet("/Assignment/{assignmentId:guid}", GetAssignment);

            app.MapDelete("/Assignment/{assignmentId:guid}", RemoveAssignment);

            app.MapGet("/Assignment/{assignmentId:guid}/Results", GetAssignmentResults);

#if DEBUG
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

        /// Need to make this better.
        private static async Task<string[][]?> GetAssignmentResults([FromRoute] Guid assignmentId, [FromServices] PapersDbContext db)
        {
            var assignment = await db.Assignments
                .Include(@assignment => @assignment.Questions)
                .Include(@assignment => @assignment.Class.Students)
                .ThenInclude(student => student.PaperResults)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment is null)
            {
                return null;
            }

            int rowCount = assignment.Questions.Count + 1;
            int columnCount = assignment.Class.Students.Count + 1;
            string[][] scoresTable = new string[rowCount][];

            for (int i = 0; i < scoresTable.Length; i++)
            {
                scoresTable[i] = new string[columnCount]; 
            }

            foreach (var (i, student) in assignment.Class.Students.WithIndex())
            {
                scoresTable[0][i + 1] = student.Name;
            }

            foreach (var (i, question) in assignment.Questions.WithIndex())
            {
                scoresTable[i + 1][0] = question.FileName;
            }



            foreach (var (i, question) in assignment.Questions.WithIndex())
            {
                foreach (var (j, paperResult) in assignment.Class.Students.Select(student => student.PaperResults.First(result => result.QuestionId == question.Id)).WithIndex())
                {
                    scoresTable[i + 1][j + 1] = $"{paperResult.Score}/{question.Marks}";
                }
            }

            return scoresTable;
        }

        private static async Task RemoveAssignment([FromRoute] Guid AssignmentId, [FromServices] PapersDbContext db)
        {
            var assignment = new Assignment { Id = AssignmentId };

            db.Assignments.Attach(assignment);

            db.Assignments.Remove(assignment);

            await db.SaveChangesAsync();
        }

        private static async Task<Assignment?> GetAssignment([FromRoute] Guid assignmentId, [FromServices] PapersDbContext db)
        {
            return await db.Assignments.FindAsync(assignmentId);
        }

        private static async Task<Assignment?> GetAssignmentWith([FromRoute] Guid assignmentId, [FromServices] PapersDbContext db, [FromQuery] bool questions = false)
        {
            IQueryable<Assignment> assignments = db.Assignments;

            if (questions)
                assignments = assignments.Include(assignment => assignment.Questions);

            return await assignments.FirstOrDefaultAsync(assignment => assignment.Id == assignmentId);
        }

        private static async Task AddAssignmentToClass([FromRoute] Guid classId, [FromBody] Assignment assignment, [FromServices] PapersDbContext db)
        {
            Class? @class = await db.Classes.Include(cl => cl.Students).FirstOrDefaultAsync(cl => cl.Id == classId);

            if (@class is null)
            {
                throw new KeyNotFoundException($"{classId} was not found in {nameof(db.Classes)}");
            }

            await db.Assignments.AddAsync(assignment);

            foreach (var student in @class.Students)
            {
                foreach (Question question in assignment.Questions)
                {
                    PaperResult blankPaperResult = new PaperResult 
                    { 
                        Assignment = assignment, 
                        Question = question, 
                        Student = student 
                    };

                    await db.PaperResults.AddAsync(blankPaperResult);
                    student.PaperResults.Add(blankPaperResult);
                }
            }

            db.Questions.AttachRange(assignment.Questions);

            @class.Assignments.Add(assignment);

            await db.SaveChangesAsync();
        }

        private static async Task<IEnumerable<Assignment>> GetClassAssignments([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            Class? @class = await db.Classes.Include(@class => @class.Assignments).FirstOrDefaultAsync(@class => @class.Id == classId);

            if (@class is null)
            {
                Console.WriteLine("Error in retrieving class");
                
                return Enumerable.Empty<Assignment>();
            }

            return @class.Assignments;
        }

        private static IAsyncEnumerable<Question> GetQuestionsWithFilters([FromBody] QuestionSearchModel searchModel, [FromServices] PapersDbContext db)
        {
            /// Limiting the number of questions to return to avoid issues with memory
            const int CountOfQuestionsToReturn = 5;

            IQueryable<Question> questionsOfCorrectExamBoardAndLevel = from question in db.Questions
                                                                       where searchModel.ExamBoards.Contains(question.ExamBoard)
                                                                       where searchModel.ValidLevels.Contains(question.Level)
                                                                       select question;

            return GetQuestionsThatMatchKeywords(searchModel, questionsOfCorrectExamBoardAndLevel).Take(CountOfQuestionsToReturn);
        }

        private static IAsyncEnumerable<Question> GetQuestionsThatMatchKeywords(QuestionSearchModel searchModel, IQueryable<Question> questionsOfCorrectExamBoardAndLevel)
        {
            IAsyncEnumerable<Question> questions = questionsOfCorrectExamBoardAndLevel.AsAsyncEnumerable();

            if (searchModel.All)
            {
                return
                    from question in questions
                    where searchModel.KeywordsList.All(keyword => question.ReadData.Contains(keyword))
                    select question;
            }

            return
                from question in questions
                orderby searchModel.KeywordsList.Count(keyword => question.ReadData.Contains(keyword))
                descending
                select question;
        }

        private static async Task<RadioOptions> GetRadioOptions([FromServices] PapersDbContext db)
        {
            return new RadioOptions(Levels: await db.Levels.ToArrayAsync(), ExamBoards: await db.ExamBoards.ToArrayAsync());
        }

        /// This method is wasteful of memory. If bottlenecks arise, this should be refactored first. 
        private static async Task<IResult> ReadInImageFile([FromBody] Question questionBase, [FromServices] PapersDbContext db)
        {
            if (questionBase.ExamBoard is not null)
                questionBase.ExamBoard = (await db.ExamBoards.FindAsync(questionBase.ExamBoard.Id))!;
            if (questionBase.Level is not null)
                db.Levels.Attach(questionBase.Level);
            

            questionBase.ReadData = await ImageReader.ReadImageAsync(questionBase.Data); 

            await db.Questions.AddAsync(questionBase);

            await db.SaveChangesAsync();

            return Results.Ok(questionBase.ReadData);
        }

        private static async Task AddStudentsToClass([FromRoute] Guid classId, [FromBody] IEnumerable<Student> students, [FromServices] PapersDbContext db)
        {
            Class @class = await db.Classes.FindAsync(classId) ?? throw new KeyNotFoundException();

            foreach (Student student in students)
            {
                @class.Students.Add(student);
            }

            await db.SaveChangesAsync();
        }

        private static async Task RemoveClass([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            Class @class = new Class { Id = classId };

            db.Classes.Attach(@class);

            db.Classes.Remove(@class);

            await db.SaveChangesAsync(); 
        }

        public static async Task<IEnumerable<Student>?> GetStudentsInClass([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            return await
                (
                from _class in db.Classes
                where _class.Id == classId
                select _class.Students
                ).FirstOrDefaultAsync();
        }

        private static async Task AddTeacherClass([FromRoute] Guid teacherId, [FromServices] PapersDbContext db, [FromBody] Class classBase)
        {
            Teacher? teacher = await db.Teachers.FindAsync(teacherId);

            if (teacher is null)
            {
                return;
            }

            db.Students.AttachRange(classBase.Students);

            db.Classes.Add(classBase);

            teacher.Classes.Add(classBase);

            await db.SaveChangesAsync();
        }

        private static async Task RemoveStudents([FromServices] PapersDbContext db, [FromBody] IEnumerable<Student> students, [FromRoute] Guid classId)
        {
            /// Possible optimisation: Alter this to only have to retrieve the students that are needed to remove. 
            /// Possible optimisation: Alter this to only receive ids instead of students. 
            Class? classToRemoveFrom = await db.Classes.Include(@class => @class.Students).FirstOrDefaultAsync(@class => @class.Id == classId);

            if (classToRemoveFrom is null)
            {
                return; 
            }

            foreach (Student student in students)
                classToRemoveFrom.Students.Remove(student);

            await db.SaveChangesAsync();
        }

        private static async Task<IEnumerable<Class>?> GetTeacherClasses([FromServices] PapersDbContext db, [FromRoute] Guid teacherId)
        {
            Teacher? teacher = await db.Teachers.Include(teacher => teacher.Classes).ThenInclude(_class => _class.Students).FirstOrDefaultAsync(teacher => teacher.Id == teacherId);
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