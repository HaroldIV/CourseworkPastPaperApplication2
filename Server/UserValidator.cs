using CourseworkPastPaperApplication2.Shared;
using FluentValidation;
using System.Linq;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Server
{
    public class UserValidator : AbstractValidator<UserWithUnencryptedPassword>
    {
        private IReadOnlySet<char> validPasswordChars { get; init; }
        public UserValidator()
        {
            validPasswordChars = ValidCharsHashSetSingleton.GetInstance();

            const int UsernameMinimumLength = 6;
            const int UsernameMaximumLength = 48;
            RuleFor(user => user.Name).NotNull()
                                      .MinimumLength(UsernameMinimumLength).WithMessage($"Username was too short. Minimum length {UsernameMinimumLength}.")
                                      .MaximumLength(UsernameMaximumLength).WithMessage($"Username was too long. Maximum length {UsernameMaximumLength}.");

            const int PasswordMinimumLength = 12;
            const int PasswordMaximumLength = 60;
            RuleFor(user => user.Password).NotNull()
                                          .MinimumLength(PasswordMinimumLength).WithMessage($"Password was too short. Minimum length {PasswordMinimumLength}.")
                                          .MaximumLength(PasswordMaximumLength).WithMessage($"Password was too long. Maximum length {PasswordMaximumLength}.")
                                          .Must(pass => pass.All(validPasswordChars.Contains)).WithMessage($"Cam only have alphanumeric characters and the following symbols: '{string.Concat(validPasswordChars.Where(c => !char.IsAsciiLetterOrDigit(c)))}'");
        }
    }

    public class ValidCharsHashSetSingleton : HashSet<char>
    {
        private const string ValidPasswordCharacters = """abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 "~`!@#$%^&*()_-+={}[]|\:;'<,>.?/""";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static ValidCharsHashSetSingleton instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static InstanceGetter GetInstance { get; private set; } = GetAndSetInstance;

        public delegate ValidCharsHashSetSingleton InstanceGetter();

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

        private static ValidCharsHashSetSingleton OnlyGetInstance()
        {
            return instance;
        }

        private ValidCharsHashSetSingleton()
        {
        }
    }
}
