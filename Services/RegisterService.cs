using SpectreKickBox.Data;
using Spectre.Console;
using SpectreKickBox.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BCrypt;

namespace SpectreKickBox.Services
{
    public class RegisterService
    {
        private readonly KickBoxingClubContext _context;

        public RegisterService(KickBoxingClubContext context)
        {
            _context = context;
        }

        private static Spectre.Console.ValidationResult ValidatePasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
                return Spectre.Console.ValidationResult.Error("Password cannot be empty");

            if (pw.Length < 8)
                return Spectre.Console.ValidationResult.Error("Password must be at least 8 characters");

            if (!pw.Any(char.IsLower))
                return Spectre.Console.ValidationResult.Error("Must contain a lowercase letter");

            if (!pw.Any(char.IsUpper))
                return Spectre.Console.ValidationResult.Error("Must contain an uppercase letter");

            if (!pw.Any(char.IsDigit))
                return Spectre.Console.ValidationResult.Error("Must contain a number");

            if (!pw.Any(c => !char.IsLetterOrDigit(c)))
                return Spectre.Console.ValidationResult.Error("Must contain a symbol");

            return Spectre.Console.ValidationResult.Success();
        }


        public void Register()
        {
            var firstName = AnsiConsole.Ask<string>("What's your name?: ");
            var lastName = AnsiConsole.Ask<string>("What's your lastname?: ");

            var dateOfBirth = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Input your [green]date of birth[/] (yyyy-mm-dd):")
                    .Validate(dob =>
                        dob > DateTime.Today
                            ? Spectre.Console.ValidationResult.Error("[red]Date of birth cannot be in the future[/]")
                            : Spectre.Console.ValidationResult.Success()
                    )
            );

            var emailValidator = new EmailAddressAttribute();

            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter an email for your account: ")
                    .Validate(mail => emailValidator.IsValid(mail)
                        ? Spectre.Console.ValidationResult.Success()
                        : Spectre.Console.ValidationResult.Error("[red]Invalid email address[/]"))
            );

            if (_context.Account.Any(a => a.Email == email))
            {
                AnsiConsole.MarkupLine("[red]That email is already registered.[/]");
                return;
            }

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter your [green]password[/]:")
                    .Secret()
                    .Validate(ValidatePasswordStrength));

            var confirm = AnsiConsole.Prompt(
                new TextPrompt<string>("Confirm your [green]password[/]:")
                    .Secret()
            );

            if (password != confirm)
            {
                AnsiConsole.MarkupLine("[red]Passwords do not match.[/]");
                return;
            }

            var user = new AppUser
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                DateOfBirth = dateOfBirth.Date,
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

            var account = new Account
            {
                Email = email,
                PasswordHash = passwordHash,
                RoleID = 3
               
            };

            using var tx = _context.Database.BeginTransaction();

            try
            {
                _context.Add(user);
                _context.SaveChanges();          

                account.UserID = user.UserID;    

                _context.Add(account);
                _context.SaveChanges();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }

            AnsiConsole.MarkupLine(
                $"[green]User[/] [blue]{user.FirstName} {user.LastName}[/] (ID: [yellow]{user.UserID}[/] with the email [blue]{account.Email}[/] has been added to the database!)"
            );
            
        }

    }
}
