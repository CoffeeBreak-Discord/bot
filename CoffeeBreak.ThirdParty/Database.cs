namespace CoffeeBreak.ThirdParty;
public class Database
{
    public static string GenerateConnectionString()
    {
        string server = Environment.GetEnvironmentVariable("MARIADB_HOST") ?? "localhost";
        string database = Environment.GetEnvironmentVariable("MARIADB_DATABASE") ?? "coffeebreak";
        string username = Environment.GetEnvironmentVariable("MARIADB_USERNAME") ?? "root";
        string password = Environment.GetEnvironmentVariable("MARIADB_PASSWORD") ?? "";
        return $"server={server};database={database};user={username};password={password}";
    }
}