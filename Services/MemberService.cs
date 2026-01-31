using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Models;
using SpectreKickBox.Data;

namespace SpectreKickBox.Services
{
    public class MemberService
    {
        private Account? _currentUser;

        private readonly KickBoxingClubContext _context;

        public MemberService(KickBoxingClubContext context)
        {
            _context = context;
        }

        // --- NEW: SEARCH SESSIONS BY TYPE ---
        public void LookForSession()
        {
            var typeName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vilken [blue]grupp[/] vill du söka efter?")
                    .AddChoices(new[] { "Newbie", "Experienced", "Advanced" }));

            var sessions = _context.Session
                .Include(s => s.SessionType)
                .Include(s => s.WeekDay)
                .Include(s => s.Trainer)
                    .ThenInclude(t => t.AppUser)
                .Where(s => s.SessionType.TypeTitle == typeName)
                .OrderBy(s => s.DayID)
                .ToList();

            if (!sessions.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]Inga pass hittades för {typeName} just nu.[/]");
                return;
            }

            var table = new Table()
                .Title($"Pass för [bold blue]{typeName}[/]")
                .Border(TableBorder.Rounded)
                .AddColumn("Dag")
                .AddColumn("Tid")
                .AddColumn("Fokus")
                .AddColumn("Tränare");

            foreach (var s in sessions)
            {
                table.AddRow(
                    s.WeekDay.DayName,
                    s.StartTime.ToString(@"hh\:mm"),
                    s.Focus ?? "Basictraining",
                    $"{s.Trainer.AppUser.FirstName} {s.Trainer.AppUser.LastName}"
                );
            }

            AnsiConsole.Write(table);
        }

        // --- UPDATED: DELETE MEMBER WITH FULL CLEANUP ---
        public void DeleteMember(Account userAcct)
        {
            var acc = _context.Account
                .Include(a => a.AppUser)
                .FirstOrDefault(a => a.AccountID == userAcct.AccountID);

            if (acc == null || acc.AppUser == null)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta kontot.[/]");
                return;
            }

            string fullName = $"{acc.AppUser.FirstName} {acc.AppUser.LastName}";

            if (!AnsiConsole.Confirm($"Vill du verkligen radera [red]{fullName}[/]? Detta går inte att ångra."))
                return;

            // --- DELETE DEPENDENCIES FIRST ---
            var invoices = _context.Invoice.Where(i => i.AccountID == acc.AccountID);
            _context.Invoice.RemoveRange(invoices);

            var newsletters = _context.Newsletter.Where(n => n.PostedByAccountID == acc.AccountID);
            _context.Newsletter.RemoveRange(newsletters);

            var sessions = _context.Session.Where(s => s.TrainerID == acc.AccountID);
            _context.Session.RemoveRange(sessions);

            // --- DELETE ACCOUNT ---
            _context.Account.Remove(acc);
            _context.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Kontot för {fullName} har raderats helt.[/]");
        }

        public void ShowNews()
        {
            var newsItems = _context.Newsletter
                .Include(n => n.PostedByAccount)
                    .ThenInclude(a => a.AppUser)
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.PostYear)
                .ThenByDescending(n => n.PostWeek)
                .ToList();

            if (!newsItems.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Inga nyheter att visa just nu.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("Ämne")
                .AddColumn("Info")
                .AddColumn("Publicerat av")
                .AddColumn("Vecka");

            foreach (var n in newsItems)
            {
                var postedBy = n.PostedByAccount?.AppUser != null
                    ? $"{n.PostedByAccount.AppUser.FirstName} {n.PostedByAccount.AppUser.LastName}"
                    : "Okänd";

                table.AddRow(n.NewsTitle, n.NewsContent, postedBy, $"{n.PostWeek} {n.PostYear}");
            }

            AnsiConsole.Write(table);
        }

        // OBS: Detta är mer "demo" just nu - den loggar in via email utan lösenord
        public void LoginAndShowDashboard()
        {
            var email = AnsiConsole.Ask<string>("Email");

            var userAcct = _context.Account
                .Include(a => a.AppUser)
                .Include(a => a.Role)
                .FirstOrDefault(a => a.Email == email);

            if (userAcct == null || userAcct.AppUser == null)
            {
                AnsiConsole.MarkupLine("[red]Invalid login![/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]Välkommen {userAcct.AppUser.FirstName}![/]");

            var membership = _context.Membership
                .Include(m => m.MembershipPlan)
                    .ThenInclude(mp => mp.PriceList)
                .FirstOrDefault(m => m.UserID == userAcct.AppUser.UserID && m.IsActive);

            if (membership == null)
            {
                AnsiConsole.MarkupLine("[yellow]Du har inget aktivt medlemskap[/]");
                return;
            }

            AnsiConsole.MarkupLine($"Plan: {membership.MembershipPlan.BillingPeriod}");
            AnsiConsole.MarkupLine($"Pris: {membership.MembershipPlan.PriceList.Label} ({membership.MembershipPlan.PriceList.Amount} kr)");
        }

        public void MemberMenu(Account userAcct)
        {
            // Hämta om användaren från DB så AppUser alltid är laddad och aktuell
            _currentUser = _context.Account
                .Include(a => a.AppUser)
                .Include(a => a.Role)
                .First(a => a.AccountID == userAcct.AccountID);

            if (_currentUser == null || _currentUser.AppUser == null)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte ladda inloggad användare.[/]");
                return;
            }

            bool inMemberMenu = true;
            while (inMemberMenu)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[bold blue]Inloggad som:[/] {_currentUser.AppUser.FirstName} {_currentUser.AppUser.LastName}");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Medlemsmeny[/]")
                        .AddChoices(new[] {
                            "Min Profil & Medlemskap",
                            "Förläng medlemskap",
                            "Avsluta medlemskap",
                            "Sök Träningspass",
                            "Radera Mitt Konto",
                            "Logga ut"
                        }));

                switch (choice)
                {
                    case "Min Profil & Medlemskap":
                        ShowProfile(_currentUser);
                        break;

                    case "Förläng medlemskap": // FIX: exakt strängmatchning
                        ExtendMembership();
                        break;

                    case "Avsluta medlemskap":
                        CancelMembership();
                        break;

                    case "Sök Träningspass":
                        LookForSession();
                        break;

                    case "Radera Mitt Konto":
                        DeleteMember(_currentUser);
                        inMemberMenu = false;
                        break;

                    case "Logga ut":
                        inMemberMenu = false;
                        break;
                }

                if (inMemberMenu)
                {
                    AnsiConsole.WriteLine("\nTryck på valfri tangent för att gå tillbaka...");
                    Console.ReadKey();
                }
            }
        }

        // --- UPDATE: EXTEND MEMBERSHIP (no params, uses _currentUser) ---
        private void ExtendMembership()
        {
            if (_currentUser?.AppUser == null)
            {
                AnsiConsole.MarkupLine("[red]Ingen inloggad användare hittades.[/]");
                return;
            }

            var membership = _context.Membership
                .Include(m => m.MembershipPlan)
                .FirstOrDefault(m =>
                    m.UserID == _currentUser.AppUser.UserID && m.IsActive);

            if (membership == null)
            {
                AnsiConsole.MarkupLine("[yellow]Du har inget aktivt medlemskap att förlänga.[/]");
                return;
            }

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Hur länge vill du förlänga?[/]")
                    .AddChoices("1 månad", "3 månader", "1 år", "Avbryt"));

            if (choice == "Avbryt")
                return;

            membership.EndDate = choice switch
            {
                "1 månad" => membership.EndDate.AddMonths(1),
                "3 månader" => membership.EndDate.AddMonths(3),
                "1 år" => membership.EndDate.AddYears(1),
                _ => membership.EndDate
            };

            _context.SaveChanges();

            AnsiConsole.MarkupLine(
                $"[green]Medlemskapet har förlängts![/]\n" +
                $"Nytt slutdatum: [yellow]{membership.EndDate:yyyy-MM-dd}[/]");
        }

        // --- UPDATE: CANCEL MEMBERSHIP (no params, uses _currentUser) ---
        private void CancelMembership()
        {
            if (_currentUser?.AppUser == null)
            {
                AnsiConsole.MarkupLine("[red]Ingen inloggad användare hittades.[/]");
                return;
            }

            var membership = _context.Membership
                .FirstOrDefault(m =>
                    m.UserID == _currentUser.AppUser.UserID && m.IsActive);

            if (membership == null)
            {
                AnsiConsole.MarkupLine("[yellow]Du har inget aktivt medlemskap att avsluta.[/]");
                return;
            }

            if (!AnsiConsole.Confirm("[red]Är du säker att du vill avsluta ditt medlemskap?[/]"))
                return;

            membership.IsActive = false;
            _context.SaveChanges();

            AnsiConsole.MarkupLine("[green]Ditt medlemskap är nu avslutat.[/]");
        }

        // Helper to show the dashboard info without asking for email again
        private void ShowProfile(Account userAcct)
        {
            // Hämta konto + AppUser från DB (viktigt för FK!)
            var acc = _context.Account
                .Include(a => a.AppUser)
                .Include(a => a.Role)
                .FirstOrDefault(a => a.AccountID == userAcct.AccountID);

            if (acc?.AppUser == null)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta användaren kopplad till kontot.[/]");
                return;
            }

            var userId = acc.AppUser.UserID;

            // Visa grundinfo
            AnsiConsole.MarkupLine($"[bold underline]Profil Information[/]");
            AnsiConsole.MarkupLine($"Namn: {acc.AppUser.FirstName} {acc.AppUser.LastName}");
            AnsiConsole.MarkupLine($"Roll: {acc.Role.Title}");

            // Hämta aktivt medlemskap
            var membership = _context.Membership
                .Include(m => m.MembershipPlan)
                    .ThenInclude(mp => mp.PriceList)
                .FirstOrDefault(m => m.UserID == userId && m.IsActive);

            if (membership != null)
            {
                AnsiConsole.MarkupLine($"Plan: [green]{membership.MembershipPlan.BillingPeriod}[/]");
                AnsiConsole.MarkupLine(
                    $"Pris: {membership.MembershipPlan.PriceList.Label} " +
                    $"({membership.MembershipPlan.PriceList.Amount} kr)"
                );
                return;
            }

            // Inget medlemskap: erbjud nytt
            AnsiConsole.MarkupLine("[yellow]Inget aktivt medlemskap hittades.[/]");

            if (!AnsiConsole.Confirm("Vill du teckna ett nytt medlemskap?"))
                return;

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Välj din prisplan[/]")
                    .AddChoices("Monthly", "Quarterly", "Yearly", "Avsluta"));

            if (choice == "Avsluta")
                return;

            // Hämta vald MembershipPlan
            var plan = _context.MembershipPlan
                .FirstOrDefault(p => p.BillingPeriod == choice);

            if (plan == null)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta vald plan i databasen.[/]");
                return;
            }

            // Beräkna giltighetstid
            var start = DateTime.Now;

            var end = choice switch
            {
                "Monthly" => start.AddMonths(1),
                "Quarterly" => start.AddMonths(3),
                "Yearly" => start.AddYears(1),
                _ => start.AddMonths(1)
            };

            // Skapa Membership (create relation)
            var newMembership = new Membership
            {
                UserID = userId,
                MembershipPlanID = plan.MembershipPlanID,
                IsActive = true,
                StartDate = start,
                EndDate = end
            };

            _context.Membership.Add(newMembership);
            _context.SaveChanges();

            AnsiConsole.MarkupLine(
                $"[green]Medlemskap har skapats![/]\n" +
                $"Din prisplan är: [blue]{plan.BillingPeriod}[/]\n" +
                $"Giltigt från: [yellow]{start:yyyy-MM-dd}[/] till [yellow]{end:yyyy-MM-dd}[/]"
            );
        }
    }
}
