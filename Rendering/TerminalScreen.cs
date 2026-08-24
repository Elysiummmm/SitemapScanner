namespace SitemapScanner.Rendering;

public abstract class TerminalScreen
{
    protected bool Running = false;

    public async Task Start()
    {
        Running = true;
        Console.CursorVisible = false;

        while (Running)
        {
            Console.SetCursorPosition(0, 0);
            Render();
            await Task.Delay(200);
        }
        
        await OnFinished();
        Running = false;
        Console.CursorVisible = true;
    }

    protected virtual async Task OnFinished() { }
    protected abstract void Render();
}