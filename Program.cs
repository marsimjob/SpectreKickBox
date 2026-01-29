using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using SpectreKickBox.Data;
using SpectreKickBox.Services;


// Load configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("C:\\Users\\mario\\source\\repos\\SpectreKickBox\\appsettings.json")
    .Build();

// Setup DbContext
var options = new DbContextOptionsBuilder<KickBoxingClubContext>()
    .UseSqlServer(config.GetConnectionString("DefaultConnection"))
    .Options;

// Services
var memberService = new MemberService(new KickBoxingClubContext(options));
var sessionService = new SessionService(new KickBoxingClubContext(options));
var adminService = new AdminService(new KickBoxingClubContext(options));

bool running = true;
while (running)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Kickboxing Club[/]")
            .AddChoices(
                "Se Nyheter",
                "Se Hela Träningsschemat",
                "Mitt Medlemskap (login)",
                "Admin Panel (login)",
                "Avsluta"));

    switch (choice)
    {
        case "Se Nyheter":
            memberService.ShowNews();
            break;
        case "Se Hela Träningsschemat":
            sessionService.ShowWeeklySessions();
            break;
        case "Mitt Medlemskap (login)":
            var email = AnsiConsole.Ask<string>("Ange din Email för att logga in:");

            // We fetch the account here to verify credentials
            using (var context = new KickBoxingClubContext(options))
            {
                var account = context.Account
                    .Include(a => a.AppUser)
                    .Include(a => a.Role)
                    .FirstOrDefault(a => a.Email == email);

                if (account != null)
                {
                    // Login success - Enter the Member Menu
                    memberService.MemberMenu(account);
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Inloggning misslyckades. Fel email.[/]");
                    Thread.Sleep(2000);
                }
            }
            break;
        case "Admin Panel (login)":
            adminService.LoginAndAdminTasks();
            break;
        case "Avsluta":
            running = false;
            break;
    }
}
