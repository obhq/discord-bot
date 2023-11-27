using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Oboteration.Funcs;
using Oboteration.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Oboteration.CommandModules
{
    public class UpdateModule : ModuleBase<SocketCommandContext>
    {
        [Command("update")]
        public async Task Update(string TitleID)
        {
           if (Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["helpChannel"])
               && Context.Channel.Id != (Convert.ToUInt64(ConfigurationManager.AppSettings["botSpam"]))
               && Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["testChat"]))
           {
               return;
           }

            //Check if format of input fits that of a TitleID
            string regex = @"^[a-zA-Z]{4}\d{5}$";
            if (!Regex.IsMatch(TitleID, regex))
            {
                await Context.Message.ReplyAsync("Invalid TitleID format. (Example ABCD12345)");
                return;
            }

            TitleID = TitleID.ToUpper();

            //Get update information
            GetUpdateInfo getUpdateInfo = new GetUpdateInfo();
            Oboteration.Models.Update update = await getUpdateInfo.getInfo(TitleID);

            //If update doesn't exist reply with message accordingly
            if (!update.updateExist)
            {
                await Context.Message.ReplyAsync($"No updates found for {TitleID}");
                return;
            }

            //Format the links
            string downloadLink = Format.Url($"{update.Title}_{update.version}.pkg", update.pieces[0].url);
            string jsonLink = Format.Url($"{TitleID}.json", update.jsonLink);

            EmbedBuilder eb = new EmbedBuilder
            {
                Title = update.Title + " - " + TitleID,
                Url = update.xmlLink,
                Color = (Discord.Color?)System.Drawing.Color.FromArgb(92, 5, 4)
            };

            //Adding embed fields
            eb.AddField($"Update version:", $"{update.version}");
            string size = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:0.00}", update.size);
            if (update.isGB)
            {
               
              eb.AddField($"Update size: ", $"{size}GB");

            }
            else
            {
                
                eb.AddField($"Update size: ", $"{size}MB");
            }

            if (update.pieces.Count > 1)
            {
                for (int i = 0; i < update.pieces.Count; i++)
                {
                    if (i > 0)
                    {
                        eb.AddField($"‎‎ ", $"{Format.Url($"{update.Title}_{update.version}.pkg [Part {i + 1}]", update.pieces[i].url)}");

                    }
                    else
                    {
                        eb.AddField($"Download Links: ", $"{Format.Url($"{update.Title}_{update.version}.pkg [Part {i + 1}]", update.pieces[i].url)}");

                    }

                }
                eb.AddField($"Json Link: ", $"{jsonLink}");
            }
            else
            {
                eb.AddField($"Download Link: ", $"{downloadLink}")
                .AddField($"Json Link: ", $"{jsonLink}");
            }
            



            await Context.Message.ReplyAsync(embed: eb.Build());

        }
    }
}
