using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IronWebScraper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoliBotdotNet.Services
{
    public class HentaiScraper : WebScraper
    {
        public int ID { get; set; }
        private int page;
        private string query;
        private Dictionary<string, object> results = new Dictionary<string, object>();
        public HentaiScraper()
        {
            this.ID = 0;
            this.page = 0;
        }

        public async Task<Dictionary<string, object>> RetreiveHentai(int ID, int page)
        {
            this.ID = ID;
            this.page = page;
            this.query = "";
            await this.StartAsync();
            return results;
        }

        public async Task<int> SearchHentai(string query)
        {
            this.query = query;
            await this.StartAsync();
            return this.ID;
        }

        public override void Init()
        {
            this.LoggingLevel = WebScraper.LogLevel.Critical;
            this.ObeyRobotsDotTxt = false;
            if (this.query.Length > 0) this.Request($"https://nhentai.net/search/?q={this.query}&sort=popular", SearchParse);
            else if (this.page == 0) this.Request($"https://nhentai.net/g/{this.ID}", Parse);
            else this.Request($"https://nhentai.net/g/{this.ID}/{this.page}", Parse);
        }

        public override void Parse(Response response)
        {
            this.results.Add("url", response.RequestlUrl);

            string encodedTagString = response.GetElementsByTagName("script").Last().TextContent;
            string encodedJsonString = encodedTagString.Substring(encodedTagString.IndexOf("\"") + 1, encodedTagString.IndexOf(";") - encodedTagString.IndexOf("\"") - 2).Trim().Trim('"');
            string decodedJsonString = Regex.Unescape(encodedJsonString);
            JObject json = JObject.Parse(decodedJsonString);
            string mediaID = json.Value<string>("media_id");
            if (this.page != 0)
            {
                String imageUrl = response.GetElementsByTagName("img").Last().GetAttribute("src");
                this.results.Add("imageURL", imageUrl);
            }
            else
            {
                this.results.Add("title", json.Value<JToken>("title").Value<string>("english"));
                this.results.Add("numPages", json.Value<string>("num_pages"));
                this.results.Add("uploadTime", response.GetElementsByTagName("time").First().TextContent);
                if (this.page == 0) this.results.Add("imageURL", response.GetElementsByTagName("div").Where((node) => node.GetAttribute("id") == "cover").First().GetElementsByTagName("img").First().GetAttribute("data-src"));

                Dictionary<string, string> tagList = new Dictionary<string, string>();
                tagList.Add("Tags", " ");
                tagList.Add("Language", " ");
                tagList.Add("Artists", " ");
                tagList.Add("Categories", " ");
                tagList.Add("Groups", " ");
                tagList.Add("Parodies", " ");

                foreach (JToken token in json.Value<JToken>("tags").Children())
                {
                    if (token.Value<string>("type") == "tag") tagList["Tags"] += $"{token.Value<string>("name")}, ";
                    if (token.Value<string>("type") == "language") tagList["Language"] += $"{token.Value<string>("name")}, ";
                    if (token.Value<string>("type") == "artist") tagList["Artists"] += $"{token.Value<string>("name")}, ";
                    if (token.Value<string>("type") == "category") tagList["Categories"] += $"{token.Value<string>("name")}, ";
                    if (token.Value<string>("type") == "group") tagList["Groups"] += $"{token.Value<string>("name")}, ";
                    if (token.Value<string>("type") == "parody") tagList["Parodies"] += $"{token.Value<string>("name")}, ";
                }
                tagList["Tags"] = tagList["Tags"].Trim().Trim(',');
                tagList["Language"] = tagList["Language"].Trim().Trim(',');
                tagList["Artists"] = tagList["Artists"].Trim().Trim(',');
                tagList["Categories"] = tagList["Categories"].Trim().Trim(',');
                tagList["Groups"] = tagList["Groups"].Trim().Trim(',');
                tagList["Parodies"] = tagList["Parodies"].Trim().Trim(',');
                this.results.Add("meta", tagList);
            }
        }

        public void SearchParse(Response response)
        {
            HtmlNode[] nodeList = response.QuerySelectorAll(".index-container div a");
            if (nodeList.Length > 1) this.ID = Int32.Parse(Regex.Match(nodeList[new Random().Next(nodeList.Length)].Attributes["href"], @"\d+").Value);
            else this.ID = 0;
        }
    }
}
