using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oboteration.Funcs
{
    public class ConsoleCommands
    {
        public Task Start()
        {
            //Console commands

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "stop")
                {
                    break;
                }

                switch (input)
                {
                    case "ping":
                        Console.WriteLine("pOWOng");
                        break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
