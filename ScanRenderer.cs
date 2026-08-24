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
        
        while (running)
        {
            Render();
            await Task.Delay(100);
        }
    }

    private void Stop()
    {
        running = false;
        Render();
    }

    private void Render()
    {
        Console.Clear();
        StatusBar();
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

        var percentage = (double)Scanner.SitesChecked.Count / Scanner.SitesToCheck.Count * 100;
        var countText = $"{percentage:F1}% ({Scanner.SitesChecked.Count} / {Scanner.SitesToCheck.Count})";
        countText = countText.PadLeft(countCharacters, ' ');
        
        statusBar.Append(urlText);
        statusBar.Append(countText);
        
        Console.WriteLine(statusBar.ToString());
        Console.ResetColor();
    }
}