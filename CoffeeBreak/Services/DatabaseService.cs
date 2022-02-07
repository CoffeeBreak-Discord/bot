using CoffeeBreak.Models;

namespace CoffeeBreak.Services;
public class DatabaseService
{
    public Context Context;
    public DatabaseService()
    {
        this.Context = new Context();
    }
}