using CoffeeBreak.Services;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseService _db;
    private InteractionService _cmd;
    public GeneralModule(DatabaseService db, InteractionService cmd)
    {
        _db = db;
        _cmd = cmd;
    }
}
