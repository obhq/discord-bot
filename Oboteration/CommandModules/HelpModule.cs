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
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task sendHelp()
        {
            if (Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["helpChannel"])
               && Context.Channel.Id != (Convert.ToUInt64(ConfigurationManager.AppSettings["botSpam"]))
               && Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["testChat"]))
            {
                return;
            }
            string example = Format.Code("!update cusa12345");
            EmbedBuilder eb = new EmbedBuilder();
            eb.Title = "Help";
            eb.Color = (Discord.Color?)System.Drawing.Color.FromArgb(92, 5, 4);

            eb.AddField("‎\n- !links", "Use this command to receive useful links you can visit.")
                .AddField("‎\n- !update \"Title ID\"", $"You can use this command along with a Title ID of choice to check if there are any updates available. For example {example}")
                .AddField("‎\nAdditionally you can upload a .txt or .log file that contains logs from Obliteration so you can get a little overview.", "‎ ");

            await Context.Message.ReplyAsync(embed: eb.Build());
        }
    }
}
