using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Models;
using SpectreKickBox.Data;

namespace SpectreKickBox.Services
{
    public class MemberService
    {
        private readonly KickBoxingClubContext _context;

        public MemberService(KickBoxingClubContext context)
        {
            _context = context;
        }

        public void LookForSession()
        {
            var typeName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vilken [blue]grupp[/] vill du söka efter?")
                    .AddChoices(new[] { "Newbie", "Experienced", "Advanced" }));

            // Match: .Sessions, .WeekDays, .User
            var sessions = _context.Sessions
                .Include(s => s.SessionType)
                .Include(s => s.Day)
                .Include(s => s.Trainer)
                    .ThenInclude(t => t.User)
                .Where(s => s.SessionType.TypeTitle == typeName)
                .OrderBy(s => s.DayId)
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
                    s.Day.DayName,
                    s.StartTime.ToString(@"hh\:mm"),
                    s.Focus ?? "Basic training",
                    $"{s.Trainer.User.FirstName} {s.Trainer.User.LastName}"
                );
            }

            AnsiConsole.Write(table);
        }

        public void DeleteMember(Account userAcct)
        {
            // Match: .Accounts, .User, .AccountId
            var acc = _context.Accounts
                .Include(a => a.User)
                .FirstOrDefault(a => a.AccountId == userAcct.AccountId);

            if (acc == null)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta kontot.[/]");
                return;
            }

            string fullName = $"{acc.User.FirstName} {acc.User.LastName}";

            if (!AnsiConsole.Confirm($"Vill du verkligen radera [red]{fullName}[/]? Detta går inte att ångra."))
                return;

            // --- DELETE DEPENDENCIES ---
            var invoices = _context.Invoices.Where(i => i.AccountId == acc.AccountId);
            _context.Invoices.RemoveRange(invoices);

            var newsletters = _context.Newsletters.Where(n => n.PostedByAccountId == acc.AccountId);
            _context.Newsletters.RemoveRange(newsletters);

            var sessions = _context.Sessions.Where(s => s.TrainerId == acc.AccountId);
            _context.Sessions.RemoveRange(sessions);

            // --- DELETE ACCOUNT ---
            _context.Accounts.Remove(acc);
            _context.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Kontot för {fullName} har raderats helt.[/]");
        }

        public void ShowNews()
        {
            // Match: .Newsletters, .User
            var newsItems = _context.Newsletters
                .Include(n => n.PostedByAccount)
                    .ThenInclude(a => a.User)
                .Where(n => n.IsActive == true)
                .OrderByDescending(n => n.PostYear)
                .ThenByDescending(n => n.PostWeek)
                .ToList();

            var table = new Table().AddColumn("Ämne").AddColumn("Info").AddColumn("Publicerat av").AddColumn("Vecka");
            foreach (var n in newsItems)
            {
                table.AddRow(
                    n.NewsTitle,
                    n.NewsContent,
                    $"{n.PostedByAccount.User.FirstName} {n.PostedByAccount.User.LastName}",
                    $"{n.PostWeek} {n.PostYear}");
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att återgå till huvudmenyn...[/]");
            Console.ReadKey(true);
        }

        public void LoginAndShowDashboard()
        {
            var email = AnsiConsole.Ask<string>("Email");
            var userAcct = _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Role)
                .FirstOrDefault(a => a.Email == email);

            if (userAcct == null)
            {
                AnsiConsole.MarkupLine("[red]Invalid login![/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]Välkommen {userAcct.User.FirstName}![/]");

            // Match: .Memberships, .Price
            var membership = _context.Memberships
                .Include(m => m.MembershipPlan)
                    .ThenInclude(mp => mp.Price)
                .FirstOrDefault(m => m.UserId == userAcct.User.UserId && m.IsActive == true);

            if (membership == null)
            {
                AnsiConsole.MarkupLine("[yellow]Du har inget aktivt medlemskap[/]");
                return;
            }

            AnsiConsole.MarkupLine($"Plan: {membership.MembershipPlan.BillingPeriod}");
            AnsiConsole.MarkupLine($"Pris: {membership.MembershipPlan.Price.Label} ({membership.MembershipPlan.Price.Amount} kr)");
        }

        public void MemberMenu(Account userAcct)
        {
            bool inMemberMenu = true;
            while (inMemberMenu)
            {
                AnsiConsole.Clear();
                // Assumes SessionService has a static ShowHeader method
                SessionService.ShowHeader(Spectre.Console.Color.Yellow, "[bold red] MEDLEMSMENY[/]");
                AnsiConsole.MarkupLine($"[bold blue]Inloggad som:[/] {userAcct.User.FirstName} {userAcct.User.LastName}");

                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold white]VAD ÄR DITT NÄSTA DRAG?[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "👤 Min Profil & Medlemskap",
                        "🔍 Sök Träningspass",
                        "🔥 Radera Mitt Konto",
                        "🚪 Logga ut"
                    }));

                switch (choice)
                {
                    case "👤 Min Profil & Medlemskap":
                        ShowProfile(userAcct);
                        break;
                    case "🔍 Sök Träningspass":
                        LookForSession();
                        break;
                    case "🔥 Radera Mitt Konto":
                        DeleteMember(userAcct);
                        inMemberMenu = false;
                        break;
                    case "🚪 Logga ut":
                        inMemberMenu = false;
                        break;
                }

                if (inMemberMenu)
                {
                    AnsiConsole.WriteLine("\nTryck på valfri tangent för att gå tillbaka...");
                    Console.ReadKey(true);
                }
            }
        }

        private void ShowProfile(Account userAcct)
        {
            // Match: .Accounts, .User, .Role
            var acc = _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Role)
                .FirstOrDefault(a => a.AccountId == userAcct.AccountId);

            if (acc?.User == null)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta användaren kopplad till kontot.[/]");
                return;
            }

            var userId = acc.User.UserId;

            AnsiConsole.MarkupLine($"[bold underline]Profil Information[/]");
            AnsiConsole.MarkupLine($"Namn: {acc.User.FirstName} {acc.User.LastName}");
            AnsiConsole.MarkupLine($"Roll: {acc.Role.Title}");

            // Match: .Memberships, .Price
            var membership = _context.Memberships
                .Include(m => m.MembershipPlan)
                    .ThenInclude(mp => mp.Price)
                .FirstOrDefault(m => m.UserId == userId && m.IsActive == true);

            if (membership != null)
            {
                AnsiConsole.MarkupLine($"Plan: [green]{membership.MembershipPlan.BillingPeriod}[/]");
                AnsiConsole.MarkupLine(
                    $"Pris: {membership.MembershipPlan.Price.Label} " +
                    $"({membership.MembershipPlan.Price.Amount} kr)"
                );
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Inget aktivt medlemskap hittades.[/]");

                if (!AnsiConsole.Confirm("Vill du teckna ett nytt medlemskap?"))
                    return;

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Välj din prisplan[/]")
                        .AddChoices("Monthly", "Quarterly", "Yearly", "Avsluta"));

                if (choice == "Avsluta")
                    return;

                var plan = _context.MembershipPlans
                    .FirstOrDefault(p => p.BillingPeriod == choice);

                if (plan == null)
                {
                    AnsiConsole.MarkupLine("[red]Kunde inte hitta vald plan i databasen.[/]");
                    return;
                }

                var start = DateTime.Now;
                var end = choice switch
                {
                    "Monthly" => start.AddMonths(1),
                    "Quarterly" => start.AddMonths(3),
                    "Yearly" => start.AddYears(1),
                    _ => start.AddMonths(1)
                };

                var newMembership = new Membership
                {
                    UserId = userId,
                    MembershipPlanId = plan.MembershipPlanId,
                    IsActive = true,
                    StartDate = start,
                    EndDate = end
                };

                _context.Memberships.Add(newMembership);
                _context.SaveChanges();

                AnsiConsole.MarkupLine(
                    $"[green]Medlemskap har skapats![/]\n" +
                    $"Din prisplan är: [blue]{plan.BillingPeriod}[/]\n" +
                    $"Giltigt från: [yellow]{start:yyyy-MM-dd}[/] till [yellow]{end:yyyy-MM-dd}[/]"
                );
            }
        }
    }
}