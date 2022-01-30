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


namespace curt
{
    internal class Curt
    {
        private readonly DiscordSocketClient _client;
        private CommandHandler _commandHandler;
        private ulong[] COOL_PEOPLE = { 298233151162155018, 138679605237252096, 535134130980257802 };

        static void Main(string[] args) 
            => new Curt()
            .MainAsync()
            .GetAwaiter()
            .GetResult();

        public Curt()
        {
            _client = new DiscordSocketClient();

            CommandService defaultCommandService = new CommandService();
            _commandHandler = new CommandHandler(_client, defaultCommandService);

            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;
            //_client.MessageReceived += MessageReceivedAsync;
            _client.InteractionCreated += InteractionCreatedAsync;
        
        }

        public async Task MainAsync()
        {
            // Read bot config
            try
            {
                var config = Toml.ToModel(System.IO.File.ReadAllText("config.toml"));

                await _client.LoginAsync(TokenType.Bot, (string)config["token"]);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("Config file not found");

                System.Environment.Exit(1);
            }

            Console.WriteLine(FiggleFonts.Block.Render("Curt"));

            await _commandHandler.InstallCommandsAsync();

            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;


            if (message.Content == "!ping")
            {
                // Create a new componentbuilder, in which dropdowns & buttons can be created.
                var cb = new ComponentBuilder()
                    .WithButton("new button", "replybtn_testButton", ButtonStyle.Primary);

                // Send a message with content 'pong', including a button.
                // This button needs to be build by calling .Build() before being passed into the call.
                await message.Channel.SendMessageAsync("pong!", components: cb.Build());
            }

            
        }

        private async Task InteractionCreatedAsync(SocketInteraction interaction)
        {
            // safety-casting is the best way to prevent something being cast from being null.
            // If this check does not pass, it could not be cast to said type.
            if (interaction is SocketMessageComponent component)
            {
                if (!Array.Exists(COOL_PEOPLE, x => x == interaction.User.Id))  
                {
                    return;
                }

                // Check for the ID created in the button mentioned above.
                Console.WriteLine($"Pressed {component.Data.CustomId} :3"); // Does concaternation work like this in C# ?  -chica
                switch(component.Data.CustomId)
                {
                    case "replybtn_testButton":
                        await interaction.RespondAsync("by clicking on this button you agree that you're a bottom");
                        break;
                    case "replybtn_owoButton":
                        await interaction.RespondAsync("uhhh HARDER!!");
                        break;
                    default:
                        Console.WriteLine("An ID has been received that has no handler!");
                        break;
                }

            }
        }
    }
}
