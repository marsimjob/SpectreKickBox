using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Data;

namespace SpectreKickBox.Services
{
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
            // Match: .Sessions, .Day, .User, .DayId
            var sessions = _context.Sessions
                .Include(s => s.Day)
                .Include(s => s.Trainer)
                    .ThenInclude(a => a.User)
                .OrderBy(s => s.DayId)
                .ThenBy(s => s.StartTime)
                .ToList();

            var table = new Table()
                .Title("[bold green]VECKOSCHEMA[/]")
                .Border(TableBorder.Rounded)
                .AddColumn("Dag")
                .AddColumn("Tid")
                .AddColumn("Tränare")
                .AddColumn("Beskrivning");

            foreach (var s in sessions)
            {
                // Format the time safely: HH:mm
                string timeDisplay = s.StartTime.ToString(@"hh\:mm");

                table.AddRow(
                    s.Day.DayName,
                    timeDisplay,
                    $"{s.Trainer.User.FirstName} {s.Trainer.User.LastName}",
                    s.Focus ?? "Basic training"
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att återgå till huvudmenyn...[/]");
            Console.ReadKey(true);
        }
    }
}