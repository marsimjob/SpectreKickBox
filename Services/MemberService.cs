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

        // --- NEW: SEARCH SESSIONS BY TYPE ---
        public void LookForSession()
        {
            // Use a Selection Prompt so users don't have to type the name manually
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

            if (acc == null)
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

            var table = new Table().AddColumn("Topic").AddColumn("Contents").AddColumn("Posted By");
            foreach (var n in newsItems)
            {
                table.AddRow(n.NewsTitle, n.NewsContent, $"{n.PostedByAccount.AppUser.FirstName} {n.PostedByAccount.AppUser.LastName}");
            }
            AnsiConsole.Write(table);
        }

        public void LoginAndShowDashboard()
        {
            var email = AnsiConsole.Ask<string>("Email");
            var userAcct = _context.Account
                .Include(a => a.AppUser)
                .Include(a => a.Role)
                .FirstOrDefault(a => a.Email == email);

            if (userAcct == null)
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
            bool inMemberMenu = true;
            while (inMemberMenu)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[bold blue]Inloggad som:[/] {userAcct.AppUser.FirstName} {userAcct.AppUser.LastName}");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Medlemsmeny[/]")
                        .AddChoices(new[] {
                    "Min Profil & Medlemskap",
                    "Sök Träningspass",
                    "Radera Mitt Konto", // Optional: Be careful!
                    "Logga ut"
                        }));

                switch (choice)
                {
                    case "Min Profil & Medlemskap":
                        ShowProfile(userAcct); // Split the dashboard logic
                        break;
                    case "Sök Träningspass":
                        LookForSession();
                        break;
                    case "Radera Mitt Konto":
                        DeleteMember(userAcct);
                        // If they delete themselves, we must kick them out
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

        // Helper to show the dashboard info without asking for email again
        private void ShowProfile(Account userAcct)
        {
            AnsiConsole.MarkupLine($"[bold underline]Profil Information[/]");
            AnsiConsole.MarkupLine($"Namn: {userAcct.AppUser.FirstName} {userAcct.AppUser.LastName}");
            AnsiConsole.MarkupLine($"Roll: {userAcct.Role.Title}");

            var membership = _context.Membership
                .Include(m => m.MembershipPlan)
                    .ThenInclude(mp => mp.PriceList)
                .FirstOrDefault(m => m.UserID == userAcct.AppUser.UserID && m.IsActive);

            if (membership != null)
            {
                AnsiConsole.MarkupLine($"Plan: [green]{membership.MembershipPlan.BillingPeriod}[/]");
                AnsiConsole.MarkupLine($"Pris: {membership.MembershipPlan.PriceList.Label} ({membership.MembershipPlan.PriceList.Amount} kr)");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Inget aktivt medlemskap hittades.[/]");
            }
        }
    }
}