namespace SitemapScanner;

public class ScanRenderer
{
    private bool shouldUpdate = false;
    private bool running = false;
    
    public ScanRenderer(SitemapScanner sitemapScanner)
    {
        sitemapScanner.OnFinished += Stop;
    }

    public async Task Start()
    {
        running = true;
        
        while (running)
        {
            Console.Clear();
            
            await Task.Delay(100);
        }
    }

    private void Stop()
    {
        running = false;
    }
}