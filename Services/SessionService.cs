using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Data;

public class SessionService
{
    private readonly KickBoxingClubContext _context;

    public SessionService(KickBoxingClubContext context)
    {
        _context = context;
    }

    public void ShowWeeklySessions()
    {
        var sessions = _context.Session
            .Include(s => s.DayOfWeek)
            .Include(s => s.Trainer)
                .ThenInclude(a => a.User)
            .OrderBy(s => s.DayID)
            .ThenBy(s => s.StartTime)
            .ToList();

        var table = new Table()
            .AddColumn("Dag")
            .AddColumn("Tid")
            .AddColumn("Tränare")
            .AddColumn("Beskrivning");

        foreach (var s in sessions)
        {
            table.AddRow(
                s.DayOfWeek.DayName,
                s.StartTime.ToString().Substring(0, 5),
                $"{s.Trainer.User.FirstName} {s.Trainer.User.LastName}",
                s.Description);
        }

        AnsiConsole.Write(table);
    }
}
