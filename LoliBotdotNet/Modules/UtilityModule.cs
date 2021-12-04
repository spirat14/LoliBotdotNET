using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using LoliBotdotNet.DataObjects;

namespace LoliBotdotNet.Modules
{
    //Module for Utility Commands
    [Summary("Utility")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        //Setup Dependency Injection
        public CommandService _command { get; set; }
        public IConfiguration _config { get; set; }

        //Help Command Definition
        [Command("help")]
        [Summary("Sends a list of available commands")]
        [Remarks("Send a list of all available commands. Optionally, if a command is given, gives in-depth instructions for the specified command.")]
        public async Task HelpCommand(string command = "")
        {
            //Create an embed for replying to original message
            var embed = new EmbedBuilder().WithColor(new Color(255, 0, 0));

            //If no user input
            if (command == "")
            {
                //Generic help command information
                embed.AddField("LoliBot Help", $"For help with a specific command, type {_config["prefix"]}help [command]");

                //Loop through each module to search for and list commands
                foreach (ModuleInfo module in _command.Modules)
                {
                    //Loop through each command and add a new line with command details
                    string value = "";
                    foreach (CommandInfo cmd in module.Commands)
                    {
                        value += $"{cmd.Name} - {cmd.Summary}\n";
                    }
                    embed.AddField(module.Summary, value);
                }

                //Send reply
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
            //If the user provided input
            else
            {
                //Initialize object for storing results from command search function
                SearchResult result = _command.Search(command);
                //If the search finds a matching command name
                if (result.IsSuccess)
                {
                    /*
                     * Check if the function accepts any parameters and form an example input string
                     */

                    //If no parameters
                    if (result.Commands[0].Command.Parameters.Count == 0)
                    {
                        embed.AddField($"{_config["prefix"]}{result.Commands[0].Command.Name}", result.Commands[0].Command.Remarks);
                        await Context.Channel.SendMessageAsync(embed: embed.Build());
                    }
                    //If parameters
                    else
                    {
                        //Cycle through every parameter for the command and add it to the example input string.
                        //If a parameter is optional or accepts infinite arguments, mark it as such and
                        //combine all parameters into one string
                        string paramString = "";
                        for (int i = 0; i < result.Commands[0].Command.Parameters.Count; i++)
                        {
                            string optional = "";
                            string remainder = "";
                            string aliases = "";
                            if (result.Commands[0].Command.Aliases.Count != 0) aliases = String.Concat("(Aliases: ", String.Join(", ", result.Commands[0].Command.Aliases.SkipWhile(alias => alias.Equals(result.Commands[0].Command.Name))), ")");
                            if (result.Commands[0].Command.Parameters[i].IsOptional) optional = "(Optional) ";
                            if (result.Commands[0].Command.Parameters[i].IsRemainder) remainder = "(Remainder) ";
                            paramString += $" [{optional}{remainder}{result.Commands[0].Command.Parameters[i].Name}] {aliases}";
                        }
                        //Add the example input string to the reply embed and send it
                        embed.AddField($"{_config["prefix"]}{result.Commands[0].Command.Name}{paramString}", $"{result.Commands[0].Command.Remarks}");

                        await Context.Channel.SendMessageAsync(embed: embed.Build());
                    }
                }
                else
                {
                    //If no search result is found
                    await Context.Channel.SendMessageAsync(text: $"Error: '{command}' is not a valid command");
                }
            }
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [Summary("Ban a user")]
        [Remarks("Ban the specified user with the specified reason")]
        public async Task BanCommand(string user, [Remainder] string reason = null)
        {
            string pattern = @"<@!?(\d+)>";
            MatchCollection matches = Regex.Matches(user, pattern);
            bool success = UInt64.TryParse(matches[0].Groups[1].Value, out ulong snowflake);

            if (!success) await Context.Channel.SendMessageAsync(text: $"Error: No user was specified");

            if (Context.Guild.GetUser(snowflake) == null)
            {
                await Context.Channel.SendMessageAsync(text: "Cannot find the specified user");
                return;
            }

            var userRoles = Context.Guild.Users.Where(guildUser => guildUser.Id == Context.User.Id).Single().Roles;
            Discord.WebSocket.SocketRole userTopRole = userRoles.First();
            foreach (Discord.WebSocket.SocketRole role in userRoles)
            {
                if (role.Position > userTopRole.Position)
                {
                    userTopRole = role;
                }
            }

            var targetRoles = Context.Guild.GetUser(snowflake).Roles;
            Discord.WebSocket.SocketRole targetTopRole = targetRoles.First();
            foreach (Discord.WebSocket.SocketRole role in targetRoles)
            {
                if (role.Position > targetTopRole.Position)
                {
                    targetTopRole = role;
                }
            }
            if (targetTopRole.Position >= userTopRole.Position)
            {
                await Context.Channel.SendMessageAsync(text: "You cannot ban this user since they have a higher role than you.");
                return;
            }

            await Context.Channel.SendMessageAsync(text: "sayonara nigger");
            await Task.Delay(30000);
            if (Context.Guild.GetUser(snowflake) == null) return;
            await Context.Guild.GetUser(snowflake).BanAsync(0, reason);
            await Context.Channel.SendMessageAsync(text: $"User <@{snowflake}> was banned");
        }

        [Command("unban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [Summary("Unban a user")]
        [Remarks("Unban the specified user")]
        public async Task UnbanCommand(string user)
        {
            string pattern = @"<@!?(\d+)>";
            MatchCollection matches = Regex.Matches(user, pattern);
            bool success = UInt64.TryParse(matches[0].Groups[1].Value, out ulong snowflake);

            if (!success) await Context.Channel.SendMessageAsync(text: $"Error: No user was specified");
            IReadOnlyCollection<Discord.Rest.RestBan> bans = await Context.Guild.GetBansAsync();
            IUser targetUser = bans.Where(ban => ban.User.Id == snowflake).First().User;

            await Context.Guild.RemoveBanAsync(targetUser);
            await Context.Channel.SendMessageAsync(text: $"User <@{snowflake}> was unbanned");
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [Summary("Kick a user")]
        [Remarks("Kick the specified user with the specified reason")]
        public async Task KickCommand(string user, [Remainder] string reason = null)
        {
            string pattern = @"<@!?(\d+)>";
            MatchCollection matches = Regex.Matches(user, pattern);
            bool success = UInt64.TryParse(matches[0].Groups[1].Value, out ulong snowflake);

            if (!success) await Context.Channel.SendMessageAsync(text: $"Error: No user was specified");

            if (Context.Guild.GetUser(snowflake) == null) {
                await Context.Channel.SendMessageAsync(text: "Cannot find the specified user");
                return;
            }

            var userRoles = Context.Guild.Users.Where(guildUser => guildUser.Id == Context.User.Id).Single().Roles;
            Discord.WebSocket.SocketRole userTopRole = userRoles.First();
            foreach (Discord.WebSocket.SocketRole role in userRoles)
            {
                if (role.Position > userTopRole.Position)
                {
                    userTopRole = role;
                }
            }

            var targetRoles = Context.Guild.GetUser(snowflake).Roles;
            Discord.WebSocket.SocketRole targetTopRole = targetRoles.First();
            foreach (Discord.WebSocket.SocketRole role in targetRoles)
            {
                if (role.Position > targetTopRole.Position)
                {
                    targetTopRole = role;
                }
            }
            if (targetTopRole.Position >= userTopRole.Position)
            {
                await Context.Channel.SendMessageAsync(text: "You cannot kick this user since they have a higher role than you.");
                return;
            }


            await Context.Guild.GetUser(snowflake).KickAsync(reason);
            await Context.Channel.SendMessageAsync(text: $"User <@{snowflake}> was kicked");
        }

        [Command("role")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Summary("Add or remove a role for a user")]
        [Remarks("Add or remove the specified role for the specified user")]
        public async Task AddRoleCommand(string user, string role)
        {
            MatchCollection userMatches = Regex.Matches(user, @"<@!?(\d+)>");
            MatchCollection roleMatches = Regex.Matches(role, @"<@&(\d+)>");
            bool successUser = UInt64.TryParse(userMatches[0].Groups[1].Value, out ulong userSnowflake);
            bool successRole = UInt64.TryParse(roleMatches[0].Groups[1].Value, out ulong roleSnowflake);

            if (successUser && successRole)
            {
                try
                {
                    if (Context.Guild.Users.Any(u => u.Id == userSnowflake))
                    {
                        if (Context.Guild.Roles.Any(r => r.Id == roleSnowflake))
                        {
                            if (Context.Guild.GetUser(userSnowflake).Roles.Any(p => p.Id == roleSnowflake))
                            {
                                await Context.Guild.GetUser(userSnowflake).RemoveRoleAsync(Context.Guild.GetRole(roleSnowflake));
                                await Context.Channel.SendMessageAsync(text: $"Role <@&{roleSnowflake}> was removed from user <@{userSnowflake}>");
                            }
                            else
                            {
                                await Context.Guild.GetUser(userSnowflake).AddRoleAsync(Context.Guild.GetRole(roleSnowflake));
                                await Context.Channel.SendMessageAsync(text: $"User <@{userSnowflake}> was given the role <@&{roleSnowflake}>");
                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync(text: $"Error: Role <@&{roleSnowflake}> could not be found in this server");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(text: $"Error: User <@{userSnowflake}> could not be found in this server");
                    }
                }
                catch
                {
                    if (!successUser) await Context.Channel.SendMessageAsync(text: $"Error: Role <@&{roleSnowflake}> failed to be given/removed for user <@{userSnowflake}>");
                }
            }
            else
            {
                //If no user is specified
                await Context.Channel.SendMessageAsync(text: $"Error: Either a user or a role was not specified. Use the format '<role user role'");
            }
        }

        [Command("eval")]
        [RequireOwner]
        [Summary("Evaluate the provided line of code")]
        [Remarks("Evaluate the specified line of code and return the result")]
        public async Task EvalCommand([Remainder] string code)
        {
            /*string assemblyCode;
            if (!code.Contains('\n'))
            {
                assemblyCode = "namespace LoliBotdotNet.Modules { public class Bar { public object evalMethod(Discord.Commands.SocketCommandContext Context) { return " + code + "; } } }";
            }
            else
            {
                string[] scripts = code.Split('\n');
                string scriptString = String.Join("; ", scripts);
                assemblyCode = "using Discord; using Discord.Commands; namespace LoliBotdotNet.Modules { public class Bar { public object evalMethod(Discord.Commands.SocketCommandContext Context) { " + scriptString + " } } }";
            }
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters() { GenerateExecutable = false, GenerateInMemory = true, };
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, assemblyCode);
            provider.Dispose();

            if (cr.Errors.Count > 0)
            {
                string errors = String.Empty;
                foreach (CompilerError error in cr.Errors) {
                    errors += (error.ErrorText + "\n");
                }
                await Context.Channel.SendMessageAsync(text: $"```{errors}```");
            }
            else
            {
                object o = cr.CompiledAssembly.CreateInstance("LoliBotdotNet.Modules.Bar");
                MethodInfo mi = o.GetType().GetMethod("evalMethod");
                object methodInvoke = mi.Invoke(o, new object[] { Context });
                await Context.Channel.SendMessageAsync(text: $"```{methodInvoke.ToString()}```");
            }*/
            SocketCommandContext c = Context;
            var result = await CSharpScript.EvaluateAsync(code, globals: new Globals { Context = Context });
            if (result == null)
            {
                await Context.Channel.SendMessageAsync(text: $"```null```");
                return;
            }
            await Context.Channel.SendMessageAsync(text: $"```{(result.GetType().Equals(typeof(Exception)) ? new Exception(result.ToString()).Message : result.ToString())}```");
        }

        [Command("purge")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [Summary("Delete a specified number of previous messages")]
        [Remarks("Delete a specified number of messages. The command message is also deleted but is not counted.")]
        private async Task PurgeCommand(int count)
        {
            try
            {
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(count + 1).FlattenAsync();
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                await Context.Channel.SendMessageAsync(text: $"{count} message were deleted");
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(text: $"An error occured so the specified number of messages could not be deleted.\n {e.Message}");
            }
        }

        [Command("createroom")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("God")]
        [Remarks("God")]
        public async Task CreateRoomCommand()
        {
            await Context.Guild.CreateVoiceChannelAsync("Create Private Room", x => {
                x.UserLimit = 0;
            });
        }
    }
}
