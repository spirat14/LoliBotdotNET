using System;
using System.Collections.Generic;
using NekosSharp;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LoliBotdotNET.Services;
using System.Net;
using System.Linq;
using Reddit;
using Reddit.Inputs.Search;

namespace LoliBotdotNET.Modules
{
    [Summary("Integration Commands")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class IntegrationModule : ModuleBase<SocketCommandContext>
    {
        private readonly NekoClient nekoClient = new NekoClient("LoliBot");
        public IConfiguration _config { get; set; }

        [Command("neko")]
        [Summary("Send an image of a neko")]
        [RequireNsfw]
        [Remarks("Pulls images from Nekos.Life using the Wrapper API. If provided, user specified tags will provide an image associated with the provided tag.\n\n" +
            "__Allowed tags:__ neko, nekogif, fox, holo, avatar, baka, waifu, wallpaper, smug, " +
            "tickle, poke, kiss, slap, cuddle, hug, pat, feed, lewd, hentai, hentaigif, femdom, " +
            "classicgif, feet, feetgif, lewdfeet, lewdneko, kunigif, boobs, boobsgif, smallboobs, pussy, pussygif, cum, cumgif, spank, solo, " +
            "sologif, blowjob, blowjobgif, yuri, yurigif, lewdyuri, trap, analgif, futa, pwankgif,  lewdholo, lewdfox, " +
            "lizard, dog, cat, goose, 8ball\n" +
            "__Temporarily Unavailable:__ nekoavatar, classic, ahegao,  cosplay, anal, erowallpaper, bdsm, piersing, pantyhose, peeing, smallboobs, yiff, yiffgif")]
        public async Task NekoCommand(string tag = "neko")
        {
            /*
             * Swap user input into lower case and run it through a switch case to check if it matches any
             * NekosSharp API Wrapper commands. If it does, grab an image from the wrapper using that command.
             */
            string category = tag.ToLower();
            Request req;
            switch (category)
            {
                case "neko":
                    req = await nekoClient.Image.Neko();
                    break;
                case "nekogif":
                    req = await nekoClient.Image.NekoGif();
                    break;
                case "fox":
                    req = await nekoClient.Image.Fox();
                    break;
                case "holo":
                    req = await nekoClient.Image.Holo();
                    break;
                case "avatar":
                    req = await nekoClient.Image.Avatar();
                    break;
                case "baka":
                    req = await nekoClient.Image.Baka();
                    break;
                case "waifu":
                    req = await nekoClient.Image.Waifu();
                    break;
                /*case "nekoavatar":
                    req = await nekoClient.Image_v3.NekoAvatar();
                    break;*/
                case "wallpaper":
                    req = await nekoClient.Image.Wallpaper();
                    break;
                case "smug":
                    req = await nekoClient.Image.Smug();
                    break;
                case "tickle":
                    req = await nekoClient.Action.Tickle();
                    break;
                case "poke":
                    req = await nekoClient.Action.Poke();
                    break;
                case "kiss":
                    req = await nekoClient.Action.Kiss();
                    break;
                case "slap":
                    req = await nekoClient.Action.Slap();
                    break;
                case "cuddle":
                    req = await nekoClient.Action.Cuddle();
                    break;
                case "hug":
                    req = await nekoClient.Action.Hug();
                    break;
                case "pat":
                    req = await nekoClient.Action.Pat();
                    break;
                case "feed":
                    req = await nekoClient.Action.Feed();
                    break;
                case "lewd":
                    req = await nekoClient.Nsfw.Lewd();
                    break;
                /*case "ahegao":
                    req = await nekoClient.Nsfw_v3.Ahegao();
                    break;*/
                case "femdom":
                    req = await nekoClient.Nsfw.Femdom();
                    break;
                /*case "cosplay":
                    req = await nekoClient.Nsfw_v3.Cosplay();
                    break;
                case "classic":
                    req = await nekoClient.Nsfw_v3.Classic();
                    break;*/
                case "classicgif":
                    req = await nekoClient.Nsfw.ClassicGif();
                    break;
                case "feet":
                    req = await nekoClient.Nsfw.Feet();
                    break;
                case "feetgif":
                    req = await nekoClient.Nsfw.FeetGif();
                    break;
                case "lewdfeet":
                    req = await nekoClient.Nsfw.LewdFeet();
                    break;
                case "lewdneko":
                    req = await nekoClient.Nsfw.LewdNeko();
                    break;
                case "kunigif":
                    req = await nekoClient.Nsfw.KuniGif();
                    break;
                case "boobs":
                    req = await nekoClient.Nsfw.Boobs();
                    break;
                case "boobsgif":
                    req = await nekoClient.Nsfw.BoobsGif();
                    break;
                case "pussy":
                    req = await nekoClient.Nsfw.Pussy();
                    break;
                case "pussygif":
                    req = await nekoClient.Nsfw.PussyGif();
                    break;
                case "cum":
                    req = await nekoClient.Nsfw.Cum();
                    break;
                case "cumgif":
                    req = await nekoClient.Nsfw.CumGif();
                    break;
                case "spank":
                    req = await nekoClient.Nsfw.Spank();
                    break;
                case "hentai":
                    req = await nekoClient.Nsfw.Hentai();
                    break;
                case "hentaigif":
                    req = await nekoClient.Nsfw.HentaiGif();
                    break;
                case "solo":
                    req = await nekoClient.Nsfw.Solo();
                    break;
                case "sologif":
                    req = await nekoClient.Nsfw.SoloGif();
                    break;
                case "blowjob":
                    req = await nekoClient.Nsfw.Blowjob();
                    break;
                case "blowjobgif":
                    req = await nekoClient.Nsfw.BlowjobGif();
                    break;
                case "yuri":
                    req = await nekoClient.Nsfw.Yuri();
                    break;
                case "yurigif":
                    req = await nekoClient.Nsfw.YuriGif();
                    break;
                case "lewdyuri":
                    req = await nekoClient.Nsfw.LewdYuri();
                    break;
                case "trap":
                    req = await nekoClient.Nsfw.Trap();
                    break;
                /*case "anal":
                    req = await nekoClient.Nsfw_v3.Anal();
                    break;*/
                case "analgif":
                    req = await nekoClient.Nsfw.AnalGif();
                    break;
                /*case "erowallpaper":
                    req = await nekoClient.Nsfw_v3.EroWallpaper();
                    break;*/
                case "futa":
                    req = await nekoClient.Nsfw.Futanari();
                    break;
                case "pwankgif":
                    req = await nekoClient.Nsfw.PwankGif();
                    break;
                /*case "bdsm":
                    req = await nekoClient.Nsfw.Bdsm();
                    break;*/
                case "lewdholo":
                    req = await nekoClient.Nsfw.LewdHolo();
                    break;
                case "lewdfox":
                    req = await nekoClient.Nsfw.LewdFox();
                    break;
                /*case "pantyhose":
                    req = await nekoClient.Nsfw_v3.Pantyhose();
                    break;
                case "piersings":
                    req = await nekoClient.Nsfw_v3.Piersing();
                    break;
                case "peeing":
                    req = await nekoClient.Nsfw_v3.Peeing();
                    break;
                case "smallboobs":
                    req = await nekoClient.Nsfw_v3.SmallBoobs();
                    break;
                case "yiff":
                    req = await nekoClient.Nsfw_v3.Yiff();
                    break;
                case "yiffgif":
                    req = await nekoClient.Nsfw_v3.YiffGif();
                    break;*/
                case "lizard":
                    req = await nekoClient.Misc.Lizard();
                    break;
                case "dog":
                    req = await nekoClient.Misc.Dog();
                    break;
                case "cat":
                    req = await nekoClient.Misc.Cat();
                    break;
                case "goose":
                    req = await nekoClient.Misc.Goose();
                    break;
                case "8ball":
                    req = await nekoClient.Misc.EightBall();
                    break;
                default:
                    req = await nekoClient.Image.Neko();
                    break;
            }
            if (req.Success)
            {
                //If an image is received, put it in an embed and reply to the user
                var embed = new EmbedBuilder();
                embed.AddField("Neko Results", req.ImageUrl)
                    .WithColor(new Color(255, 0, 0))
                    .WithImageUrl(req.ImageUrl);
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
            else
            {
                //If no image is received, write the error to console and reply to user
                Console.WriteLine($"An error ocurred while trying to fetch an image with exception: {req.Code} {req.Error}");
                await Context.Channel.SendMessageAsync(text: "An error ocurred while trying to fetch the image");
            }
        }

        [Command("nh")]
        [RequireNsfw]
        [Summary("Searches for a hentai on nHentai")]
        [Remarks("Uses the provided query to search for a hentai on nHentai.")]
        public async Task nhCommand([Remainder] string query)
        {
            HentaiScraper scraper = new HentaiScraper();
            scraper.HttpRetryAttempts = 0;
            int ID = await scraper.SearchHentai(WebUtility.UrlEncode(query));
            Dictionary<string, object> results = await scraper.RetreiveHentai(ID, 0);
            try
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(255, 0, 0))
                    .WithTitle(ID.ToString())
                    .WithUrl((string)results["url"])
                    .AddField("Title:", (string)results["title"]);

                foreach (KeyValuePair<string, string> s in (Dictionary<string, string>)results["meta"])
                {
                    if (s.Value.Count() != 0) embed.AddField(s.Key, s.Value);
                }

                embed
                    .AddField("Number of pages:", (string)results["numPages"])
                    .AddField("Uploaded", (string)results["uploadTime"])
                    .WithImageUrl((string)results["imageURL"])
                    .WithFooter("min 5 digits");
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync(text: $"No results were found with the query \"{query}\"");
            }
        }

        [Command("reddit")]
        [Alias("r")]
        [Summary("Pulls posts from reddit")]
        [Remarks("Pull a post from the specified subreddit. If no sort modifer is provided, the command will default to hot.")]
        public async Task redditCommand(string query, string sortModifier = "hot")
        {
            var embed = new EmbedBuilder().WithColor(new Color(255, 0, 0)).WithColor(new Color(255, 0, 0));
            RedditClient reddit = new RedditClient(appId: _config["reddit_app_id"], appSecret: _config["reddit_secret"], refreshToken: _config["reddit_refresh_token"]);
            try {
                var a = reddit.SubredditAutocompleteV2(query, includeOver18: true, includeProfiles: false, includeCategories: false).Where(x => x.Name.ToLowerInvariant() == query.ToLowerInvariant()).First();
                if (a.Name.ToLower() == query.ToLower())
                {
                    List<Reddit.Controllers.Post> posts;
                    switch (sortModifier)
                    {
                        case "best":
                            posts = a.Posts.Best;
                            break;
                        case "top":
                            posts = a.Posts.Top;
                            break;
                        case "new":
                            posts = a.Posts.New;
                            break;
                        case "controversial":
                            posts = a.Posts.Controversial;
                            break;
                        case "rising":
                            posts = a.Posts.Rising;
                            break;
                        default:
                            posts = a.Posts.Hot;
                            break;
                    }
                    var post = posts[new Random().Next(posts.Count())];
                    if (!Context.Guild.GetTextChannel(Context.Channel.Id).IsNsfw && a.Over18.Value)
                    {
                        await Context.Channel.SendMessageAsync(text: "This subreddit is nsfw and posts cannot be sent in a sfw channel.");
                        return;
                    }
                    int i = 0;
                    while (!Context.Guild.GetTextChannel(Context.Channel.Id).IsNsfw && post.NSFW)
                    {
                        post = posts[new Random().Next(posts.Count())];
                        i++;
                        if (i >= 15)
                        {
                            await Context.Channel.SendMessageAsync(text: "A sfw post could not be found.");
                            return;
                        } 
                    }
                    
                    embed.WithColor(new Color(255, 0, 0))
                        .WithTitle(post.Title)
                        .WithUrl($"https://reddit.com{post.Permalink}")
                        .WithImageUrl(post.Listing.URL);
                    await Context.Channel.SendMessageAsync(embed: embed.Build());
                    return;
                }
            }
            catch {
                await Context.Channel.SendMessageAsync(text: $"The subreddit '{query}' does not exist or has no posts.");
                return;
            }

        }
    }
}
