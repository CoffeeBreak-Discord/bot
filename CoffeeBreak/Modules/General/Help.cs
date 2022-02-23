using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("help", "List all the command in this bot")]
    public async Task Help([Summary(description: "The command you can know how to use it")] string? command = null)
    {
        var cmdList = _cmd.SlashCommands;
        var embedColor = Global.BotColors.Randomize();
        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(embedColor.IntCode)
            .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
            .WithCurrentTimestamp()
            .WithFooter($"Requested by {this.Context.User}");

        // List command context
        if (command == null)
        {
            // List based Module Name
            Dictionary<string, List<string>> cmdPrint = new Dictionary<string, List<string>>();
            foreach (SlashCommandInfo cmd in cmdList)
            {
                // If data doesn't exists, initialize list
                List<string> cmdPrintList = new List<string>();

                // If data exists, import data from dictionary and delete it
                if (cmdPrint.Where(x => x.Key == cmd.Module.Name).Count() > 0)
                {
                    cmdPrintList = cmdPrint[cmd.Module.Name];
                    cmdPrint.Remove(cmd.Module.Name);
                }

                cmdPrintList.Add(cmd.Module.SlashGroupName == null ? cmd.Name : $"{cmd.Module.SlashGroupName} {cmd.Name}");
                cmdPrint.Add(cmd.Module.Name, cmdPrintList);
            }

            // Print to embed
            foreach (KeyValuePair<string, List<string>> cmd in cmdPrint)
            {
                // Add space into PascalCase, split it, and remove "Module"
                // ref: https://stackoverflow.com/a/3103795
                string[] oldKey = new Regex(@"(?!^)(?=[A-Z])").Replace(cmd.Key, " ").Split(" ");
                string newKey = string.Join(" ", oldKey.Take(oldKey.Count() - 1));
                embed
                    .AddField(newKey, $"`{string.Join("`, `", cmd.Value.ToArray())}`", true)
                    .WithAuthor($"{Global.Bot.PrettyName} Command Menu");
            }
        }

        await this.RespondAsync(embed: embed.Build());
    }
}
