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
                .AddChoices("Skicka Faktura", "Visa Revenue", "Radera Medlem", "Tillbaka"));

            switch (choice)
            {
                case "Skicka Faktura":
                    CreateAndSendInvoice();
                    break;
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
        // Add a safety check in case the view returns no rows
        var rev = _context.vw_TotalInvoiceRevenue.FirstOrDefault();

        if (rev != null)
        {
            var panel = new Panel(new Rows(
                new Markup($"[bold green]Totalt Revenue:[/] {rev.Revenue} kr"),
                new Markup($"[bold blue]Antal Fakturor:[/] {rev.NumberOfInvoices}")
            ));
            panel.Header = new PanelHeader("Ekonomisk Översikt");
            panel.Border = BoxBorder.Rounded;

            AnsiConsole.Write(panel);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Ingen revenue-data tillgänglig för tillfället.[/]");
        }

        AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
        Console.ReadKey(true);
    }
    public void CreateAndSendInvoice()
    {
        AnsiConsole.MarkupLine("[yellow]Skapa nya fakturor för medlemmar[/]");

        var invoicesToCreate = new List<Invoice>();
        bool addingInvoices = true;

        while (addingInvoices)
        {
            int accountId = AnsiConsole.Ask<int>("Ange [green]AccountID[/] för faktura:");

            // Fetch active membership and price for the user
            var accountInfo = _context.Account
                .Where(a => a.AccountID == accountId)
                .Select(a => new
                {
                    FullName = a.AppUser.FirstName + " " + a.AppUser.LastName,
                    MembershipPlanID = a.AppUser.Membership
                        .Where(m => m.IsActive)
                        .Select(m => m.MembershipPlanID)
                        .FirstOrDefault(),
                    Price = a.AppUser.Membership
                        .Where(m => m.IsActive)
                        .Select(m => m.MembershipPlan.PriceList.Amount)
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            if (accountInfo == null || accountInfo.Price == 0 || accountInfo.MembershipPlanID == 0)
            {
                AnsiConsole.MarkupLine("[red]Ingen aktiv medlemskap eller pris hittades för detta konto.[/]");
                continue;
            }

            // Show confirmation before creating invoice
            AnsiConsole.MarkupLine($"Skapar faktura för: [green]{accountInfo.FullName}[/], Belopp: [green]{accountInfo.Price} kr[/]");
            if (!AnsiConsole.Confirm("Vill du fortsätta med denna faktura?"))
                continue;

            invoicesToCreate.Add(new Invoice
            {
                AccountID = accountId,
                MembershipPlanID = accountInfo.MembershipPlanID,
                InvoiceDate = DateTime.Now
            });

            addingInvoices = AnsiConsole.Confirm("Vill du lägga till en till faktura?");
        }

        if (invoicesToCreate.Count > 0)
        {
            _context.Invoice.AddRange(invoicesToCreate);
            _context.SaveChanges();
            AnsiConsole.MarkupLine($"[green]{invoicesToCreate.Count} fakturor skapades och skickades![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Inga fakturor skapades.[/]");
        }

        AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
        Console.ReadKey(true);
    }


    public void DeleteMember()
    {
        int id = AnsiConsole.Ask<int>("AccountID att ta bort?");

        // 1. Fetch the account AND the associated AppUser data
        var acc = _context.Account
            .Include(a => a.AppUser) // This joins the AppUser table
            .FirstOrDefault(a => a.AccountID == id);

        if (acc == null)
        {
            AnsiConsole.MarkupLine($"[red]Konto med ID {id} hittades inte.[/]");
            return;
        }

        // 2. Create a friendly name string
        string fullName = $"{acc.AppUser.FirstName} {acc.AppUser.LastName}";

        // 3. Updated Confirmation Prompt
        if (AnsiConsole.Confirm($"Är du säker på att du vill radera [bold]{fullName}[/] (AccountID: [yellow]{id}[/])?"))
        {
            // Clear related data to avoid foreign key errors
            var relatedInvoices = _context.Invoice.Where(i => i.AccountID == id);
            _context.Invoice.RemoveRange(relatedInvoices);

            var relatedNewsletters = _context.Newsletter.Where(n => n.PostedByAccountID == id);
            _context.Newsletter.RemoveRange(relatedNewsletters);

            var relatedSessions = _context.Session.Where(s => s.TrainerID == id);
            _context.Session.RemoveRange(relatedSessions);

            // Finally, remove the account
            _context.Account.Remove(acc);

            _context.SaveChanges();
            AnsiConsole.MarkupLine($"[green]Kontot för {fullName} har raderats![/]");
        }
    }
}