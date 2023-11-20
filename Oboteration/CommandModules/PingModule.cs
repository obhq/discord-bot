using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oboteration.CommandModules
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            if (Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["helpChannel"])
               && Context.Channel.Id != (Convert.ToUInt64(ConfigurationManager.AppSettings["botSpam"]))
               && Context.Channel.Id != Convert.ToUInt64(ConfigurationManager.AppSettings["testChat"]))
            {
                return;
            }
            await ReplyAsync("pOWOng");
        }
    }
}
