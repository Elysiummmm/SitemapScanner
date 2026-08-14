namespace SitemapScanner;

class Program
{
    public static void Main(string[] args)
    {
        SitemapScanner sitemapScanner = new(args[^1]);
    }
}