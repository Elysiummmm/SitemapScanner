using System.Net;
using System.Text;

namespace SitemapScanner;

public class ScanRenderer
{
    private bool running = false;
    
    private SitemapScanner Scanner { get; init; }
    
    public ScanRenderer(SitemapScanner sitemapScanner)
    {
        Scanner = sitemapScanner;
        Scanner.OnFinished += Stop;
    }

    public async Task Start()
    {
        running = true;
        Console.CursorVisible = false;
        
        while (running)
        {
            Render();
            await Task.Delay(200);
        }
    }

    private void Stop()
    {
        running = false;
        Render();
    }

    private void Render()
    {
        Console.SetCursorPosition(0, 0);
        
        StatusBar();
        InProgressTable();
        
        Console.ResetColor();
    }

    private void StatusBar()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;

        var urlCharacters = (int)(Console.WindowWidth * 0.7);
        var countCharacters = Console.WindowWidth - urlCharacters;
        var statusBar = new StringBuilder();

        var urlText = Scanner.SitemapUrl.PadRight(urlCharacters, ' ');
        if (urlText.Length > urlCharacters) urlText = "..." + urlText[3..(urlCharacters - 3)];

        var countText = $"{Scanner.SitesChecked.Count} / {Scanner.SitesToCheck.Count}";
        countText = countText.PadLeft(countCharacters, ' ');
        
        statusBar.Append(urlText);
        statusBar.Append(countText);
        
        Console.Write(statusBar.ToString());
    }

    private void InProgressTable()
    {
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        
        var statusCharacters = 6;
        var urlCharacters = Console.WindowWidth - statusCharacters;
        var tableRows = Console.WindowHeight - 1;

        for (var row = 0; row < tableRows; row++)
        {
            Console.SetCursorPosition(0, row + 1);
            var rowText = new StringBuilder();

            try
            {
                var rowData = Scanner.SitesChecked[^row];
                
                Console.ForegroundColor = rowData.status != HttpStatusCode.OK ? ConsoleColor.Red : ConsoleColor.White;
                
                var urlText = rowData.url.PadRight(urlCharacters, ' ');
                if (urlText.Length > urlCharacters) urlText = "..." + urlText[3..(urlCharacters - 3)];

                var statusText = $"{(int)rowData.status} ";
                statusText += Enum.GetName(rowData.status) ?? "???";
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
        }
    }
}