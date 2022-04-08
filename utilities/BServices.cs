using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using Discord.Interactions;
using Discord;

namespace discordbottemplate.utilities{
    public class BServices{
        private static readonly BServices _instance = new();
        public static BServices Instance
        {
            get { return _instance; }
        }
        /// <summary>
        /// Return a socket command context using a socketmessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual SocketCommandContext ReturnContext(SocketMessage message){
            return new SocketCommandContext(Bot.InstanceClient, (SocketUserMessage)message);
        }
        /// <summary>
        /// Get author id
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual ulong GetAuthorID(SocketCommandContext context = null, SocketSlashCommand command = null) {
            CommandOrContextNullCheck(context, command);
            if (context == null) return command.User.Id;
            else return context.Message.Author.Id;
        }
        /// <summary>
        /// Use this to thrown an <see cref="ArgumentException"/> if the context and the command are null
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        public virtual void CommandOrContextNullCheck(SocketCommandContext context = null, SocketSlashCommand command = null)
        {
            if (context == null && command == null) throw new ArgumentException("Context and command are empty, fill them please!");
        }
        /// <summary>
        /// Return the user id for the command or slash command
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual ulong GetDiscordID(SocketCommandContext context = null, SocketSlashCommand command = null)
        {
            CommandOrContextNullCheck(context, command);
            if (context == null) return command.User.Id;
            else return context.User.Id;
        }
        /// <summary>
        /// Use this function to answer to context or commands buttons
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stickers"></param>
        /// <param name="embed"></param>
        /// <param name="embeds"></param>
        /// <param name="ephemeral"></param>
        /// <param name="component"></param>
        /// <param name="disregardArgumentExceptions"></param>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual async Task<RestUserMessage> MakeResponseAsync(string text = null, Sticker[] stickers = null, Embed embed = null, Embed[] embeds = null, bool ephemeral = false, MessageComponent component = null, bool disregardArgumentExceptions = false, SocketCommandContext context = null, SocketSlashCommand command = null)
        {
            CommandOrContextNullCheck(context, command);
            return await context.Channel.SendMessageAsync(text ?? " ", embed: embed, components: component);
        }
        /// <summary>
        /// Task to create embed pages
        /// </summary>
        /// <param name="client"></param>
        /// <param name="embedBuilders"></param>
        /// <param name="extraButtons"></param>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual async Task CreateEmbedPages(DiscordSocketClient client, List<EmbedBuilder> embedBuilders, ButtonBuilder[] extraButtons = null, SocketCommandContext context = null, SocketSlashCommand command = null){
            CommandOrContextNullCheck(context, command);

            if(!embedBuilders.Any()){
               await MakeResponseAsync("Check your parameters again, it was not possible to build the message", context:context);
            }

            int currentPage = 0;
            embedBuilders[0] = embedBuilders[0].WithFooter($"Page: {currentPage+1}/{embedBuilders.Count}");

            ComponentBuilder componentBuilder = GetComponentBuilder(embedBuilders.Count, currentPage + 1);

            var currentMessage = await MakeResponseAsync(embed:embedBuilders[0].Build(), component: componentBuilder.Build(), context: context, command: command);
            client.InteractionCreated += async (socketInteraction) =>{
                SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                if (interaction.Data.Type != ComponentType.Button) return;
                if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == GetAuthorID(context, command))
                {
                    currentPage = await GetCurrentPage(interaction, currentPage, embedBuilders, currentMessage);
                }
            };
        }
        /// <summary>
        /// Component builder for the buttons
        /// </summary>
        /// <param name="maxPage"></param>
        /// <param name="currentPage"></param>
        /// <param name="extraButtons"></param>
        /// <returns></returns>
        private ComponentBuilder GetComponentBuilder(int maxPage, int currentPage, ButtonBuilder[] extraButtons = null)
        {
            int buttonCount = 2;

            ComponentBuilder componentBuilder = new();
            ButtonBuilder buttonBuilder;

            buttonBuilder = new ButtonBuilder()
                .WithCustomId("page_first_embed")
                .WithLabel("First page" ?? "«")
                .WithStyle(ButtonStyle.Primary);

            componentBuilder.WithButton(buttonBuilder);
            buttonCount++;

            if (currentPage != 1)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("page_back_button")
                    .WithLabel("Previous" ?? "‹")
                    .WithStyle(ButtonStyle.Primary);

                componentBuilder.WithButton(buttonBuilder);
            }
            else
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("page_back_button")
                    .WithLabel("Previous" ?? "‹")
                    .WithStyle(ButtonStyle.Primary);

                buttonBuilder.IsDisabled = true;

                componentBuilder.WithButton(buttonBuilder);
            }

            if (currentPage < maxPage)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("page_forward_button")
                    .WithLabel("Next" ?? "›")
                    .WithStyle(ButtonStyle.Primary);

                componentBuilder.WithButton(buttonBuilder);
            }
            else
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("page_forward_button")
                    .WithLabel("Next" ?? "›")
                    .WithStyle(ButtonStyle.Primary);

                buttonBuilder.IsDisabled = true;

                componentBuilder.WithButton(buttonBuilder);
            }

            return componentBuilder;
        }
        /// <summary>
        /// Use this function to return the current page
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="currentPage"></param>
        /// <param name="embedBuilders"></param>
        /// <returns></returns>
        private async Task<int> GetCurrentPage(SocketMessageComponent interaction, int currentPage, List<EmbedBuilder> embedBuilders, RestUserMessage msg) {
            switch (interaction.Data.CustomId) {
                case "page_back_button":
                    if (currentPage - 1 < 0) currentPage = 0;
                    else {
                        currentPage -= 1;
                        await msg.ModifyAsync(msg =>{
                            embedBuilders[currentPage].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");
                            msg.Embed = embedBuilders[currentPage].Build();

                            ComponentBuilder componentBuilder = GetComponentBuilder(embedBuilders.Count, currentPage + 1);

                            msg.Components = componentBuilder.Build();
                        });
                        await interaction.DeferAsync();
                    }
                    break;
                case "page_forward_button":
                    if (currentPage + 1 == embedBuilders.Count) currentPage = 0;
                    else {
                        currentPage += 1;
                        await msg.ModifyAsync(msg =>{
                            embedBuilders[currentPage].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");
                            msg.Embed = embedBuilders[currentPage].Build();

                            ComponentBuilder componentBuilder = GetComponentBuilder(embedBuilders.Count, currentPage + 1);

                            msg.Components = componentBuilder.Build();
                        });
                        await interaction.DeferAsync();
                    }
                    break;
                case "page_first_embed":
                    currentPage = 0;
                            await msg.ModifyAsync(msg =>{
                            embedBuilders[currentPage].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");
                            msg.Embed = embedBuilders[currentPage].Build();

                            ComponentBuilder componentBuilder = GetComponentBuilder(embedBuilders.Count, currentPage + 1);

                            msg.Components = componentBuilder.Build();
                        });
                        await interaction.DeferAsync();
                    break;
            }
            return currentPage;
        }
    }
}