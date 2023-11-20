using Discord;
using Discord.WebSocket;
using Oboteration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Oboteration.Funcs
{
    public class SendLogInfo
    {
        private DiscordSocketClient _client;
        private SocketUserMessage _message;

        public SendLogInfo(DiscordSocketClient client, SocketUserMessage message)
        {
            _client = client;
            _message = message;
        }

        public async Task Send()
        {
            //Create Instances of needed classes
            ReadLogFile readLogFile = new ReadLogFile();
            Github githubFuncs = new Github();

            //Retrieve file content of uploaded log
            string fileContent;
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(_message.Attachments.ElementAt(0).Url);
                var text = response.Content.ReadAsStringAsync().Result;
                fileContent = text;
            }

            //Check if the log file is valid
            if (await readLogFile.checkValid(fileContent) == false){ return; }
            
            //Get the information from log and fetch github info
            LogFile logInfo = await readLogFile.getLogInfo(fileContent);
            string?[] githubInfo = await githubFuncs.getGithubInfo(logInfo.AppID);


            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = (Discord.Color?)System.Drawing.Color.FromArgb(92, 5, 4);


            //Check to see if there were no W/E/P found and if some information is null or empty
            if (logInfo.Wep == null || logInfo.Wep == ""){ logInfo.Wep = "No warnings, errors, or panics found.";}

            if (logInfo.AppID == null || logInfo.AppID == "") { logInfo.AppID = null; eb.Title = "Unknown Game"; }
            else { eb.Title = logInfo.AppTitle + " - " + logInfo.AppID; }

            if (githubInfo[1] == null || logInfo.AppID == null) { eb.AddField("Game Status", "Unknown"); }
            else { eb.AddField("Game Status", githubInfo[1]); }

            if (logInfo.CpuInfo == null) { logInfo.CpuInfo = "Unknown"; }
            if (logInfo.RamInfo == null) { logInfo.RamInfo = "Unknown"; }
            if (logInfo.OS == null) { logInfo.OS = "Unknown"; }


            string formatWep = Discord.Format.Code(logInfo.Wep);
            
            //Adding fields to embed after everything has been checked

            eb.AddField("System OS", logInfo.OS)
                .AddField("CPU", logInfo.CpuInfo)
                .AddField("Available RAM", logInfo.RamInfo);
                
            // Setting the W/E/P title accordingly to the content of the W/E/P
            switch(logInfo.Wep)
            {
                case string x when x.StartsWith("++++++++++++++++++ E"):
                    eb.AddField("Last Error", formatWep);
                    break;
                case string x when x.StartsWith("++++++++++++++++++ W"):
                    eb.AddField("Last Warning", formatWep);
                    break;
                case string x when x.StartsWith("++++++++++++++++++ P"):
                    eb.AddField("Kernel Panic", formatWep);
                    break;
                default:
                    eb.AddField("No issues in log", formatWep);
                    break;
            }

            //Decide if url is valid and assign it
            if (githubInfo[0] != null && githubInfo[0].StartsWith("https://")  && logInfo.AppID != null)
            {
                eb.WithUrl(githubInfo[0]);
            }


            await _message.ReplyAsync(embed: eb.Build());
        }
    }
}
