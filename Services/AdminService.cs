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
        AnsiConsole.Clear(); //clear the console 
        var email = AnsiConsole.Ask<string>("Admin Email:");

        // Fetch the account including the AppUser to get the name
        var userAcct = _context.Account
            .Include(a => a.Role)
            .Include(a => a.AppUser)
            .FirstOrDefault(a => a.Email == email && (a.Role.Title == "Staff" || a.Role.Title == "Trainer"));

        if (userAcct == null)
        {
            AnsiConsole.MarkupLine("[red]Admin login failed! Account not found or insufficient permissions.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Välkommen, {userAcct.AppUser.FirstName} {userAcct.AppUser.LastName}![/]");

        bool adminMenu = true;
        while (adminMenu)
        {
            AnsiConsole.Clear();

            AnsiConsole.Write(
                new FigletText("ADMIN MENU")
                    .Centered()
                    .Color(Color.Yellow)
            );

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .AddChoices(new[]
            {
                "Skapa Träningspass",
                "Rensa Träningsvecka",
                "Skriv Nyhetsbrev",
                "Rensa månadsgamla nyhetsbrev",
                "Se alla fakturor",
                "Skicka Faktura",
                "Visa Revenue",
                "Rapport: Top kunder (flest fakturor)",
                "Rapport: Medlemskap går ut inom 14 dagar",
                "Radera Medlem",
                "Tillbaka"
            }));

            switch (choice)
            {
                case "Skapa Träningspass":
                    AddWorkoutSession();
                    break;

                case "Rensa Träningsvecka":
                    DeleteAllSessions();
                    break;

                case "Skriv Nyhetsbrev":
                    WriteNewsletter(userAcct.AccountID, userAcct.AppUser.FirstName);
                    break;

                case "Rensa månadsgamla nyhetsbrev":
                    CleanOldNewsletters();
                    break;

                case "Se alla fakturor":
                    OverseeInvoices();
                    break;

                case "Skicka Faktura":
                    CreateAndSendInvoice();
                    break;

                case "Visa Revenue":
                    ShowRevenue();
                    break;

                case "Rapport: Top kunder (flest fakturor)":
                    ReportTopCustomers();
                    break;

                case "Rapport: Medlemskap går ut inom 14 dagar":
                    ReportExpiringMemberships();
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
    public void DeleteAllSessions() 

    {    AnsiConsole.Clear(); //clear the console
        var count = _context.Session.Count();

        if (count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Det finns redan 0 sessioner i databasen.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[bold red]VARNING:[/] Du är på väg att radera ALLA [bold]{count}[/] pass i schemat.");

        if (AnsiConsole.Confirm("Vill du verkligen fortsätta? Detta kan inte ångras."))
        {
            var allSessions = _context.Session.ToList();
            _context.Session.RemoveRange(allSessions); // Vi hämtar alla och använder RemoveRange
            _context.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Tabellen har rensats. {count} pass raderades.[/]");
        }
    }
    public void AddWorkoutSession()

    {   AnsiConsole.Clear(); //clear the console
        AnsiConsole.MarkupLine("[bold yellow]SKAPA NYTT TRÄNINGSPASS[/]");

        var trainers = _context.Account
            .Include(a => a.AppUser)
            .Where(a => a.Role.Title == "Trainer" || a.Role.Title == "Staff")
            .ToList();
        var trainerChoice = AnsiConsole.Prompt(
            new SelectionPrompt<Account>()
                .Title("Välj [green]Tränare[/]:")
                .UseConverter(a => $"{a.AppUser.FirstName} {a.AppUser.LastName}")
                .AddChoices(trainers));
        var types = _context.SessionType.ToList();
        var typeChoice = AnsiConsole.Prompt(
            new SelectionPrompt<SessionType>()
                .Title("Välj [green]Passtyp[/]:")
                .UseConverter(t => t.TypeTitle)
                .AddChoices(types));
        var days = _context.WeekDay.OrderBy(d => d.DayID).ToList();
        var dayChoice = AnsiConsole.Prompt(
            new SelectionPrompt<WeekDay>()
                .Title("Välj [green]Dag[/]:")
                .UseConverter(d => d.DayName)
                .AddChoices(days));
        var startTime = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Välj [green]Starttid[/]:")
                .AddChoices(new[] { "08:00", "09:30", "11:00", "17:00", "18:30", "20:00" }));

        var duration = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Välj [green]Längd[/] (minuter):")
                .AddChoices(new[] { 60, 75, 90 }));
        var focus = AnsiConsole.Ask<string>("Ange [green]Fokus[/] för passet (t.ex. 'Sparring', 'Teknik'):");
        int currentWeek = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            DateTime.Now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var newSession = new Session
        {
            TrainerID = trainerChoice.AccountID,
            SessionTypeID = typeChoice.SessionTypeID,
            DayID = dayChoice.DayID,
            StartTime = TimeSpan.Parse(startTime),
            Duration = duration,
            Focus = focus,
            Capacity = 30, // Default enligt krav
            SessionWeek = currentWeek
        };
        _context.Session.Add(newSession);
        _context.SaveChanges();

        AnsiConsole.MarkupLine($"[green]Succé![/] Passet '[bold]{typeChoice.TypeTitle}[/]' med [bold]{trainerChoice.AppUser.FirstName}[/] är nu skapat för vecka {currentWeek}.");
    }
    public void OverseeInvoices()
    {
        AnsiConsole.Clear(); //clear the console
        var invoices = _context.Invoice
    .Include(i => i.Account)
        .ThenInclude(a => a.AppUser) // This reaches the AppUser table
    .Include(i => i.MembershipPlan)
    .OrderByDescending(i => i.InvoiceID)
    .AsNoTracking()
    .ToList();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]SPECTRE KICKBOXING - FAKTURAÖVERSIKT[/]");

        table.AddColumn("[bold]Medlem[/]"); // Our SQL Alias
        table.AddColumn("[bold]ID[/]");
        table.AddColumn("[bold]Skickat[/]");

        foreach (var i in invoices)
        {
            // Reflection of: u.FirstName + ' ' + u.LastName
            string medlem = i.Account.AppUser != null
                ? $"{i.Account.AppUser.FirstName} {i.Account.AppUser.LastName}"
                : "Okänd Användare";

            table.AddRow(
                $"[blue]{medlem}[/]",
                i.InvoiceID.ToString(),
                i.InvoiceDate.ToString("yyyy-MM-dd")
            );
        }

        AnsiConsole.Write(table);
        Console.ReadKey();
    }
    public void WriteNewsletter(int accountId, string authorName)
    {   AnsiConsole.Clear(); //clear the console
        AnsiConsole.MarkupLine($"[yellow]Skriv och posta ett nyhetsbrev som:[/] [blue]{authorName}[/]");

        // Ask for NewsType using a prompt for better UX
        string type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Välj [green]Kategori[/]:")
                .AddChoices(new[] { "Information", "Träning", "Event", "Viktigt" }));

        string title = AnsiConsole.Ask<string>("Ange [green]Titel[/]:");
        string content = AnsiConsole.Ask<string>("Ange [green]Innehåll[/]:");

        int weekNum = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            DateTime.Now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var newsletter = new Newsletter
        {
            NewsTitle = title,
            NewsContent = content,
            NewsType = type,
            PostedByAccountID = accountId,
            IsActive = true,
            PostYear = DateTime.Now.Year,
            PostWeek = weekNum
        };

        _context.Newsletter.Add(newsletter);
        _context.SaveChanges();

        AnsiConsole.MarkupLine("[green]Nyhetsbrevet har postats![/]");
    }
    public void ShowRevenue() 
        {  AnsiConsole.Clear(); //clear the console
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

    public void ReportTopCustomers()
    {
        AnsiConsole.Clear();

        var data = _context.Invoice
            .GroupBy(i => i.AccountID)
            .Select(g => new
            {
                AccountID = g.Key,
                TotalInvoices = g.Count()
            })
            .OrderByDescending(x => x.TotalInvoices)
            .Take(10)
            .Join(
                _context.Account.Include(a => a.AppUser),
                x => x.AccountID,
                a => a.AccountID,
                (x, a) => new
                {
                    a.AccountID,
                    Name = a.AppUser.FirstName + " " + a.AppUser.LastName,
                    TotalInvoices = x.TotalInvoices
                })
            .ToList();

        if (!data.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Inga fakturor hittades.[/]");
            Console.ReadKey(true);
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]TOP KUNDER – FLEST FAKTUROR[/]");

        table.AddColumn("AccountID");
        table.AddColumn("Medlem");
        table.AddColumn("Antal fakturor");

        foreach (var r in data)
        {
            table.AddRow(
                r.AccountID.ToString(),
                r.Name,
                r.TotalInvoices.ToString()
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
        Console.ReadKey(true);
    }

    public void ReportExpiringMemberships()
    {
        AnsiConsole.Clear();

        var limitDate = DateTime.Today.AddDays(14);

        var memberships = _context.Membership
            .Include(m => m.AppUser)
            .Include(m => m.MembershipPlan)
                .ThenInclude(mp => mp.PriceList)
            .Where(m => m.IsActive && m.EndDate <= limitDate)
            .OrderBy(m => m.EndDate)
            .ToList();

        if (!memberships.Any())
        {
            AnsiConsole.MarkupLine("[green]Inga medlemskap går ut inom 14 dagar.[/]");
            Console.ReadKey(true);
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]MEDLEMSKAP SOM GÅR UT INOM 14 DAGAR[/]");

        table.AddColumn("Medlem");
        table.AddColumn("Plan");
        table.AddColumn("Slutdatum");
        table.AddColumn("Dagar kvar");
        table.AddColumn("Pris");

        foreach (var m in memberships)
        {
            int daysLeft = (m.EndDate.Date - DateTime.Today).Days;

            table.AddRow(
                $"{m.AppUser.FirstName} {m.AppUser.LastName}",
                m.MembershipPlan.BillingPeriod,
                m.EndDate.ToString("yyyy-MM-dd"),
                daysLeft.ToString(),
                $"{m.MembershipPlan.PriceList.Amount} kr"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
        Console.ReadKey(true);
    }

    public void CleanOldNewsletters() 
    {    AnsiConsole.Clear(); //clear the console 
        var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        DateTime now = DateTime.Now;

        int currentWeek = cal.GetWeekOfYear(now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        int currentYear = now.Year;

        // Vi sätter gränsen vid 4 veckor sedan
        int thresholdWeek = currentWeek - 4;
        int thresholdYear = currentYear;

        // Hantera om 4 veckor sedan var förra året
        if (thresholdWeek <= 0)
        {
            thresholdYear -= 1;
            int lastYearMaxWeek = cal.GetWeekOfYear(new DateTime(thresholdYear, 12, 31), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            thresholdWeek = lastYearMaxWeek + thresholdWeek; // t.ex. 52 + (-2) = vecka 50
        }

        // Vi raderar allt som är ÄLDRE än tröskelvärdet
        var newsToDelete = _context.Newsletter
            .Where(n => n.PostYear < thresholdYear ||
                       (n.PostYear == thresholdYear && n.PostWeek < thresholdWeek))
            .ToList();

        if (newsToDelete.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Inga gamla nyhetsbrev (4 veckor+) hittades.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[bold red]ARKIVRENSNING:[/] Hittade [bold]{newsToDelete.Count}[/] nyhetsbrev som är äldre än 4 veckor.");

        if (AnsiConsole.Confirm("Vill du radera dessa gamla inlägg?"))
        {
            _context.Newsletter.RemoveRange(newsToDelete);
            _context.SaveChanges();
            AnsiConsole.MarkupLine($"[green]Rensat![/] {newsToDelete.Count} gamla nyhetsbrev raderades.");
        }
    }

    public void CreateAndSendInvoice()
    {
        AnsiConsole.Clear(); //clear the console
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
    {    AnsiConsole.Clear(); //clear the console 
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