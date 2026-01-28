using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Data;
using SpectreKickBox.Models; // Ensure this is added to access your View models
using Microsoft.Extensions.Configuration;
public class AdminService
{
    private readonly KickBoxingClubContext _context;

    public AdminService(KickBoxingClubContext context)
    {
        _context = context;
    }

    public void LoginAndAdminTasks()
    {
        var email = AnsiConsole.Ask<string>("Admin Email:");

        // Improved query to handle potential nulls and case sensitivity
        var userAcct = _context.Account
            .Include(a => a.Role)
            .FirstOrDefault(a => a.Email == email && (a.Role.Title == "Staff" || a.Role.Title == "Trainer"));

        if (userAcct == null)
        {
            AnsiConsole.MarkupLine("[red]Admin login failed! Account not found or insufficient permissions.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Välkommen, {email}![/]");

        bool adminMenu = true;
        while (adminMenu)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[yellow]Admin Tasks[/]")
                .PageSize(10)
                .AddChoices("Visa Revenue", "Radera Medlem", "Tillbaka"));

            switch (choice)
            {
                case "Visa Revenue":
                    ShowRevenue();
                    break;
                case "Radera Medlem":
                    DeleteMember();
                    break;
                case "Tillbaka":
                    adminMenu = false;
                    break;
            }
        }
    }

    public void ShowRevenue()
    {
        //// Add a safety check in case the view returns no rows
        //var rev = _context.vw_TotalInvoiceRevenue.FirstOrDefault();

        //if (rev != null)
        //{
        //    var panel = new Panel(new Rows(
        //        new Markup($"[bold green]Totalt Revenue:[/] {rev.Revenue} kr"),
        //        new Markup($"[bold blue]Antal Fakturor:[/] {rev.NumberOfInvoices}")
        //    ));
        //    panel.Header = new PanelHeader("Ekonomisk Översikt");
        //    panel.Border = BoxBorder.Rounded;

        //    AnsiConsole.Write(panel);
        //}
        //else
        //{
        //    AnsiConsole.MarkupLine("[yellow]Ingen revenue-data tillgänglig för tillfället.[/]");
        //}

        //AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
        //Console.ReadKey(true);
    }

    public void DeleteMember()
    {
        int id = AnsiConsole.Ask<int>("AccountID att ta bort?");
        var acc = _context.Account.Find(id);

        if (acc != null)
        {
            // Confirmation prompt to prevent accidental deletion
            if (AnsiConsole.Confirm($"Är du säker på att du vill radera konto [red]{id}[/]?"))
            {
                _context.Account.Remove(acc);
                _context.SaveChanges();
                AnsiConsole.MarkupLine("[green]Kontot har raderats![/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Konto med ID {id} hittades inte.[/]");
        }
    }
}