using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oboteration.CommandModules
{
    public class LinksModule : ModuleBase<SocketCommandContext>
    {
        [Command("links")]
        public async Task sendLinks()
        {
            string githubLink = "https://github.com/obhq/obliteration";
            string compatibilityLink = "https://github.com/obhq/compatibility";
            string website = "https://obliteration.net/";
            string logoLink = "https://github.com/obhq/obliteration/blob/main/logo.png?raw=true";

            if (Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["helpChannel"])
               && Context.Channel.Id != (Convert.ToUInt64(ConfigurationManager.AppSettings["botSpam"]))
               && Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["testChat"]))
            {
                return;
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = (Discord.Color?)System.Drawing.Color.FromArgb(92, 5, 4);
            eb.Title = "Related Links";
            eb.ImageUrl = logoLink;

            eb.AddField("Github Repo", $"{githubLink}")
                .AddField("Compatibility Repo", $"{compatibilityLink}")
                .AddField("Official Website", $"{website}");

            await Context.Message.ReplyAsync(embed: eb.Build());
        }
    }
}
