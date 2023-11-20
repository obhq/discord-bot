using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oboteration.Models;
using Newtonsoft.Json;
using System.Configuration;

namespace Oboteration.Funcs
{
    public class Github
    {
        public async Task<string?[]> getGithubInfo(string TitleID)
        {
            string?[] info = new string[2];

            await Task.Run(async () =>
            {
                using (HttpClient client = new HttpClient())
                {
                    //Setting up link and headers
                    string url = $"https://api.github.com/search/issues?q={TitleID}+is:issue+is:open+repo:obhq/compatibility";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["gt"]}");
                    client.DefaultRequestHeaders.Add("User-Agent", "Oboteration");

                    //Get the json body and deserialize it
                    var responseMessage = await client.GetAsync(url);
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    Oboteration.Models.Github.Cusa.Root? issueRoot = JsonConvert.DeserializeObject<Oboteration.Models.Github.Cusa.Root>(responseBody);

                    //Attempt to get url
                    try
                    {
                        info[0] = issueRoot?.items[0].html_url;
                        
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        info[0] = null;
                        
                    }

                    //Attempt to get status
                    try
                    {
                        foreach (var item in issueRoot?.items[0].labels)
                        {
                            if (item.name.StartsWith("status"))
                            {
                                string[] status = item.name.Split('-');
                                info[1] = char.ToUpper(status[1].First()) + status[1].Substring(1).ToLower();
                                return;
                            }

                        }
                        info[1] = null;
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        info[1] = null;
                        return;
                    }

                }
            });
            return info;
        }
    }
}
