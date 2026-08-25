namespace SitemapScanner.Rendering;

public abstract class TerminalScreen
{
    protected bool Running = false;
    protected virtual int RenderDelay => 200;

    public async Task Start()
    {
        await BeforeStart();
        
        Running = true;
        Console.CursorVisible = false;

        while (Running)
        {
            Console.SetCursorPosition(0, 0);
            Render();
            await Task.Delay(RenderDelay);
        }
        
        await OnFinished();
        Running = false;
        Console.CursorVisible = true;
    }

    protected static string CenteredText(string text, int width = -1)
    {
        if (width == -1) width = Console.WindowWidth;
        var paddingL = (width - text.Length) / 2;
        var paddingR = width - text.Length - paddingL;
        
        if (paddingR < 0) paddingR = 0;
        if (paddingL < 0) paddingL = 0;

        return new string(' ', paddingL) + text + new string(' ', paddingR);
    }
    
    protected virtual async Task OnFinished() { }
    protected virtual async Task BeforeStart() { }
    protected abstract void Render();
}