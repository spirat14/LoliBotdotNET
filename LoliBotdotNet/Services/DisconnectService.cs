using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoliBotdotNet.Services
{
    class DisconnectService
    {
        public DisconnectService(DiscordSocketClient client, CommandService command)
        {
            client.UserVoiceStateUpdated += DisconnectAsync;
        }

        private async Task<Task> DisconnectAsync(SocketUser user, SocketVoiceState stateStart, SocketVoiceState stateEnd)
        {
            SocketGuildUser guildUser = user.MutualGuilds.FirstOrDefault((guild) => guild.Id == 388797015376658442).GetUser(265899771380629504);
            if (guildUser.VoiceChannel != null && guildUser.IsSelfDeafened)
            {
                await Task.Delay(600000);
                if (guildUser.VoiceChannel != null && guildUser.IsSelfDeafened)
                {
                    await guildUser.ModifyAsync((properties) =>
                    {
                        properties.Channel = null;
                    });
                }
            }
            return Task.CompletedTask;
        }
    }
}
