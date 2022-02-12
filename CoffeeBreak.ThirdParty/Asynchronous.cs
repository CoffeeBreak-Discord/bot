namespace CoffeeBreak.ThirdParty;
public static class Asynchronous
{
    public static async void SetInterval(Action action, TimeSpan timeout)
    {
        await SetTimeout(action, timeout);
        SetInterval(action, timeout);
    }

    public static async Task SetTimeout(Action action, TimeSpan timeout)
    {
        await Task.Delay(timeout).ConfigureAwait(false);
        action();
    }
}
