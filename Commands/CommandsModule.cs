// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Reflection;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace JimmyRewrite.Commands;

public partial class CommandsModule : ApplicationCommandModule<ApplicationCommandContext>
{
    
    private static Task<InteractionCallbackResponse?> SendGuildOnlyError(ApplicationCommandContext context)
    {
        var messageProperties =
            new InteractionMessageProperties().WithContent(":x: This command can only be used in a guild!");
            
        return context.Interaction.SendResponseAsync(
            InteractionCallback.Message(messageProperties)
        );
    }
    
    [SlashCommand("ping", "Ping always comes with a pong!")]
    public string Ping() => $"Pong!\n" +
                            $"-# {Math.Round(Context.Client.Latency.TotalMilliseconds)} ms";
    
    [SlashCommand("about", "Who is this fella?")]
    public async Task About()
    {

        var embed = new EmbedProperties()
            .WithFields([
                new EmbedFieldProperties()
                    .WithName("About")
                    .WithValue("This is a rewrite of **Jimmy**, a Discord bot written in C# using [NetCord](https://netcord.dev/), " +
                               "originally written in TypeScript by [TheMonHub](https:///themonhub.net).\n" +
                               "**Jimmy** (*shortly Jim*) is a multipurpose Discord bot with its main focus being moderation." +
                               "It provides a *variety of features* for moderation, intending to be a better way to manage your server."),
                new EmbedFieldProperties()
                    .WithName("Version")
                    .WithValue($"`{Assembly.GetEntryAssembly()!.GetName().Version}`"),
                new EmbedFieldProperties()
                    .WithName("Source Code")
                    .WithValue("https://github.com/TheMonHub/JimmyRewrite")
            ]);
        
        var messageProperties = new InteractionMessageProperties()
            .AddEmbeds(embed);

        await Context.Interaction.SendResponseAsync(
            InteractionCallback.Message(messageProperties)
        );
    }
}