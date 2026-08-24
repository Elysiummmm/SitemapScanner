using SitemapScanner.Rendering;

namespace SitemapScanner;

class Program
{
    public static async Task Main(string[] args)
    {
        Logger logger = new();
        var isValidUrl = false;
        var url = "";

        while (!isValidUrl)
        {
            Console.Write("Link to sitemap.xml file: ");
            url = Console.ReadLine();
        
            isValidUrl = url?.EndsWith("sitemap.xml") ?? false;
            if (!isValidUrl) logger.WriteError("Provided URL isn't valid!", true);
        }

        if (url == null) return;
        
        SitemapScanner sitemapScanner = new(logger);
        ScanRenderer scanRenderer = new(sitemapScanner);
        
        sitemapScanner.Scan(url);
        await scanRenderer.Start();
    }
}