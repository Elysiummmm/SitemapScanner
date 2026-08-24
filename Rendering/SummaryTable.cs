using System.Net;

namespace SitemapScanner.Rendering;

public class SummaryTable(SitemapScanner sitemapScanner) : TerminalScreen
{
    protected override int RenderDelay => 0;

    private static List<string> _keybinds = [
        "[Q] - quit",
        "[\u2191] / [PgUp] - scroll up",
        "[\u2193] / [PgDown] - scroll down",
        "[E] - export data"
    ]; 
    
    private SitemapScanner Scanner { get; init; } = sitemapScanner;
    private int BrokenLinkLines { get; set; } = 0;
    private int BrokenLinkStartLine { get; set; } = 0;
    private int BrokenLinkLineOffset { get; set; } = 0;

    protected override void Render()
    {
        BrokenLinkList();
        
        var key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.Q:
                Running = false;
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.PageUp:
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.PageDown:
                break;
            case ConsoleKey.E:
                break;
        }
    }

    protected override async Task BeforeStart()
    {
        Console.Clear();
        Header();
        Footer();
        StatusTable();

        BrokenLinkLines = Console.WindowHeight - Console.CursorTop - 2;
        BrokenLinkStartLine = Console.CursorTop + 1;
    }

    private void Header()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;

        var headerText = CenteredText("Scan Summary");
        Console.Write(headerText);
    }

    private void Footer()
    {
        Console.ResetColor();
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        
        foreach (var bind in _keybinds)
        {
            var keybindText = CenteredText(bind, Console.WindowWidth / _keybinds.Count);
            Console.Write(keybindText);
        }
    }

    private void StatusTable()
    {
        Console.ResetColor();
        Console.SetCursorPosition(0, 1);
        
        List<HttpStatusCode> statusCodes = [];
        foreach (var site in Scanner.SitesChecked)
            if (!statusCodes.Contains(site.status)) statusCodes.Add(site.status);

        foreach (var statusCode in statusCodes)
        {
            var count = Scanner.SitesChecked.FindAll(s => s.status == statusCode).Count;
                
            var statusText = CenteredText($"Status {(int)statusCode} {Enum.GetName(statusCode) ?? "???"}", Console.WindowWidth / 2);
            var countText = CenteredText(count.ToString(), Console.WindowWidth / 2);
            
            Console.Write(statusText + countText);
            Console.CursorTop++;
        }
    }

    private void BrokenLinkList()
    {
        Console.ResetColor();
        Console.SetCursorPosition(0, BrokenLinkStartLine);
        
        Console.BackgroundColor = ConsoleColor.White;
        Console.Write(new string('\u2501', Console.WindowWidth));
        Console.SetCursorPosition(0, BrokenLinkStartLine + BrokenLinkLines);
        Console.Write(new string('\u2501', Console.WindowWidth));
        
        for (var i = 0; i < BrokenLinkLines; i++)
        {
            
        }
    }
}