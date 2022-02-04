using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;

namespace CoffeeBreak.Services
{
    public class CommandHandlerService
    {
        private readonly DiscordShardedClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public CommandHandlerService(DiscordShardedClient client,
            InteractionService commands, IServiceProvider service)
        {
            _client = client;
            _commands = commands;
            _services = service;
        }

        public async Task InitializeAsync()
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += this.HandleInteraction;
            _client.ShardReady += this.RegisterInteractionToGuild;
        }

        private async Task RegisterInteractionToGuild(DiscordSocketClient client)
        {
#if DEBUG
            await _commands.RegisterCommandsToGuildAsync(Global.Constant.GUIDDevelopment);
#else
            await _commands.RegisterCommandsGloballyAsync();
#endif
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new ShardedInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync()
                        .ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }
    }
}
