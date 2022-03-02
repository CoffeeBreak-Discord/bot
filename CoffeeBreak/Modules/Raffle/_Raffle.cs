using CoffeeBreak.Services;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class RaffleModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseService _db;
    public RaffleModule(DatabaseService db)
    {
        _db = db;
    }
}
