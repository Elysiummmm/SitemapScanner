namespace SitemapScanner;

public class Logger
{
    private string logFilePath;
    
    private StreamWriter Log;
    private StreamWriter Error;

    private long startTime;

    public Logger()
    {
        startTime = DateTime.Now.Ticks;
        
        var now = DateTime.Now;
        logFilePath = $"./{now.Day}-{now.Month}-{now.Year}_{now:HH-MM-ss}";

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

    private string GetTimestamp()
    {
        var time = (double)(DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerSecond;
        return $"[{time}] ";
    }
}