namespace CoffeeBreak.ThirdParty;
public static class Interval
{
    public static System.Timers.Timer SetInterval(Action action, int timeout)
    {
        var timer = new System.Timers.Timer(timeout);
        timer.Elapsed += (s, e) =>
        {
            timer.Enabled = false;
            action();
            timer.Enabled = true;
        };
        timer.Enabled = true;
        return timer;
    }

    public static async void SetInterval(Func<Task> action, TimeSpan timeout)
    {
        await action();
        await Task.Delay(timeout).ConfigureAwait(false);
        SetInterval(action, timeout);
    }

    public static void ClearInterval(System.Timers.Timer timer)
    {
        timer.Stop();
        timer.Dispose();
    }

    public static async Task SetTimeout(Action action, TimeSpan timeout)
    {
        await Task.Delay(timeout).ConfigureAwait(false);
        action();
    }

    public static async Task SetTimeout(Func<Task> action, TimeSpan timeout)
    {
        await Task.Delay(timeout).ConfigureAwait(false);
        await action();
    }
}
