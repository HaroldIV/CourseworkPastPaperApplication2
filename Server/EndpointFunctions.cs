using CourseworkPastPaperApplication2.Server;
using CourseworkPastPaperApplication2.Shared;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
//using CourseworkPastPaperApplication.Shared;
namespace CourseworkPastPaperApplication2
{
    internal static class EndpointFunctions
    {
        // Gets all students in a class
        public static async Task<IEnumerable<Student>?> GetStudentsInClass([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            return await
                (
                from _class in db.Classes
                where _class.Id == classId
                select _class.Students
                ).FirstOrDefaultAsync();
        }

        // Signs up (adds) a new user if the user is valid. 
        // Uses a generic type parameter to build separate functions for the Teacher and the Student types with a single definition. 
        public static async Task<IResult> SignUp<TUser>([FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user) where TUser : User, new()
        {
            // Uses the UserValidator added in the services part of the program to validate the users.
            ValidationResult validationResult = await validator.ValidateAsync(user);

            // Gets the corresponding table for the type of the current user. 
            DbSet<TUser> userSet = GetSet<TUser>(typeof(TUser), db);

            // Checks if the user is in the database. 
            Task<bool> inDatabase = userSet.AnyAsync(queryUser => queryUser.Name == user.Name);

            if (!validationResult.IsValid)
            {
                // Gets a dictionary of all errors
                Dictionary<string, string[]> errors = validationResult.Errors.ToDictionary();
                // If in debug mode, writes all errors to the console log. 
#if DEBUG
                Console.WriteLine(JsonSerializer.Serialize(errors));
#endif
                // Returns an HTTP 400 with all validation errors. 
                return Results.ValidationProblem(errors);
            }

            // If the user is present, returns an HTTP 409 with the user data. 
            if (await inDatabase)
            {
                return Results.Conflict(user);
            }

            //
            TUser userToAdd = new TUser
            {
                Name = user.Name,
                PasswordAsHex = hasher.HashPassword(user.Name, user.Password)
            };

            await userSet.AddAsync(userToAdd);

            // Trys to save changes, if failure, returns an HTTP 500. 
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.StatusCode(500);
            }

            return Results.Created(nameof(GetTeacherById), userToAdd);
        }

        // Adds an assignment to a class. 
        public static async Task AddAssignmentToClass([FromRoute] Guid classId, [FromBody] Assignment assignment, [FromServices] PapersDbContext db)
        {
            // Finds the class indicated by classId
            Class? @class = await db.Classes.Include(cl => cl.Students).FirstOrDefaultAsync(cl => cl.Id == classId);

            if (@class is null)
            {
                throw new KeyNotFoundException($"{classId} was not found in {nameof(db.Classes)}");
            }

            // Adds the assignment to the database.
            await db.Assignments.AddAsync(assignment);

            // Adds each a blank PaperResult for each student, for each question for the assignment. 
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

            // Attaches questions so they are not duplicated in the database. 
            db.Questions.AttachRange(assignment.Questions);

            // Adds the assignment to the class. 
            @class.Assignments.Add(assignment);

            // Saves Changes
            await db.SaveChangesAsync();
        }

        // Adds a collection of students to a given class
        public static async Task AddStudentsToClass([FromRoute] Guid classId, [FromBody] IEnumerable<Student> students, [FromServices] PapersDbContext db)
        {
            // Finds the class. 
            Class @class = await db.Classes.FindAsync(classId) ?? throw new KeyNotFoundException();

            // Adds each student to the class. 
            foreach (Student student in students)
            {
                @class.Students.Add(student);
            }

            await db.SaveChangesAsync();
        }

        // Adds a class to a teacher. 
        public static async Task AddTeacherClass([FromRoute] Guid teacherId, [FromServices] PapersDbContext db, [FromBody] Class classBase)
        {
            // Finds the teacher. 
            Teacher? teacher = await db.Teachers.FindAsync(teacherId);

            // If no teacher was found, early return (Guard Clause)
            if (teacher is null)
            {
                return;
            }

            // Attaches each student in the new class to the database facade to ensure they are not duplicated on adding the class. 
            db.Students.AttachRange(classBase.Students);

            // Adds the class to the database
            db.Classes.Add(classBase);

            // Adds the class to the teacher.
            teacher.Classes.Add(classBase);

            // Saves changes
            await db.SaveChangesAsync();
        }

        // Checks if a PasswordVerificationResult was successful or not via pattern matching. 
        public static bool CheckValidVerificationResult(PasswordVerificationResult result) => result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;

        // Gets an assignment from an id. 
        public static async Task<Assignment?> GetAssignment([FromRoute] Guid assignmentId, [FromServices] PapersDbContext db)
        {
            return await db.Assignments.FindAsync(assignmentId);
        }

        // Returns a ResultsTableInitialisationComponents object such that the client can build an a ResultsTable from them. 
        public static async Task<ResultsTableInitialisationComponents?> GetAssignmentResults([FromRoute] Guid assignmentId, [FromServices] PapersDbContext db)
        {
            // Query that gets the assignments and all their questions and all the students that have the assignment as well as all their results for the questions. 
            var assignment = await db.Assignments
                .Include(@assignment => @assignment.Questions)
                .Include(@assignment => @assignment.Class.Students)
                .ThenInclude(student => student.PaperResults)
                .Select(assignment => new { assignment.Name, assignment.Id, assignment.Class, Questions = assignment.Questions.Select(question => new Question { FileName = question.FileName, Id = question.Id, Marks = question.Marks }) })
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            // Early return if assignment was not found. 
            if (assignment is null)
            {
                return null;
            }

            // Instantiates and returns the ResultsTableInitialisationComponents object with the retrieved data. 
            return new ResultsTableInitialisationComponents { Students = assignment.Class.Students.ToArray(), AssignmentId = assignment.Id, Name = assignment.Name, Questions = assignment.Questions.ToArray() };
        }

        // Gets assignments with properties included, as specified by query parameters in the URL. (Currently only questions, this function was build with extension in mind)
        public static async Task<Assignment?> GetAssignmentWith([FromRoute] Guid assignmentId, [FromServices] PapersDbContext db, [FromQuery] bool questions = false)
        {
            IQueryable<Assignment> assignments = db.Assignments;

            if (questions)
                assignments = assignments.Include(assignment => assignment.Questions);

            return await assignments.FirstOrDefaultAsync(assignment => assignment.Id == assignmentId);
        }

        // Gets all assignments for a class
        public static async Task<IEnumerable<Assignment>> GetClassAssignments([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            // Gets the specified class with all its assignments
            Class? @class = await db.Classes.Include(@class => @class.Assignments).FirstOrDefaultAsync(@class => @class.Id == classId);

            // If no class was found, logs an error and returns an empty collection of Assignments
            if (@class is null)
            {
                Console.WriteLine("Error in retrieving class");

                return Enumerable.Empty<Assignment>();
            }

            return @class.Assignments;
        }

        // Gets all classes
        public static IEnumerable<Class> GetClasses([FromServices] PapersDbContext db) => db.Classes;

        // Gets all the PaperResults for a specific student for a specific assignment
        public static IEnumerable<PaperResult> GetPaperResultsForSpecificStudentAssignment([FromServices] PapersDbContext db, [FromRoute] Guid assignmentId, [FromRoute] Guid studentId)
        {
            // matches assignment id then student id. 
            return db.PaperResults.Where(paperResult => paperResult.AssignmentId == assignmentId)
                                  .Where(paperResult => paperResult.StudentId == studentId);
        }

        // Gets all questions of a given set of questions that match keywords specified in the provided search model. 
        public static IAsyncEnumerable<Question> GetQuestionsThatMatchKeywords(QuestionSearchModel searchModel, IQueryable<Question> questionsOfCorrectExamBoardAndLevel)
        {
            IAsyncEnumerable<Question> questions = questionsOfCorrectExamBoardAndLevel.AsAsyncEnumerable();

            // If specified for all keywords to match, only returns where all keywords are included in the read data of the question. (This matching is only around 95-98% accurate)
            if (searchModel.All)
            {
                return
                    from question in questions
                    where searchModel.KeywordsList.All(keyword => question.ReadData.Contains(keyword))
                    select question;
            }

            // Orders by the number of matching keywords
            return
                from question in questions
                orderby searchModel.KeywordsList.Count(keyword => question.ReadData.Contains(keyword))
                descending
                select question;
        }

        // Gets questions specified by a search model. 
        // Only returns CountOfQuestionsToReturn questions. 
        public static IAsyncEnumerable<Question> GetQuestionsWithFilters([FromBody] QuestionSearchModel searchModel, [FromServices] PapersDbContext db)
        {
            /// Limiting the number of questions to return to avoid issues with memory
            const int CountOfQuestionsToReturn = 5;

            // Retrieves where exam board and level matches. 
            IQueryable<Question> questionsOfCorrectExamBoardAndLevel = from question in db.Questions
                                                                       where searchModel.ExamBoards.Contains(question.ExamBoard)
                                                                       where searchModel.ValidLevels.Contains(question.Level)
                                                                       select question;

            // Gets questions that match keywords.
            return GetQuestionsThatMatchKeywords(searchModel, questionsOfCorrectExamBoardAndLevel).Take(CountOfQuestionsToReturn);
        }

        // Gets all radio options (exam boards, levels)
        public static async Task<RadioOptions> GetRadioOptions([FromServices] PapersDbContext db)
        {
            return new RadioOptions(Levels: await db.Levels.ToArrayAsync(), ExamBoards: await db.ExamBoards.ToArrayAsync());
        }

        // Gets the database table set for a given type (avoiding being fully generic for performance). 
        public static DbSet<TUser> GetSet<TUser>(Type type, PapersDbContext db) where TUser : User => (DbSet<TUser>)(object)(type == typeof(Teacher) ? db.Teachers : db.Students);

        // Gets all students that match a given name as either like the name or matching the Soundex form of the name to the Soundex form of the student's name. 
        // Soundex is an algorithm that approximates what a word should sound like. 
        public static IQueryable<Student> GetSoundsLike([FromServices] PapersDbContext db, [FromRoute] string username)
        {
            var matches = db.Students.Where(student => PapersDbContext.Soundex(student.Name) == PapersDbContext.Soundex(username) || EF.Functions.ILike(student.Name, $"%{username}%"));
            return matches;
        }

        // Retrieves all students
        public static IEnumerable<Student> GetStudents([FromServices] PapersDbContext db) => db.Students;

        // Gets teacher with id
        public static async Task<Teacher?> GetTeacherById([FromServices] PapersDbContext db, [FromRoute] Guid Id)
        {
            return await db.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == Id);
        }

        // Gets all classes of a teacher. 
        public static async Task<IEnumerable<Class>?> GetTeacherClasses([FromServices] PapersDbContext db, [FromRoute] Guid teacherId)
        {
            Teacher? teacher = await db.Teachers.Include(teacher => teacher.Classes).ThenInclude(_class => _class.Students).FirstOrDefaultAsync(teacher => teacher.Id == teacherId);
            return teacher?.Classes;
        }

        // Gets all teachers. 
        public static IEnumerable<Teacher> GetTeachers([FromServices] PapersDbContext db) => db.Teachers;

        // Reads in a given image file and adds it and an associated question to the database. 
        // This method is wasteful of memory. If bottlenecks arise, this should be refactored first. 
        // A possible issue could be for a file that is too large. If this is an issue, using multiple requests to send the data property should be considered. 
        public static async Task<IResult> ReadInImageFile([FromBody] Question questionBase, [FromServices] PapersDbContext db)
        {
            if (questionBase.ExamBoard is not null)
                questionBase.ExamBoard = (await db.ExamBoards.FindAsync(questionBase.ExamBoard.Id))!;
            if (questionBase.Level is not null)
                db.Levels.Attach(questionBase.Level);

            // Uses the Tesseract OCR to attempt to read the image to provide the ReadData property of the Question class. 
            questionBase.ReadData = await ImageReader.ReadImageAsync(questionBase.Data);

            // Adds the question to the database. 
            // The question was already pre-built by the client, hence why it only needs the ReadData property being assigned to. 
            await db.Questions.AddAsync(questionBase);

            // Saves
            await db.SaveChangesAsync();

            // Returns HTTP 200 with the questions read data. 
            return Results.Ok(questionBase.ReadData);
        }

        // Deletes an assignment from the database. 
        public static async Task RemoveAssignment([FromRoute] Guid AssignmentId, [FromServices] PapersDbContext db)
        {
            // Creates a new assignment to be attached to the database so it can be removed. 
            var assignment = new Assignment { Id = AssignmentId };

            // Attaches the assignment to the database so it can be removed. 
            db.Assignments.Attach(assignment);

            // Removes the assignment from the database. 
            db.Assignments.Remove(assignment);

            // Saves the changes. 
            await db.SaveChangesAsync();
        }

        public static async Task RemoveClass([FromRoute] Guid classId, [FromServices] PapersDbContext db)
        {
            // Creates a new class to be attached to the database so it can be removed. 
            Class @class = new Class { Id = classId };

            // Attaches the class to the database so it can be removed. 
            db.Classes.Attach(@class);

            // Removes the class from the database. 
            db.Classes.Remove(@class);

            // Saves the changes.
            await db.SaveChangesAsync();
        }

        // Removes given students from a given class. 
        public static async Task RemoveStudents([FromServices] PapersDbContext db, [FromBody] IEnumerable<Student> students, [FromRoute] Guid classId)
        {
            /// Possible optimisation: Alter this to only have to retrieve the students that are needed to remove. 
            /// Possible optimisation: Alter this to only receive ids instead of students. 
            // Gets the class. 
            Class? classToRemoveFrom = await db.Classes.Include(@class => @class.Students).FirstOrDefaultAsync(@class => @class.Id == classId);

            // If the class was not found, early return. 
            if (classToRemoveFrom is null)
            {
                return;
            }

            // Removes each student from the class. 
            foreach (Student student in students)
                classToRemoveFrom.Students.Remove(student);

            // Saves changes. 
            await db.SaveChangesAsync();
        }

        // Updates results from an array of new Paper Results
        // Tries to update the score for each paper result, if this fails at any point due to the paper result not existing, simply stops and returns before saving changes. 
        // If successful, returns HTTP 202
        public static async Task<IResult> UpdateResults([FromServices] PapersDbContext db, [FromBody] PaperResult[] results)
        {
            try
            {
                await results.ForEach(async result => (await db.PaperResults.FindAsync(result.Id) ?? throw new ArgumentException(JsonSerializer.Serialize(result))).Score = result.Score);
            }
            catch (ArgumentException e)
            {
                return Results.NotFound(e.Message);
            }
            finally
            {
                await db.SaveChangesAsync();
            }

            return Results.Accepted();
        }

        // Checks if a teacher's login information is valid and returns Success if they are. 
        public static async Task<IResult> ValidateTeacher([FromServices] User userInstance, [FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user)
        {
            // Validates the given login details. 
            // If they are invalid then there is no need to query the database. 
            ValidationResult validationResult = await validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors.ToDictionary();

                // logs and returns the errors as an HTTP 400
#if DEBUG
                Console.WriteLine(JsonSerializer.Serialize(errors));
#endif

                return Results.ValidationProblem(errors);
            }

            // Gets the teacher with a matching password or null if no teacher was found. 
            var queriedTeacher = await db.Teachers.Where(teacher => teacher.Name == user.Name)
                                               .Select(teacher => new { teacher.Id, user.Name, Password = PapersDbContext.SQLUtf8ToString(teacher.Password) })
                                               .AsAsyncEnumerable()
                                               .FirstOrDefaultAsync(teacher => (hasher.VerifyHashedPassword(teacher.Name, teacher.Password, user.Password) & (PasswordVerificationResult.Success | PasswordVerificationResult.SuccessRehashNeeded)) != 0);


            // If no teacher found, return HTTP 404. 
            if (queriedTeacher is null)
            {
                return Results.NotFound(user);
            }

            // Sets the details of the scoped user instance in the services part of the program. 
            userInstance.Id = queriedTeacher.Id;
            userInstance.Name = queriedTeacher.Name;
            userInstance.PasswordAsHex = queriedTeacher.Password;

            return Results.Ok(userInstance);
        }

        // Checks if a user's login information is valid and returns Success if they are. 
        public static async Task<IResult> ValidateUser<TUser>([FromServices] User userInstance, [FromServices] UserValidator validator, [FromServices] PasswordHasher<string> hasher, [FromServices] PapersDbContext db, [FromBody] UserWithUnencryptedPassword user) where TUser : User
        {
            // Validates the given login details. 
            // If they are invalid then there is no need to query the database. 
            ValidationResult validationResult = await validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors.ToDictionary();

                // logs and returns the errors as an HTTP 400
#if DEBUG
                Console.WriteLine(JsonSerializer.Serialize(errors));
#endif

                return Results.ValidationProblem(errors);
            }

            // Gets the set to add to. 
            DbSet<TUser> userSet = GetSet<TUser>(typeof(TUser), db);

            // Gets the user with a matching password or null if no user was found. 
            var queriedUser = await userSet.Where(queryUser => queryUser.Name == user.Name)
                                               .Select(queryUser => new { queryUser.Id, queryUser.Name, Password = PapersDbContext.SQLUtf8ToString(queryUser.Password) })
                                               .AsAsyncEnumerable()
                                               .FirstOrDefaultAsync(queryUser => (hasher.VerifyHashedPassword(queryUser.Name, queryUser.Password, user.Password) & (PasswordVerificationResult.Success | PasswordVerificationResult.SuccessRehashNeeded)) != 0);
            // If no user found, return HTTP 404. 
            if (queriedUser is null)
            {
                return Results.NotFound(user);
            }

            // Sets the details of the scoped user instance in the services part of the program. 
            userInstance.Id = queriedUser.Id;
            userInstance.Name = queriedUser.Name;
            userInstance.PasswordAsHex = queriedUser.Password;

            return Results.Ok(userInstance);
        }
    }
}