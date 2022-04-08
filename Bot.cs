using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

namespace discordbottemplate
{
    public class Bot
    {   
        public static DiscordSocketClient InstanceClient;
        public DiscordSocketClient Client{get;private set;}
        private string commandsPrefix;
        public async Task RunAsync(){

            Client = new DiscordSocketClient();

            //Creating our usermanager

            Client.Log += Log;


            var json = string.Empty;

            using(var fs = File.OpenRead("config.json")){
                using(var sr = new StreamReader(fs)){
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            //Setting the commands
            var c =  new Commands();
            Client.MessageReceived += c.CommandHandler;
            
            //Setting it to debug mode 
            if(configJson.DebugMode){
                Console.WriteLine("Starting the debug stuff");
            }

            //Settings our prefix to a private variable
            commandsPrefix = configJson.CommandPrefix;

            //Initializating the bot with the token and starting it.
            await Client.LoginAsync(TokenType.Bot, configJson.Token);
            await Client.StartAsync();

            InstanceClient = Client;

            await System.Threading.Tasks.Task.Delay(-1);
        }
        private Task Log(LogMessage msg){
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}