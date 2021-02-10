using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LoliBotdotNET.Services;
using Discord.Commands;

namespace LoliBotdotNET
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _command;
        private readonly IConfiguration _config;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public Program()
        {
            //Load configuration from Json.config
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            _config = _builder.Build();
        }
        public async Task MainAsync()
        {
            //Initialize main client
            var _clientConfig = new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                ExclusiveBulkDelete = true,
                
            };
            _client = new DiscordSocketClient(_clientConfig);

            //Initialize the command service
            var _commandConfig = new CommandServiceConfig
            {
                IgnoreExtraArgs = true,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            };
            _command = new CommandService(_commandConfig);

            //Inisitalize service provider for Dependency Injection
            IServiceProvider _services = new Initialize(_config, _client, _command)
                .BuildServiceProvider();

            //Setup events
            _client.Ready += ReadyAsync;

            //Login bot to Discord servers
            string token = _config["discord_token"];

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //Initialize message handler to start listening for commands
            await _services.GetRequiredService<CommandHandler>().InitializeAsync();

            //Stop the process from ending due to async
            await Task.Delay(-1);
        }

        private Task ReadyAsync()
        {
            //Block for events to be run after bot account login
            return Task.CompletedTask;
        }
    }
}
