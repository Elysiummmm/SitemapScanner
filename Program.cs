namespace SitemapScanner;

class Program
{
    public static async Task Main(string[] args)
    {
        Logger logger = new();

        if (args.Length == 0)
        {
            logger.WriteError("No arguments provided!");
            return;
        }

        var url = args[^1];
        
        if (!url.EndsWith("sitemap.xml"))
        {
            logger.WriteError("Provided URL isn't a sitemap.xml file!");
            return;
        }
        
        SitemapScanner sitemapScanner = new(url, logger);
        await sitemapScanner.Scan();
    }
}