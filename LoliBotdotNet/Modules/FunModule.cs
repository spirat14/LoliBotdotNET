using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoliBotdotNet.Modules
{
    //Module for Fun Commands
    [Summary("Fun")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        public IConfiguration _config { get; set; }

        [Command("ram")]
        [Summary("Send a special image of Ram from Re:Zero")]
        [Remarks("Send a special image of Ram from Re:Zero")]
        public async Task RamCommand()
        {
            //Use imgur link in an embed and send the embed to the user
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 0, 0))
                .WithImageUrl("http://i.imgur.com/DYToB2e.jpg");
            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("cytus")]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        [Summary("Send an image of a character from Cytus")]
        [Remarks("Send a user specified image of a character from Cytus.\n" +
            "__Possible images:__ neko")]
        public async Task CytusCommand(string image)
        {
            //if the user provides input
            if (image != null)
            {
                //Create an object to map different user input keywords to different imgur images
                Dictionary<string, string> urlMap = new Dictionary<string, string>
                {
                    { "neko", "https://i.imgur.com/IxtJtPH.png" },
                };
                //Check if the user input was one of the predefined keywords, if it was grab the associated image and put it in an embed
                string imgPath;
                if (urlMap.TryGetValue(image, out imgPath))
                {
                    var embed = new EmbedBuilder()
                        .WithColor(new Color(255, 0, 0))
                        .WithImageUrl(imgPath);
                    await Context.Channel.SendMessageAsync(embed: embed.Build());
                }
                else
                {
                    //If no image for the specified input exists, tell the user that
                    await Context.Channel.SendMessageAsync(text: $"The image \'{image}\' does not exist. Use {_config["prefix"]}help cytus for more information.");
                }
            }
            //If the user does not provide input
            else
            {
                //Tell the user they need to provide input
                await Context.Channel.SendMessageAsync(text: $"You need to specify an image. Use {_config["prefix"]}help cytus for more information.");
            }
        }

        [Command("thanos")]
        [RequireNsfw]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        [Summary("Send an picture of Thanos")]
        [Remarks("Send a blessed picture of Thanos")]
        public async Task ThanosCommand()
        {
            string imgPath = "https://i.imgur.com/NNLT3rZ.png";
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 0, 0))
                .WithImageUrl(imgPath);
            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("bully")]
        [Summary("Bully Rohan because he deserves it.")]
        [Remarks("Bully Rohan a lot because he deserves it.")]
        public async Task BullyCommand()
        {
            await Context.Channel.SendMessageAsync(text: $"<@!{229431556974837769}> fuck you faggot");
        }

        [Command("reject")]
        [Summary("Reject Talia because she deserves it.")]
        [Remarks("Reject Talia and give her PTSD.")]
        public async Task RejectCommand()
        {
            await Context.Channel.SendMessageAsync(text: $"<@!{308292173957300224}> sorry im gay lol");
        }

        [Command("russianroulette")]
        [Alias("roulette")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Play a game of russian roulette where the punishment is getting kicked.")]
        [Remarks("Start a game of russian roulette with a specied number of bullets in the chamber. If you lose, you get kicked. Bullet count cannot negative, zero, or greater than five.")]
        public async Task RouletteCommand(long bullets)
        {
            long bulletCount = bullets;
            if (bullets > 5) bulletCount = 5;
            if (bullets <= 0) bulletCount = 1;
            bool win = new Random().Next(6) <= bulletCount - 1;
            if (win)
            {
                await Task.Delay(2000);
                await Context.Channel.SendMessageAsync(text: $"you win!");
                IInviteMetadata invite = await Context.Guild.GetTextChannel(Context.Channel.Id).CreateInviteAsync(maxUses: 1);
                await Context.User.SendMessageAsync(text: invite.Url);
                await Context.Guild.GetUser(Context.User.Id).KickAsync(reason: "suicide");
                
            }
            else await Context.Channel.SendMessageAsync(text: $"loser");
        }

        [Command("cocaina")]
        [Alias("coke")]
        [Summary("Find out what kind of drugs the bot does.")]
        [Remarks("Find out what kind of drugs LoliBot does.")]
        public async Task CocainaCommand()
        {
            await Context.Channel.SendMessageAsync(text: $"Hago cocaina");
        }

        [Command("metanfetamina")]
        [Alias("meta")]
        [Summary("Find out what kind of drugs the bot does.")]
        [Remarks("Find out what kind of drugs LoliBot does.")]
        public async Task MetanfetaminaCommand()
        {
            await Context.Channel.SendMessageAsync(text: $"Hago metanfetamina de cristal");
        }

        [Command("monky")]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        [Summary("Send an picture of monky")]
        [Remarks("Send a blessed picture of monky")]
        public async Task MonkyCommand()
        {
            string imgPath = "https://i.imgur.com/gCeZkxv.png";
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 0, 0))
                .WithImageUrl(imgPath);
            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
