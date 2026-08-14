namespace SitemapScanner;

public class Logger
{
    private string logFilePath;
    
    private StreamWriter Log;
    private StreamWriter Error;

    private long startTime;

    public Logger(string logFileLoc = ".")
    {
        startTime = DateTime.Now.Ticks;
        
        var now = DateTime.Now;
        logFilePath = $"{logFileLoc}/{now.Day}-{now.Month}-{now.Year}_{now:HH-MM-ss}";

        Log = File.CreateText(logFilePath + ".log");
        Error = File.CreateText(logFilePath + ".error.log");
        Log.AutoFlush = Error.AutoFlush = true;
    }

    public void WriteLog(string content)
    {
        var timestamp = GetTimestamp();
        
        Console.WriteLine(timestamp + content);
        Log.WriteLine(timestamp + content);
    }

    public void WriteError(string content)
    {
        var timestamp = GetTimestamp();
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(timestamp + "[ERROR] " + content);
        Console.ResetColor();
        
        Log.WriteLine(timestamp + "[ERROR] " + content);
        Error.WriteLine(timestamp + "[ERROR] " + content);
    }

    public void GenerateReport(ReportData report)
    {
        using var reportFile = File.CreateText(logFilePath + ".report.log");
        var timeTaken = (double)(DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerSecond;
        var brokenPercentage = report.BrokenUrls.Count == 0 ? 0 : Math.Round((double)report.UrlsChecked / report.BrokenUrls.Count * 100);
        
        reportFile.WriteLine($"Time taken: {Math.Floor(timeTaken / 60)} minutes {Math.Floor(timeTaken % 60)} seconds");
        reportFile.WriteLine($"URLs checked: {report.UrlsChecked}");
        reportFile.WriteLine($"Broken URLs: {report.BrokenUrls.Count} ({brokenPercentage}%)\n");
        
        reportFile.WriteLine("SITEMAPS CHECKED");
        reportFile.WriteLine("================");
        foreach (var url in report.SitemapsChecked) reportFile.WriteLine(url);
        
        reportFile.Write("\n");
        
        reportFile.WriteLine("BROKEN LINKS");
        reportFile.WriteLine("============\n");
        foreach (var url in report.BrokenUrls) reportFile.WriteLine(url);
        
        reportFile.Flush();
    }

    private string GetTimestamp()
    {
        var time = (double)(DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerSecond;
        return $"[{time,9}] ";
    }
}