using System.Net;
using System.Xml;

namespace SitemapScanner;

public class SitemapScanner
{
    public delegate void EmptyHandler();

    public event EmptyHandler? OnFinished;
        
    private HttpClient HttpClient { get; init; }
    private ReportData Report { get; init; }
    
    public Logger Logger { get; init; }
    
    public string SitemapUrl { get; set; }

    public List<string> SitesToCheck { get; private set; }
    public List<string> SitesChecked { get; private set; }
    
    public SitemapScanner(Logger? logger = null)
    {
        Logger = logger ?? new Logger();
        SitemapUrl = "";

        HttpClient = new HttpClient();
        Report = new ReportData();
        
        SitesToCheck = [];
        SitesChecked = [];
    }

    public async Task Scan(string sitemapUrl)
    {
        SitemapUrl = sitemapUrl;
        SitesToCheck = [];
        SitesChecked = [];
        
        using var response = await GetSitemap(sitemapUrl);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) return;

        Report.SitemapsChecked.Add(sitemapUrl);
        if (content.Contains("sitemapindex"))
        {
            Logger.WriteLog("Found sitemap index");
            await ScanSitemapIndex(await response.Content.ReadAsStreamAsync());
        }
        else if (content.Contains("urlset"))
        {
            Logger.WriteLog("Found sitemap");
            await ScanSitemap(await response.Content.ReadAsStreamAsync());
        }
        else
        {
            Logger.WriteError($"{sitemapUrl} doesn't contain a valid sitemap!");
            return;
        }

        foreach (var site in SitesToCheck)
        {
            await CheckSite(site);
        }
        
        Logger.WriteLog("Scan complete");

        OnFinished?.Invoke();
    }

    private async Task<HttpResponseMessage> GetSitemap(string url)
    {
        Logger.WriteLog($"Scanning {url}...");
        
        HttpResponseMessage response = await HttpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) Logger.WriteError($"Couldn't get sitemap {url}!");

        return response;
    }

    private async Task ScanSitemapIndex(Stream content)
    {
        bool checkNextNode = false;
        var settings = new XmlReaderSettings { Async = true };
        
        using var reader = XmlReader.Create(content, settings);
        while (await reader.ReadAsync())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (reader.Name == "loc") checkNextNode = true;
                    break;
                case XmlNodeType.Text:
                    if (checkNextNode)
                    {
                        checkNextNode = false;
                        
                        var sitemapUrl = await reader.GetValueAsync();
                        var sitemap = await GetSitemap(sitemapUrl);
                        if (!sitemap.IsSuccessStatusCode) continue;
                        
                        Report.SitemapsChecked.Add(sitemapUrl);
                        await ScanSitemap(await sitemap.Content.ReadAsStreamAsync());
                    }

                    break;
            }
        }
    }
    
    private async Task ScanSitemap(Stream content)
    {
        bool checkNextNode = false;
        var settings = new XmlReaderSettings { Async = true };
        
        using var reader = XmlReader.Create(content, settings);
        while (await reader.ReadAsync())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (reader.Name == "loc") checkNextNode = true;
                    break;
                case XmlNodeType.Text:
                    if (checkNextNode)
                    {
                        checkNextNode = false;
                        
                        var siteUrl = await reader.GetValueAsync();
                        SitesToCheck.Add(siteUrl);
                    }

                    break;
            }
        }
    }

    public async Task CheckSite(string siteUrl)
    {
        Report.UrlsChecked++;
        
        HttpResponseMessage response;
        try
        {
            response = await HttpClient.GetAsync(siteUrl);
        }
        catch (HttpRequestException e)
        {
            // im assuming this is the only thing causing this error and i hope im right
            Logger.WriteError($"{siteUrl} couldn't be reached");
            return;
        }
                        
        Logger.WriteLog($"{siteUrl} returned {response.StatusCode}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                var newUrl = response.Headers.Location?.ToString();
                if (newUrl == null) return;

                Logger.WriteLog($"{siteUrl} redirects to {newUrl}");
                await CheckSite(siteUrl);
            }
            else
            {
                Report.BrokenUrls.Add(siteUrl);
                Logger.WriteError($"Site {siteUrl} is broken, got response code {response.StatusCode}");
            }
        }
        
        SitesChecked.Add(siteUrl);
    }
}