using StackExchange.Redis;

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

    public static ConfigurationOptions GenerateRedisConnection()
    {
        string server = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
        string port = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
        string username = Environment.GetEnvironmentVariable("REDIS_USERNAME") ?? "root";
        string password = Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? "";
    
        return new ConfigurationOptions
        {
            EndPoints = { $"{server}:{port}" },
            User = username,
            Password = password
        };
    }
}
