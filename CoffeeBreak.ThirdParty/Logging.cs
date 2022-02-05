namespace CoffeeBreak.ThirdParty;
public class Logging
{
    /// <summary>
    /// Set default for padding source
    /// </summary>
    public static int MaximumLengthSource = 12;

    public static string LogBuilder(string message, string source)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string fxSource = source.Length > MaximumLengthSource
            ? source.Substring(0, MaximumLengthSource)
            : source;
        fxSource = fxSource.Length < MaximumLengthSource
            ? fxSource.PadRight(MaximumLengthSource, ' ')
            : fxSource;

        return $"{timestamp} {fxSource} {message}";
    }

    public static void Info(string message, string source)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("INFO ");
        Console.WriteLine(LogBuilder(message, source));
    }

    public static void Warning(string message, string source)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("WARN ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(LogBuilder(message, source));
    }

    public static void Error(string message, string source)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("ERR  ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(LogBuilder(message, source));
    }
}
