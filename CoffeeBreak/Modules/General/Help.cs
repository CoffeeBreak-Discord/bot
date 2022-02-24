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
            .WithCurrentTimestamp()
            .WithFooter($"Requested by {this.Context.User}");

        // Show menu help
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
                    .WithDescription("Type `/help <command>` to get more info on a specific command.")
                    .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
                    .WithAuthor($"{Global.Bot.PrettyName} Command Menu");
            }
        }
        // Show command help
        else 
        {
            var searchCmd = cmdList.Where(x => x.Name == command || $"{x.Module.SlashGroupName} {x.Name}" == command);
            
            // If command found
            if (searchCmd.Count() > 0)
            {
                var ctxCmd = searchCmd.First();
                string cmdName = "/" + (ctxCmd.Module.SlashGroupName == null ? ctxCmd.Name : $"{ctxCmd.Module.SlashGroupName} {ctxCmd.Name}");

                // Generate usage & description
                List<string> usages = new List<string>();
                List<string> information = new List<string>();
                List<string> description = new List<string>();
                if (ctxCmd.Parameters.Count() > 0)
                {
                    bool notHiddenParams(Type param) => param != typeof(IChannel) || param != typeof(IUser);
                    foreach (var param in ctxCmd.Parameters)
                    {
                        var paramPrint = param.IsRequired ? $"<{param.Name}>" : $"[{param.Name}]";
                        if (notHiddenParams(param.ParameterType.GetType())) usages.Add(paramPrint);

                        // Check if parameter have [ChannelTypes] and [Summary]
                        if (param.ChannelTypes.Count() > 0)
                        {
                            var chanType = param.ChannelTypes.Select(x => x.ToString());
                            information.Add(
                                $"• This command can be executed **only** in `{string.Join("`, `", chanType)}` channel.");
                        }
                        if (param.Description != null)
                        {
                            if (notHiddenParams(param.ParameterType.GetType())) description.Add($"• `{paramPrint}` -> {param.Description}");
                        }
                    }
                }
                // Generate information about precondition
                if (ctxCmd.Preconditions.Count() > 0)
                {
                    foreach (var precon in ctxCmd.Preconditions)
                    {
                        void PrintPrecondition<T>(Func<T, string> text) where T : PreconditionAttribute
                        {
                            var format = precon as T;
                            if (format != null) information.Add(text(format));
                        };

                        PrintPrecondition<RequireBotPermissionAttribute>(x =>
                            $"• The bot must have `{x.GuildPermission}{(x.GuildPermission != null && x.ChannelPermission != null ? "` and `" : "")}{x.ChannelPermission}` permission.");
                        PrintPrecondition<RequireNsfwAttribute>(x =>
                            $"• This command is dirty, use `NSFW Channel` when execute this command.");
                        PrintPrecondition<RequireRoleAttribute>(x =>
                            $"• The executor of command must have `{x.RoleName}` role before execute this command.");
                            //$"• The executor of command must have <@&{x.RoleID}> role before execute this command.");
                        PrintPrecondition<RequireUserPermissionAttribute>(x =>
                            $"• The executor of command must have `{x.GuildPermission}` permission before execute this command.");
                    }
                }

                embed
                    .WithAuthor(cmdName)
                    .WithThumbnailUrl("https://media.discordapp.net/attachments/946050537814655046/946050592596447262/question_mark.png")
                    .WithDescription(ctxCmd.Description ?? "No description")
                    .AddField("Usage", $"{cmdName} {string.Join(' ', usages.ToArray())}{(usages.Count() > 0 ? "\n\nParameter `<param>` is required. Otherwise, parameter `[param]` is optional." : "")}");
                
                if (description.Count() > 0) embed.AddField("Description", string.Join("\n", description.ToArray()));
                if (information.Count() > 0) embed.AddField("Information", string.Join("\n", information.ToArray()));
            }
            // If command not found
            else embed.WithAuthor("Command not found!").WithDescription("Please check again.");
        }

        await this.RespondAsync(embed: embed.Build());
    }
}
