using Discord.WebSocket;
using Discord;
using Oboteration.Funcs;
using Discord.Commands;
using Oboteration.Handlers;
using System.Configuration;

namespace Oboteration
{
    public class Program
    {
        private DiscordSocketClient? _client;
        private CommandService? _commandService;
        public static void Main(string[] args) => new Program().MainAsync();


        public async Task MainAsync()
        {
            string token = ConfigurationManager.AppSettings["dt"];

            DiscordSocketConfig config = new DiscordSocketConfig {
                GatewayIntents = GatewayIntents.All
            };

            _client = new DiscordSocketClient(config);
            _commandService = new CommandService();

            //Logging

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Start Logging \n");
            Logging logging = new Logging();
            //_client.Log += logging.Log;

            //Command Handlers

            await logging.BotLog("Installing Modules");
            TextBasedCommands textCommandHandler = new TextBasedCommands(client: _client,commandService: _commandService);
            await textCommandHandler.InstallCommandsAsync();


            //Bot client Login
            await logging.BotLog("Logging In");
            _client.LoggedIn += () =>
            {
                logging.BotLog("Logged In");
                return Task.CompletedTask;
            };
            await _client.LoginAsync(TokenType.Bot,token);
            
            ConsoleCommands consoleCommands = new ConsoleCommands();
            await logging.BotLog("Starting Bot");
            await _client.StartAsync();
            _client.Ready += () =>
            {
                logging.BotLog("Bot is Ready!");
                Console.ForegroundColor = ConsoleColor.Gray;
                return Task.CompletedTask;
            };

            await consoleCommands.Start();
           


        }
    }
}