using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("respond", "Talk as bot")]
    public async Task Respond(string message)
    {
        await this.RespondAsync($"<@!{this.Context.User.Id}> said: {message}");
    }
}
