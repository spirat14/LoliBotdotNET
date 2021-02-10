using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LoliBotdotNET.Services
{
    public class Initialize
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private LoggingService _logger;

        /*
         * Ask if there are existing CommandService and DiscordSocketClient
         * instance. If there are, we retrieve them and add them to the
         * DI container; if not, we create our own.
         */
        public Initialize(IConfiguration config, DiscordSocketClient client = null, CommandService commands = null)
        {
            _config = config;
            _client = client ?? new DiscordSocketClient();
            _commands = commands ?? new CommandService();
            _logger = new LoggingService(_client, _commands);
        }

        //Add dependencies to collection and build a ServiceProvider from that collection
        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_config)
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton(_logger)
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
    }
}