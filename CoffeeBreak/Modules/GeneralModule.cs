using Discord.Interactions;

namespace CoffeeBreak.Modules
{
    public class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
    {
        [SlashCommand("ping", "Ping!")]
        public async Task Ping()
        {
            await this.RespondAsync("Pong!");
        }

        [SlashCommand("respond", "Talk as bot")]
        public async Task Respond(string message)
        {
            await this.RespondAsync(message);
        }
    }
}
