using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Oboteration.Funcs
{
    public class Logging
    {
        public Task Log(LogMessage message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Discord Log]: " + message);
            return Task.CompletedTask;

        }

        public Task BotLog(string args)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[Bot Log]: {args}");
            return Task.CompletedTask;
        }
    }
}
