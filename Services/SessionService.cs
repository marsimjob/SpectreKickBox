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

    public static void ShowHeader(Spectre.Console.Color colorSet, string message)
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText(".NET KICKBOXING")
                .LeftJustified()
                .Color(colorSet));
        AnsiConsole.Write(new Rule(message) { Justification = Justify.Left });
        AnsiConsole.WriteLine();
    }

    public void ShowWeeklySessions()
    {
        var sessions = _context.Session
            .Include(s => s.WeekDay)
            .Include(s => s.Trainer)
                .ThenInclude(a => a.AppUser)
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
                s.WeekDay.DayName,
                s.StartTime.ToString().Substring(0, 5),
                $"{s.Trainer.AppUser.FirstName} {s.Trainer.AppUser.LastName}",
                s.Focus);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att återgå till huvudmenyn...[/]");
        Console.ReadKey(true);
    }
}
