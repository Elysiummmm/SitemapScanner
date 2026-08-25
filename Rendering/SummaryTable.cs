using System.Diagnostics;
using System.Net;
using System.Text;

namespace SitemapScanner.Rendering;

public class SummaryTable(SitemapScanner sitemapScanner) : TerminalScreen
{
    protected override int RenderDelay => 0;

    private static List<string> _keybinds = [
        "[Q] - quit",
        "[\u2191] / [PgUp] - scroll up",
        "[\u2193] / [PgDown] - scroll down",
        "[E] - export broken links"
    ]; 
    
    private SitemapScanner Scanner { get; init; } = sitemapScanner;
    private List<(string url, HttpStatusCode code)> BrokenLinks { get; set; } = [];
    private int BrokenLinkLines { get; set; } = 0;
    private int BrokenLinkStartLine { get; set; } = 0;
    private int BrokenLinkLineOffset { get; set; } = 0;

    protected override void Render()
    {
        BrokenLinkList();
        Footer();
        
        var key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.Q:
                Running = false;
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.PageUp:
                if (BrokenLinkLineOffset > 0) BrokenLinkLineOffset--;
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.PageDown:
                if (BrokenLinkLineOffset < BrokenLinks.Count - 1) BrokenLinkLineOffset++;
                break;
            case ConsoleKey.E:
                var path = ExportPathInput();
                ExportCsv(path);
                
                // full re-render because of the readline
                Console.Clear();
                Header();
                StatusTable();
                break;
        }
    }

    protected override async Task BeforeStart()
    {
        Console.Clear();
        Header();
        StatusTable();

        BrokenLinkLines = Console.WindowHeight - Console.CursorTop - 2;
        BrokenLinkStartLine = Console.CursorTop + 1;
        BrokenLinks = Scanner.SitesChecked.FindAll(s => s.status != HttpStatusCode.OK);
    }

    protected override Task OnFinished()
    {
        Console.SetCursorPosition(0, Console.WindowHeight);
        return Task.CompletedTask;
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

            string rowLabel;

            if (statusCode == HttpStatusCode.Continue) rowLabel = "Unreachable";
            else rowLabel = $"Status {(int)statusCode} {Enum.GetName(statusCode) ?? "???"}";
            
            var statusText = CenteredText(rowLabel, Console.WindowWidth / 2);
            var countText = CenteredText(count.ToString(), Console.WindowWidth / 2);
            
            Console.Write((statusText + countText).PadLeft(Console.WindowWidth, ' '));
        }
    }

    private void BrokenLinkList()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        
        Console.SetCursorPosition(0, BrokenLinkStartLine);
        Console.Write(CenteredText("Broken Links"));
        
        Console.ResetColor();
        Console.SetCursorPosition(0, BrokenLinkStartLine + 1);
        
        var statusCharacters = 15;
        var urlCharacters = Console.WindowWidth - statusCharacters;
        
        for (var row = 0; row < BrokenLinkLines; row++)
        {
            var rowText = new StringBuilder();
            
            try
            {
                var rowData = BrokenLinks[row + BrokenLinkLineOffset];
                
                var urlText = rowData.url.PadRight(urlCharacters, ' ');
                if (urlText.Length > urlCharacters) urlText = "..." + urlText[3..(urlCharacters - 3)];

                string statusText;

                if (rowData.code == HttpStatusCode.Continue)
                {
                    statusText = "Unreachable";
                }
                else
                {
                    statusText = $"{(int)rowData.code} ";
                    statusText += Enum.GetName(rowData.code) ?? "???";
                }
                
                statusText = statusText.PadLeft(statusCharacters, ' ');

                rowText.Append(urlText);
                rowText.Append(statusText);
            }
            catch (Exception)
            {
                rowText = new StringBuilder();
                rowText.Append(new string(' ', Console.WindowWidth));
            }
            
            Console.Write(rowText.ToString());
            Scanner.Logger.WriteLog(rowText.ToString());
        }
        
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.SetCursorPosition(0, BrokenLinkStartLine + BrokenLinkLines - 1);
        Console.Write(new string(' ', Console.WindowWidth));

        if (BrokenLinks.Count > 0) return;
        Console.ResetColor();
        Console.SetCursorPosition(0, BrokenLinkStartLine + (BrokenLinkLines - 1) / 2);
        Console.Write(CenteredText("No broken links found!"));
    }

    private string ExportPathInput()
    {
        Console.CursorVisible = true;
        
        ClearLine(Console.WindowHeight - 1);
        
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write("Export location (CSV file, leave blank to cancel): ");
        
        var path = Console.ReadLine() ?? "";
        
        Console.CursorVisible = false;
        return path;
    }

    private void ExportCsv(string path)
    {
        if (path == string.Empty) return;

        try
        {
            using var output = File.CreateText(path);
            output.WriteLine("url,status");

            foreach (var site in BrokenLinks)
            {
                if (site.code == HttpStatusCode.Continue) output.WriteLine($"{site.url},0");
                else output.WriteLine($"{site.url},{(int)site.code}");
            }

            output.Flush();
        }
        catch (Exception)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Export failed, press any key");
            Console.ReadKey(true);
        }
    }
}