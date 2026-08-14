namespace SitemapScanner;

public class Logger
{
    private string logFilePath;
    
    private StreamWriter Log;
    private StreamWriter Error;

    public Logger()
    {
        var now = DateTime.Now;
        logFilePath = $"./{now.Day}-{now.Month}-{now.Year}_{now:HH-MM-ss}";

        Log = File.CreateText(logFilePath + ".log");
        Error = File.CreateText(logFilePath + ".error.log");
    }

    public void WriteLog(string content)
    {
        Console.WriteLine(content);
        Log.WriteLine(content);
    }

    public void WriteError(string content)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(content);
        Console.ResetColor();
        
        Error.WriteLine(content);
    }
}