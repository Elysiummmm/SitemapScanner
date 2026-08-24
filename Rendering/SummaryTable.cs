namespace SitemapScanner.Rendering;

public class SummaryTable(SitemapScanner sitemapScanner) : TerminalScreen
{
    private SitemapScanner Scanner { get; init; } = sitemapScanner;

    protected override void Render()
    {
        Header();
    }

    private void Header()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
    }
}