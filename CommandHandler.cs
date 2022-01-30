using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Tomlyn;
using Figgle;
using System.Reflection;

namespace curt
{

    public class CommandHandler
    {   
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private ulong[] COOL_PEOPLE = { 298233151162155018, 138679605237252096, 535134130980257802 };

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }
        
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            if (!Array.Exists(COOL_PEOPLE, x => x == message.Author.Id))
            {
                return;
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: null);

            
        }
    }


    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        // Chica is gay:
        [Command("chica")]
        [Summary("Calls chica gay")]
        public Task chicaAsync()
            => ReplyAsync("is very homosexual");


        // Simple ping test:
        [Command("ping")]
        [Summary("ping with a button")]
        public Task pingAsync() 
        {
            // Create a new componentbuilder, in which dropdowns & buttons can be created.
            var cb = new ComponentBuilder()
                    .WithButton("new button", "replybtn_testButton", ButtonStyle.Primary);

            // Send a message with content 'pong', including a button.
            // This button needs to be build by calling .Build() before being passed into the call.
            return ReplyAsync("pong!", components: cb.Build());
        }


        // Chica Experimenting:
        [Command("owo")]
        [Summary("chica experiments")]
        public Task owoAsync() // Different task name per button thingy
        {
            // Create a new componentbuilder, in which dropdowns & buttons can be created.
            var cb = new ComponentBuilder()
                    .WithButton("Push ME!", "replybtn_owoButton", ButtonStyle.Primary);

            // Send a message with content 'pong', including a button.
            // This button needs to be build by calling .Build() before being passed into the call.
            return ReplyAsync("Push the button!", components: cb.Build());
        }


        // Collect Messages:
        [Command("collect")]
        [Summary("Collect the 5 past messages and paste them in chat")]
        public Task collectAsync()
        {
            var messages = Context.Channel.GetMessagesAsync(
                Context.Message,
                Direction.Before,
                5,
                CacheMode.AllowDownload).Flatten();

            Console.WriteLine(messages);

            return Task.CompletedTask;
        }


        // Image test:
        [Command("loveduck")]
        [Summary("Tries to auto embed image")]
        public Task loveduckAsync()
            => ReplyAsync("Loveduck\nhttps://cdn.discordapp.com/emojis/385464688819044354.png");
        
    }
}