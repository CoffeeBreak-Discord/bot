using CoffeeBreak.Services;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseService _db;
    public GeneralModule(DatabaseService db)
    {
        _db = db;
    }
}
