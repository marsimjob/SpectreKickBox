using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Models;
using SpectreKickBox.Data;

public class MemberService
{
    private readonly KickBoxingClubContext _context;

    public MemberService(KickBoxingClubContext context)
    {
        _context = context;
    }

    public void ShowNews()
    {
        var newsItems = _context.Newsletter
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.PostYear)
            .ThenByDescending(n => n.PostWeek)
            .ToList();

        var table = new Table().AddColumn("Topic").AddColumn("Contents").AddColumn("Posted By");
        foreach (var n in newsItems)
        {
            table.AddRow(n.NewsTitle, n.NewsContent, $"{n.PostedByAccount.User.FirstName} {n.PostedByAccount.User.LastName}");
        }

        AnsiConsole.Write(table);
    }

    public void LoginAndShowDashboard()
    {
        var email = AnsiConsole.Ask<string>("Email");
        var userAcct = _context.Account
            .Include(a => a.User)
            .Include(a => a.Role)
            .FirstOrDefault(a => a.Email == email);

        if (userAcct == null)
        {
            AnsiConsole.MarkupLine("[red]Invalid login![/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Välkommen {userAcct.User.FirstName}![/]");

        var membership = _context.Membership
            .Include(m => m.MembershipPlan)
            .ThenInclude(mp => mp.PriceList)
            .FirstOrDefault(m => m.UserID == userAcct.UserID && m.IsActive);

        if (membership == null)
        {
            AnsiConsole.MarkupLine("[yellow]Du har inget aktivt medlemskap[/]");
            return;
        }

        AnsiConsole.MarkupLine($"Plan: {membership.MembershipPlan.BillingPeriod}");
        AnsiConsole.MarkupLine($"Pris: {membership.MembershipPlan.PriceList.Label} ({membership.MembershipPlan.PriceList.Amount} kr)");
    }
}
