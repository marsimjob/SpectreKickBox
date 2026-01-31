using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Data;
using SpectreKickBox.Models;
using Microsoft.Extensions.Configuration;
namespace SpectreKickBox.Services
{
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

            // Match: .Accounts, .User, and .AccountId
            var userAcct = _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.User)
                .FirstOrDefault(a => a.Email == email && (a.Role.Title == "Staff" || a.Role.Title == "Trainer"));

            if (userAcct == null)
            {
                AnsiConsole.MarkupLine("[red]Admin login failed! Account not found or insufficient permissions.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]Välkommen, {userAcct.User.FirstName} {userAcct.User.LastName}![/]");

            bool adminMenu = true;
            while (adminMenu)
            {
                SessionService.ShowHeader(Spectre.Console.Color.Red, "[bold white]🥊 CLUB ADMINISTRATION SYSTEM 🥊[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("[yellow]Välj administrativ uppgift:[/]")
                    .PageSize(12)
                    .AddChoices(new[] {
                    "➕ Skapa Träningspass",
                    "🧹 Rensa Träningsvecka",
                    "📝 Skriv Nyhetsbrev",
                    "🗑️ Rensa gamla nyhetsbrev",
                    "📑 Se alla fakturor",
                    "💳 Skicka Faktura",
                    "💰 Visa Revenue",
                    "❌ Radera Medlem",
                    "🔙 Tillbaka"
                    }));

                switch (choice)
                {
                    case "➕ Skapa Träningspass":
                        AddWorkoutSession();
                        break;
                    case "🧹 Rensa Träningsvecka":
                        DeleteAllSessions();
                        break;
                    case "📝 Skriv Nyhetsbrev":
                        WriteNewsletter(userAcct.AccountId, userAcct.User.FirstName);
                        break;
                    case "🗑️ Rensa gamla nyhetsbrev":
                        CleanOldNewsletters();
                        break;
                    case "📑 Se alla fakturor":
                        OverseeInvoices();
                        break;
                    case "💳 Skicka Faktura":
                        CreateAndSendInvoice();
                        break;
                    case "💰 Visa Revenue":
                        ShowRevenue();
                        break;
                    case "❌ Radera Medlem":
                        DeleteMember();
                        break;
                    case "🔙 Tillbaka":
                        adminMenu = false;
                        break;
                }
            }
        }

        public void DeleteAllSessions()
        {
            var count = _context.Sessions.Count();

            if (count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Det finns redan 0 sessioner i databasen.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[bold red]VARNING:[/] Du är på väg att radera ALLA [bold]{count}[/] pass i schemat.");

            if (AnsiConsole.Confirm("Vill du verkligen fortsätta? Detta kan inte ångras."))
            {
                var allSessions = _context.Sessions.ToList();
                _context.Sessions.RemoveRange(allSessions);
                _context.SaveChanges();

                AnsiConsole.MarkupLine($"[green]Tabellen har rensats. {count} pass raderades.[/]");
            }
        }

        public void AddWorkoutSession()
        {
            AnsiConsole.MarkupLine("[bold yellow]SKAPA NYTT TRÄNINGSPASS[/]");

            var trainers = _context.Accounts
                .Include(a => a.User)
                .Where(a => a.Role.Title == "Trainer" || a.Role.Title == "Staff")
                .ToList();

            var trainerChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Account>()
                    .Title("Välj [green]Tränare[/]:")
                    .UseConverter(a => $"{a.User.FirstName} {a.User.LastName}")
                    .AddChoices(trainers));

            var types = _context.SessionTypes.ToList();
            var typeChoice = AnsiConsole.Prompt(
                new SelectionPrompt<SessionType>()
                    .Title("Välj [green]Passtyp[/]:")
                    .UseConverter(t => t.TypeTitle)
                    .AddChoices(types));

            var days = _context.WeekDays.OrderBy(d => d.DayId).ToList();
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
                TrainerId = trainerChoice.AccountId,
                SessionTypeId = typeChoice.SessionTypeId,
                DayId = dayChoice.DayId,
                StartTime = TimeOnly.Parse(startTime),
                Duration = duration,
                Focus = focus,
                Capacity = 30,
                SessionWeek = currentWeek
            };

            _context.Sessions.Add(newSession);
            _context.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Succé![/] Passet '[bold]{typeChoice.TypeTitle}[/]' med [bold]{trainerChoice.User.FirstName}[/] är nu skapat för vecka {currentWeek}.");
        }

        public void OverseeInvoices()
        {
            var invoices = _context.Invoices
                .Include(i => i.Account)
                    .ThenInclude(a => a.User)
                .Include(i => i.MembershipPlan)
                .OrderByDescending(i => i.InvoiceId)
                .AsNoTracking()
                .ToList();

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title("[yellow]SPECTRE KICKBOXING - FAKTURAÖVERSIKT[/]");

            table.AddColumn("[bold]Medlem[/]");
            table.AddColumn("[bold]ID[/]");
            table.AddColumn("[bold]Skickat[/]");

            foreach (var i in invoices)
            {
                string medlem = i.Account.User != null
                    ? $"{i.Account.User.FirstName} {i.Account.User.LastName}"
                    : "Okänd Användare";

                table.AddRow(
                    $"[blue]{medlem}[/]",
                    i.InvoiceId.ToString(),
                    i.InvoiceDate.ToString("yyyy-MM-dd")
                );
            }

            AnsiConsole.Write(table);

            AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey(true);
        }

        public void WriteNewsletter(int accountId, string authorName)
        {
            AnsiConsole.MarkupLine($"[yellow]Skriv och posta ett nyhetsbrev som:[/] [blue]{authorName}[/]");

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
                PostedByAccountId = accountId,
                IsActive = true,
                PostYear = DateTime.Now.Year,
                PostWeek = weekNum
            };

            _context.Newsletters.Add(newsletter);
            _context.SaveChanges();

            AnsiConsole.MarkupLine("[green]Nyhetsbrevet har postats![/]");
        }

        public void ShowRevenue()
        {
            // Match: VwTotalInvoiceRevenues
            var rev = _context.VwTotalInvoiceRevenues.FirstOrDefault();

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

        public void CleanOldNewsletters()
        {
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            DateTime now = DateTime.Now;

            int currentWeek = cal.GetWeekOfYear(now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int currentYear = now.Year;

            int thresholdWeek = currentWeek - 4;
            int thresholdYear = currentYear;

            if (thresholdWeek <= 0)
            {
                thresholdYear -= 1;
                int lastYearMaxWeek = cal.GetWeekOfYear(new DateTime(thresholdYear, 12, 31), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                thresholdWeek = lastYearMaxWeek + thresholdWeek;
            }

            var newsToDelete = _context.Newsletters
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
                _context.Newsletters.RemoveRange(newsToDelete);
                _context.SaveChanges();
                AnsiConsole.MarkupLine($"[green]Rensat![/] {newsToDelete.Count} gamla nyhetsbrev raderades.");
            }
        }

        public void CreateAndSendInvoice()
        {
            AnsiConsole.MarkupLine("[yellow]Skapa nya fakturor för medlemmar[/]");

            var invoicesToCreate = new List<Invoice>();
            bool addingInvoices = true;

            while (addingInvoices)
            {
                int accountId = AnsiConsole.Ask<int>("Ange [green]AccountID[/] för faktura:");

                var accountInfo = _context.Accounts
                    .Where(a => a.AccountId == accountId)
                    .Select(a => new
                    {
                        FullName = a.User.FirstName + " " + a.User.LastName,
                        MembershipPlanID = a.User.Memberships
                            .Where(m => m.IsActive == true)
                            .Select(m => m.MembershipPlanId)
                            .FirstOrDefault(),
                        Price = a.User.Memberships
                            .Where(m => m.IsActive == true)
                            .Select(m => m.MembershipPlan.Price.Amount)
                            .FirstOrDefault()
                    })
                    .FirstOrDefault();

                if (accountInfo == null || accountInfo.Price == 0 || accountInfo.MembershipPlanID == 0)
                {
                    AnsiConsole.MarkupLine("[red]Ingen aktiv medlemskap eller pris hittades för detta konto.[/]");
                    continue;
                }

                AnsiConsole.MarkupLine($"Skapar faktura för: [green]{accountInfo.FullName}[/], Belopp: [green]{accountInfo.Price} kr[/]");
                if (!AnsiConsole.Confirm("Vill du fortsätta med denna faktura?"))
                    continue;

                invoicesToCreate.Add(new Invoice
                {
                    AccountId = accountId,
                    MembershipPlanId = accountInfo.MembershipPlanID,
                    InvoiceDate = DateTime.Now
                });

                addingInvoices = AnsiConsole.Confirm("Vill du lägga till en till faktura?");
            }

            if (invoicesToCreate.Count > 0)
            {
                _context.Invoices.AddRange(invoicesToCreate);
                _context.SaveChanges();
                AnsiConsole.MarkupLine($"[green]{invoicesToCreate.Count} fakturor skapades och skickades![/]");
            }

            AnsiConsole.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey(true);
        }

        public void DeleteMember()
        {
            int id = AnsiConsole.Ask<int>("AccountID att ta bort?");

            var acc = _context.Accounts
                .Include(a => a.User)
                .FirstOrDefault(a => a.AccountId == id);

            if (acc == null)
            {
                AnsiConsole.MarkupLine($"[red]Konto med ID {id} hittades inte.[/]");
                return;
            }

            string fullName = $"{acc.User.FirstName} {acc.User.LastName}";

            if (AnsiConsole.Confirm($"Är du säker på att du vill radera [bold]{fullName}[/] (AccountID: [yellow]{id}[/])?"))
            {
                var relatedInvoices = _context.Invoices.Where(i => i.AccountId == id);
                _context.Invoices.RemoveRange(relatedInvoices);

                var relatedNewsletters = _context.Newsletters.Where(n => n.PostedByAccountId == id);
                _context.Newsletters.RemoveRange(relatedNewsletters);

                var relatedSessions = _context.Sessions.Where(s => s.TrainerId == id);
                _context.Sessions.RemoveRange(relatedSessions);

                _context.Accounts.Remove(acc);

                _context.SaveChanges();
                AnsiConsole.MarkupLine($"[green]Kontot för {fullName} har raderats![/]");
            }
        }
    }
}