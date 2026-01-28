using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using SpectreKickBox.Data;

// Load configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("D:\\Handelsakademin\\C++\\DynamicMemory\\SpectreKickBox\\appsettings.json")
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
                "Se Träningsschema",
                "Mitt Medlemskap (login)",
                "Admin Panel (login)",
                "Avsluta"));

    switch (choice)
    {
        case "Se Nyheter":
            memberService.ShowNews();
            break;
        case "Se Träningsschema":
            sessionService.ShowWeeklySessions();
            break;
        case "Mitt Medlemskap (login)":
            memberService.LoginAndShowDashboard();
            break;
        case "Admin Panel (login)":
            adminService.LoginAndAdminTasks();
            break;
        case "Avsluta":
            running = false;
            break;
    }
}
