using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace LoliBotdotNET.Services
{
    public class CommandHandler
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfiguration>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _services = services;
        }

        public async Task InitializeAsync()
        {
            /*
             * Pass the service provider to the second parameter of
             * AddModulesAsync to inject dependencies to all modules
             * that may require them.
             */
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            //Don't process the command if it was a system message
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return;

            //Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            char prefix = Char.Parse(_config["prefix"]);
            string pattern = @"\(\s*(\d{5,})\s*(\d+)?\s*\)";
            MatchCollection matches = Regex.Matches(message.Content, pattern);

            //make sure no bots trigger commands
            if (message.Author.IsBot) return;

            //Determine if the message is a command
            if (!message.HasCharPrefix(prefix, ref argPos))
            {
                if (matches.Count != 0 && matches[0].Success)
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        int ID = Int32.Parse(matches[i].Groups[1].Value);
                        int page = 0;
                        if (matches[i].Groups[2].Success) page = Int32.Parse(matches[i].Groups[2].Value);

                        //Handle nhentai integration
                        HentaiScraper scraper = new HentaiScraper();
                        Dictionary<string, object> results = await scraper.RetreiveHentai(ID, page);
                        try
                        {

                            var embed = new EmbedBuilder()
                                .WithColor(new Color(255, 0, 0))
                                .WithTitle(ID.ToString())
                                .WithUrl((string)results["url"])
                                .AddField("Title", results["title"]);

                            foreach (KeyValuePair<string, string> s in (Dictionary<string,string>)results["meta"])
                            {
                                if (s.Value.Count() != 0) embed.AddField(s.Key, s.Value);
                            }

                            embed
                                .AddField("Number of pages:", (string)results["numPages"])
                                .AddField("Uploaded", (string)results["uploadTime"]);

                            SocketGuildChannel channel = message.Channel as SocketGuildChannel;
                            if (channel.Guild.GetTextChannel(message.Channel.Id).IsNsfw) embed.WithImageUrl((string)results["imageURL"]);
                            embed.WithFooter("min 5 digits");
                            await msg.Channel.SendMessageAsync(embed: embed.Build());
                        }
                        catch
                        {
                            try
                            {
                                if (page != 0)
                                {
                                    var embed = new EmbedBuilder();
                                    embed.AddField($"/g/{ID}/{page}", results["imageURL"]);
                                    embed.WithImageUrl((string)results["imageURL"]);
                                    embed.WithColor(new Color(255, 0, 0));
                                    await msg.Channel.SendMessageAsync(embed: embed.Build());
                                }
                                else throw new Exception();
                            }
                            catch
                            {
                                await msg.Channel.SendMessageAsync(text: $"There was a error fetching '{ID}'");
                            }
                        }
                    }
                }
                else return;
            }

            //Create a WebSocket-based command context if the message is a command
            var context = new SocketCommandContext(_client, message);
            /*
             *Execute the command with the command context we just
             * created, along with the service provideer for precondition checks
             */
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);
        }
    }
}