using Discord.Commands;
using Discord.WebSocket;
using Oboteration.Funcs;
using Oboteration.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oboteration.Handlers
{
    public class TextBasedCommands
    {

        private DiscordSocketClient _client;
        private CommandService _commandService;

        public TextBasedCommands(DiscordSocketClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;
        }
        

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleTextCommandAsync;
            await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        public async Task HandleTextCommandAsync(SocketMessage message)
        {
            var argPos = 0;

            var receivedMessage = message as SocketUserMessage;
            if (receivedMessage == null) return;

            //JUST CUZ STFU

            if (receivedMessage.Channel.Id == Convert.ToUInt64(ConfigurationManager.AppSettings["helpChannel"])
                && receivedMessage.Attachments.Count > 0 
                && Regex.IsMatch(receivedMessage.Attachments.ElementAt(0).Filename, @".*\.(log|txt)$") 

                || 

                receivedMessage.Channel.Id == Convert.ToUInt64(ConfigurationManager.AppSettings["botSpam"])
                && receivedMessage.Attachments.Count > 0
                && Regex.IsMatch(receivedMessage.Attachments.ElementAt(0).Filename, @".*\.(log|txt)$")

                ||

                receivedMessage.Channel.Id == Convert.ToUInt64(ConfigurationManager.AppSettings["testChat"])
                && receivedMessage.Attachments.Count > 0
                && Regex.IsMatch(receivedMessage.Attachments.ElementAt(0).Filename, @".*\.(log|txt)$"))
            {
                SendLogInfo logInfo = new SendLogInfo(client: _client,message: receivedMessage);
                await logInfo.Send();
            }

            if (receivedMessage.Author.IsBot || !(receivedMessage.HasCharPrefix(Convert.ToChar(ConfigurationManager.AppSettings["prefix"]), ref argPos) || receivedMessage.HasMentionPrefix(_client.CurrentUser,ref argPos))) return;
            

            var context = new SocketCommandContext(_client,receivedMessage);

            await _commandService.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}
