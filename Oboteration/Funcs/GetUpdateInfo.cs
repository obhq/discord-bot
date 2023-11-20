using Newtonsoft.Json;
using Oboteration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Oboteration.Funcs
{
    public class GetUpdateInfo
    {
        
        public async Task<Update> getInfo(string TitleID)
        {
            //Variables UwU
            Oboteration.Models.Update update = new Oboteration.Models.Update();
            byte[] key = StringToByteArray("AD62E37F905E06BC19593142281C112CEC0E7EC3E97EFDCAEFCDBAAFA6378D84");
            string url;

            //Generate XML link
            using (var hmacsha256 = new HMACSHA256(key))
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes($"np_{TitleID}"));
                string convertedHash = BitConverter.ToString(hash).Replace("-", "").ToLower();

                url = $"https://gs-sec.ww.np.dl.playstation.net/plo/np/{TitleID}/{convertedHash}/{TitleID}-ver.xml";
                update.xmlLink = url;
            }


            update = await Task.Run(async () =>
            {
                //Sony are retarded <3 :3
                //Certification handler, because sony hates us
                var sonycerthandler = new HttpClientHandler();
                sonycerthandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                
                Oboteration.Models.Update update = new Oboteration.Models.Update();

                string body;
                using (HttpClient client = new HttpClient(sonycerthandler))
                {
                    //Fetch XML body
                    var responseMessage = await client.GetAsync(url);
                    body = await responseMessage.Content.ReadAsStringAsync();

                    //Load XML body
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(body);


                    //Check if Update exists
                    XmlNode? nokey = xmlDoc.SelectSingleNode("/Error/Code");
                    if (nokey?.InnerText == "NoSuchKey")
                    {
                        update.updateExist = false;
                        return update;
                    }

                    //If update exists  retrieve all needed information for the update
                    XmlNode? jsonNode = xmlDoc.SelectSingleNode("/titlepatch/tag/package/@manifest_url");
                    XmlNode? titleNode = xmlDoc.SelectSingleNode("/titlepatch/tag/package/paramsfo/title");
                    XmlNode? sizeNode = xmlDoc.SelectSingleNode("/titlepatch/tag/package/@size");
                    XmlNode? versionNode = xmlDoc.SelectSingleNode("/titlepatch/tag/package/@version");
                    update.jsonLink = jsonNode.InnerText;

                    //Check and convert size to MB/GB
                    if (Convert.ToDouble(sizeNode?.InnerText) >= 1073741824)
                    {
                        update.size = ((Convert.ToDouble(sizeNode?.InnerText) / 1048576) / 1024);
                        update.isGB = true;
                    }
                    else
                    {
                        update.size = (Convert.ToDouble(sizeNode?.InnerText) / 1048576);
                    }

                    update.Title = titleNode?.InnerText;
                    update.version = versionNode?.InnerText;
                    update.updateExist = true;


                    //If Json pattern is different
                    if (titleNode?.InnerText == null)
                    {
                        jsonNode = xmlDoc.SelectSingleNode("/titlepatch/selective_tag/package/@manifest_url");
                        titleNode = xmlDoc.SelectSingleNode("/titlepatch/selective_tag/package/paramsfo/@title");
                        sizeNode = xmlDoc.SelectSingleNode("/titlepatch/selective_tag/package/@size");
                        versionNode = xmlDoc.SelectSingleNode("/titlepatch/selective_tag/package/@version");
                        update.jsonLink = jsonNode?.InnerText;

                        //Check and convert size to MB/GB
                        if (Convert.ToInt32(sizeNode?.InnerText) >= 1073741824)
                        {
                            sizeNode.InnerText = "1073741824";
                            update.size = ((Convert.ToInt32(sizeNode?.InnerText) / 1048576) / 1024);
                            update.isGB = true;
                        }
                        else
                        {
                            update.size = (Convert.ToInt32(sizeNode?.InnerText) / 1048576);
                        }

                        update.Title = titleNode?.InnerText;
                        update.version = versionNode?.InnerText;
                    }

                    //Get Pkg Link
                    client.DefaultRequestHeaders.Add("User-Agent", "Oboteration");
                    var jsonResponseMessage = await client.GetAsync(update.jsonLink);
                    string responseBody = await jsonResponseMessage.Content.ReadAsStringAsync();
                    Oboteration.Models.Sony.Root? issueRoot = JsonConvert.DeserializeObject<Oboteration.Models.Sony.Root>(responseBody);

                    update.pkgLink = issueRoot?.pieces[0].url;
                }


                return update;
            });
            return update;
        }


        static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] byteArray = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return byteArray;
        }
    }
}
