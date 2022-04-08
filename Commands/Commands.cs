using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Discord.WebSocket;
using Discord;
using Discord.Net;
using Newtonsoft.Json;
using discordbottemplate.utilities;

namespace discordbottemplate{
    public class Commands{
        //The command handler
        public async Task CommandHandler(SocketMessage message){
            //local variables for the command string and lenght
            string command = "";
            string subCommand = "";
            string param1 = "";
            var param2 = "";
            int lenghtCommand = -1;
            //Check for the prefix and if its a bot that called
            Console.WriteLine("Trying to execute a command");
            var s = ".";
            if(!message.Content.StartsWith(s[0]))
                await Task.CompletedTask;

            if(message.Author.IsBot)
                await Task.CompletedTask;

            if(message.Content.Contains(' ')){
                lenghtCommand = message.Content.IndexOf(' ');
                Console.WriteLine("Size: "+message.Content.Length+" Sub: "+lenghtCommand);
                try
                {
                    subCommand = message.Content.Substring(lenghtCommand + 1);
                    int nxtSpace = message.Content.IndexOf(' ', lenghtCommand+2);
                    Console.WriteLine(nxtSpace);
                    if(nxtSpace != -1){
                        subCommand = subCommand.Substring(0, subCommand.IndexOf(' ', 1));
                        param1 = message.Content.Substring(nxtSpace + 1);
                        nxtSpace = message.Content.IndexOf(' ', nxtSpace + 2);
                        Console.WriteLine(nxtSpace);
                        if(nxtSpace != -1){
                            param1 = param1.Substring(0, param1.IndexOf(' ', 1));
                            param2 = message.Content.Substring(nxtSpace + 1);
                        }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
                lenghtCommand = message.Content.Length;

            command = message.Content.Substring(1, lenghtCommand - 1);
            Console.WriteLine("The command is : "+command+" Done by: "+message.Author.ToString());

            switch(command){
                case "help":
                    EmbedUtils.CreateBasicEmbed(message, Color.Red, "yup", "no help for you");
                break;
                case "multipage":
                    await CreateMultiPageExample(5, message);
                break;
            }
            await Task.CompletedTask;
        }
        public async Task CreateMultiPageExample(int pages, SocketMessage message){
            List<EmbedBuilder> embeds = new List<EmbedBuilder>();
            for (int i = 0; i < pages; i++)
            {
                var eb = EmbedUtils.CreateBasicEmbed(Color.Green, $"Its your {pages}Page!", "Embed multi page example");
                embeds.Add(eb);
            }
            await BServices.Instance.CreateEmbedPages(Bot.InstanceClient, embeds, context: BServices.Instance.ReturnContext(message));
        }
    }
}