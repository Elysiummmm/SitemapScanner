namespace SitemapScanner;

public class SitemapScanner
{
    public Logger Logger { get; init; }
    public string SitemapUrl { get; init; }

    public SitemapScanner(string sitemapUrl)
    {
        Logger = new Logger();
        SitemapUrl = sitemapUrl;
    }
}