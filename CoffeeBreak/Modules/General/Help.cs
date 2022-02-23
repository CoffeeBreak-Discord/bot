using System.Linq;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("help", "List all the command in this bot")]
    public async Task Help([Summary(description: "The command you can know how to use it")] string? cmd = null)
    {
        var cmdList = _cmd.SlashCommands.Select(x => $"{x.Module.Name}.{x.MethodName}() -> {x.Module.SlashGroupName ?? "TopLevel"} {x.Name} - {x.Description}");
        Console.WriteLine(string.Join("\n", cmdList));
    }
}