using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using SpectreKickBox.Data;
using SpectreKickBox.Services;
using System.Text;

// Tvinga konsolen att tolka UTF-8 (viktigt för emojis och specialtecken)
Console.OutputEncoding = Encoding.UTF8;

// Load configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("C:\\Users\\Senkai\\source\\repos\\SpectreKickBox\\appsettings.json")
    .Build();

// Setup DbContext
var options = new DbContextOptionsBuilder<KickBoxingClubContext>()
    .UseSqlServer(config.GetConnectionString("DefaultConnection"))
    .Options;

// Services
var memberService = new MemberService(new KickBoxingClubContext(options));
var sessionService = new SessionService(new KickBoxingClubContext(options));
var adminService = new AdminService(new KickBoxingClubContext(options));
var registerService = new RegisterService(new KickBoxingClubContext(options));

PlayKickAnimation();
bool running = true;

while (running)
{
    SessionService.ShowHeader(Spectre.Console.Color.Green, "[bold white]🥊 KICKSTART YOUR KICKBOXING! 🥊[/]");

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[grey]Välkommen till klubben! Vad vill du göra?[/]")
            .AddChoices(new[] {
                "📰 Se Nyheter",
                "🗓️ Se Hela Träningsschemat",
                "🥋 Bli medlem",
                "👤 Mitt Medlemskap (login)",
                "🛡️ Admin Panel (login)",
                "❌ Avsluta"
            }));

    switch (choice)
    {
        case "📰 Se Nyheter":
            memberService.ShowNews();
            break;
        case "🗓️ Se Hela Träningsschemat":
            sessionService.ShowWeeklySessions();
            break;
        case "👤 Mitt Medlemskap (login)":
            var email = AnsiConsole.Ask<string>("[yellow]Ange din Email för att logga in:[/]");

            using (var context = new KickBoxingClubContext(options))
            {
                // Match: .Accounts, .User, .Role
                var account = context.Accounts
                    .Include(a => a.User)
                    .Include(a => a.Role)
                    .FirstOrDefault(a => a.Email == email);

                if (account != null)
                {
                    AnsiConsole.MarkupLine("[green]Inloggning lyckades! Välkommen tillbaka.[/] :victory_hand:");
                    Thread.Sleep(1000);
                    memberService.MemberMenu(account);
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Inloggning misslyckades. Fel email.[/] :warning:");
                    Thread.Sleep(2000);
                }
            }
            break;

        case "🛡️ Admin Panel (login)":
            adminService.LoginAndAdminTasks();
            break;
        case "🥋 Bli medlem":
            registerService.Register();
            break;
        case "❌ Avsluta":
            AnsiConsole.MarkupLine("[yellow]Ses på nästa träning! Hejdå![/] :wave:");
            running = false;
            break;
    }
}

static void PlayKickAnimation()
{
    // Frame 1: Distans
    string far = @"⠀⢀⣶⣿⣿⣷⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣾⣿⣿⣶⡀⠀
⠀⠘⣿⣿⣿⣿⠇⠀⠀⠀⠀⠀⠀⢀⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡀⠀⠀⠀⠀⠀⠀⠸⣿⣿⣿⣿⠃⠀
⠀⠀⠈⠙⠋⢁⣀⣠⣤⣀⣀⣀⣰⣿⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣆⣀⣀⣀⣤⣄⣀⡈⠙⠋⠁⠀⠀
⠀⠀⣀⣴⣾⣿⣿⣿⣿⣿⠿⠿⠿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠿⠿⠿⣿⣿⣿⣿⣿⣷⣦⣀⠀⠀
⢠⣾⣿⣿⢿⣿⣿⣿⣿⣧⠀⠀⠀⣀⣤⣴⣾⣿⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣷⣦⣤⣀⠀⠀⠀⣼⣿⣿⣿⣿⢿⣿⣿⣷⡄
⠘⣿⣇⠀⠈⢻⣿⣿⣿⣿⣷⣶⣿⣿⣿⡿⠛⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠛⢿⣿⣿⣿⣶⣶⣿⣿⣿⣿⡟⠁⠀⣸⣿⠃
⠀⢻⣿⡆⠀⠀⣿⣿⣿⣿⣿⣿⠿⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠿⣿⣿⣿⣿⣿⣿⡿⠀⠀⢰⣿⡟⠀
⠀⠈⠛⠁⠀⠀⢹⣿⣿⡟⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢻⣿⣿⡏⠀⠀⠀⠈⠛⠁⠀
⠀⠀⠀⠀⠀⠀⢸⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠘⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⠇⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⢿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣿⡿⠀⠀⠀⠀⠀⠀⠀";

    // Frame 2: Nära (Face-off)
    string close = @"⠀⢀⣶⣿⣿⣷⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣾⣿⣿⣶⡀⠀
⠀⠘⣿⣿⣿⣿⠇⠀⠀⠀⠀⢀⣤⡀⠀⠀⠀⠀⠀⠀⠸⣿⣿⣿⣿⠃⠀
⠀⠀⠈⠙⠋⢁⣀⣠⣤⣀⣰⣿⠟⠁⠻⣿⣆⣀⣤⣄⣀⡈⠙⠋⠁⠀⠀
⠀⠀⣀⣴⣾⣿⣿⣿⣿⠿⠿⠋⠀⠀⠀⠙⠿⠿⣿⣿⣿⣿⣿⣷⣦⣀⠀
⢠⣾⣿⣿⢿⣿⣿⣿⣿⣧⠀⣀⣤⣴⢿⣿⣷⣦⣀⠀⣼⣿⣿⣿⣿⢿⣿
⠘⣿⣇⠀⠈⢻⣿⣿⣿⣿⣶⣿⣿⡿⠉⠛⢿⣿⣿⣶⣿⣿⣿⡟⠁⣸⣿
⠀⢻⣿⡆⠀⠀⣿⣿⣿⣿⣿⠿⠋⠀⠀⠀⠀⠙⠿⣿⣿⣿⣿⡿⠀⢰⣿
⠀⠈⠛⠁⠀⠀⢹⣿⣿⡟⠋⠀⠀⠀⠀⠀⠀⠀⠀⠙⢻⣿⣿⡏⠀⠈⠛
⠀⠀⠀⠀⠀⠀⢸⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠘⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⠇⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⢿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣿⡿⠀⠀⠀⠀";

    // Frame 3: Kicken
    string kick = @"⠀⠀⠀⢀⣤⣶⣿⣿⣿⣷⣶⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣶⣾⣿⣷⣶⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠹⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠈⠛⠿⣿⣿⣿⣿⠿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠀⣠⣤⣤⣤⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⣀⣤⣤⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠿⣿⣿⣿⣿⣿⠿⠋⠀⣚⣫⣭⣶⣮⡝⣿⣷⣄⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⢀⣴⣿⣿⠿⠿⣿⣿⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣭⣤⣶⣶⣿⣿⣿⣿⣿⣿⣿⠇⣿⣿⣿⣷⣄⠀⠀⠀⠀⠀⠀
⠀⠀⣰⣿⣿⡟⣱⣿⣿⣦⡙⣿⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣤⣶⣶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⣛⣥⣾⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀
⠀⣰⣿⣿⣿⣧⢻⣿⣿⣿⣿⣌⢿⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠿⢛⣛⣭⣵⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣆⠀⠀⠀
⢠⣿⣿⣿⣿⣿⣎⢿⣿⣿⣿⣿⣷⣥⣀⣀⣀⣀⣤⣤⣤⣤⡀⠀⠀⠀⠀⠀⠙⣛⣛⣛⣋⣭⣭⣥⣶⣶⡿⠿⠟⠛⠋⠉⠀⠙⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⠀⠀
⣼⣿⣿⣿⣿⣿⣿⣦⡙⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡷⠀⠀⠀⠀⠀⠀⠉⠙⠛⠛⠉⠉⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⠀
⣿⣿⣿⣿⠹⣿⣿⣿⣿⣶⣭⣙⡻⠿⠿⠿⠿⠿⠿⠿⠿⠛⠁⠀⠀⠀⠀⣀⣀⣤⣤⣴⣶⣶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣦⡀⠀⠀⠀⠙⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆
⣿⣿⣿⣿⡀⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⡄⠀⠀⠀⠀⢀⣀⣤⣴⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⢈⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇
⢹⣿⣿⣿⣧⠀⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣄⣤⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠟⠁⠀⠀⣠⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠇
⠈⢿⣿⣿⣿⠆⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠿⠛⠛⠛⠉⠉⠉⠉⠉⠀⠀⠀⣠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠃⠀
⠀⠈⠛⠛⠋⠀⠀⠀⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠛⠋⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣾⣿⣿⣿⣿⣿⣿⣿⡿⠛⠉⠉⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠻⣿⣿⣿⣿⣿⣿⣿⣿⡿⠟⠋⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣾⣿⣿⣿⣿⣿⣿⣿⡿⠋⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⣿⣿⣿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⣿⣿⣿⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣿⣿⣿⣿⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣿⡿⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀";

    Console.CursorVisible = false;

    // Animera fram och tillbaka 3 gånger
    for (int i = 0; i < 3; i++)
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold red]ARE YOU READY?[/]");
        Console.WriteLine(far);
        Thread.Sleep(400);

        Console.Clear();
        AnsiConsole.MarkupLine("[bold yellow]FACE TO FACE![/]");
        Console.WriteLine(close);
        Thread.Sleep(400);
    }

    // FINAL KICK
    Console.Clear();
    AnsiConsole.MarkupLine("[bold red]K.O.![/]");
    AnsiConsole.Write(new Markup($"[bold red]{kick}[/]")); // Röd färg för extra kraft

    Thread.Sleep(2000); // Håll i 2 sekunder

    Console.CursorVisible = true;
    Console.Clear();
}