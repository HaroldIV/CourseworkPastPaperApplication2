using CourseworkPastPaperApplication2.Shared;
using FluentValidation;
using System.Linq;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Server
{
    // This class is used for validating users' usernames and passwords. 
    // This class inherits from the AbstractValidator class which is supplied by the FluentValidation package that allows for terse and functional definitions for validation rules. 
    public class UserValidator : AbstractValidator<UserWithUnencryptedPassword>
    {
        // The set of all valid characters. 
        // The property accessors allow it to be retreieved but only initialised, making it a readonly property. 
        // The IReadOnlySet<char> is an interface, this allows for polymorphism without the constraints of inheritance. 
        private IReadOnlySet<char> validPasswordChars { get; init; }

        public UserValidator()
        {
            // Uses a singleton (class for which there can be only a single instance) to keep track of all valid passwords. 
            // A singleton is used to save on memory. 
            // If demand grew then this could be refactored to use a pool of password sets and use whichever is not currently in use. 
            // As it stands, this is not a necessity. 
            validPasswordChars = ValidCharsHashSetSingleton.GetInstance();

            // Each of these validation rules has an associated error message that will be returned if the validation rule fails. 

            // Defines validation rules for the username. 
            // These rules state that the username must not be null and must be between 6 and 48 characters in length (inclusive). 
            const int UsernameMinimumLength = 6;
            const int UsernameMaximumLength = 48;
            RuleFor(user => user.Name).NotNull()
                                      .MinimumLength(UsernameMinimumLength).WithMessage($"Username was too short. Minimum length {UsernameMinimumLength}.")
                                      .MaximumLength(UsernameMaximumLength).WithMessage($"Username was too long. Maximum length {UsernameMaximumLength}.");

            // Defines validation rules for the password. 
            // These rules state that the password must not be null and must be between 12 and 60 characters in length (inclusive) and that each character must be contained within the set of valid characters. 
            const int PasswordMinimumLength = 12;
            const int PasswordMaximumLength = 60;
            RuleFor(user => user.Password).NotNull()
                                          .MinimumLength(PasswordMinimumLength).WithMessage($"Password was too short. Minimum length {PasswordMinimumLength}.")
                                          .MaximumLength(PasswordMaximumLength).WithMessage($"Password was too long. Maximum length {PasswordMaximumLength}.")
                                          .Must(pass => pass.All(validPasswordChars.Contains)).WithMessage($"Cam only have alphanumeric characters and the following symbols: '{string.Concat(validPasswordChars.Where(c => !char.IsAsciiLetterOrDigit(c)))}'");
        }
    }

    // Singleton containing all the valid characters. 
    // This inherits from the HashSet class which uses hashing to create a collection of distinct values and O(1) lookup times for checking if a value is contained within the set. 
    public class ValidCharsHashSetSingleton : HashSet<char>
    {
        // All valid characters for the password. 
        private const string ValidPasswordCharacters = """abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 "~`!@#$%^&*()_-+={}[]|\:;'<,>.?/""";

        // Instance of the class, warning disabled for compilation purposes. 
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static ValidCharsHashSetSingleton instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Delegate that handles the getting of an instance. 
        // At the start, this is set to a function that will both set the instance and get it. 
        // Later, this is set to a function that will only return the instance. 
        // This uses the idea that functions are first-class objects and so can be reassigned freely. 
        // The property accessor for setting this is made private to prevent it from being assigned to from without. 
        public static InstanceGetter GetInstance { get; private set; } = GetAndSetInstance;

        // Definition for the delegate type that GetInstance is of. 
        // This states that InstanceGetter will be a function that will return a ValidCharsHashSetSingleton. 
        public delegate ValidCharsHashSetSingleton InstanceGetter();

        // This sets the instance to be a new instance of the class and fills it with the characters it checks for. 
        private static ValidCharsHashSetSingleton GetAndSetInstance()
        {
            instance = new ValidCharsHashSetSingleton();

            foreach (char character in ValidPasswordCharacters)
            {
                instance.Add(character);
            }

            GetInstance = OnlyGetInstance;

            return instance;
        }

        // This returns the instance only. 
        private static ValidCharsHashSetSingleton OnlyGetInstance()
        {
            return instance;
        }

        // Private constructor preventing this class from being instantiated without the GetInstance method. 
        private ValidCharsHashSetSingleton()
        {
        }
    }
}
