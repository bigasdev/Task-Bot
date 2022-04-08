using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.WebSocket;
using Discord;
using System.Linq;
using Discord.Rest;
using Discord.Commands;
using Newtonsoft.Json;
using System.Net.Http;
using json = System.Text.Json;
using System.Threading;

namespace discordbottemplate.utilities{
    public static class EmbedUtils{
        /// <summary>
        /// Use this to generate and send a basic embed
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static EmbedBuilder CreateBasicEmbed(SocketMessage msg, Color color,  string message, string title = ""){
            var eb = new EmbedBuilder{
                Title = title,
                Description = message,
                Color = color
            };
            msg.Channel.SendMessageAsync("", embed: eb.Build());
            return eb;
        }
        /// <summary>
        /// Use this to generate and store a basic embed
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static EmbedBuilder CreateBasicEmbed(Color color,  string message, string title = ""){
            var eb = new EmbedBuilder{
                Title = title,
                Description = message,
                Color = color
            };
            return eb;
        }
    }
}