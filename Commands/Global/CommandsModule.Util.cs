using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace JimmyRewrite.Commands.Global;

public partial class CommandsModule
{
    [SlashCommand("avatar", "Get someone's avatar!")]
    public async Task Avatar(
        [SlashCommandParameter(Name = "user", Description = "The person that you want an avatar of")] User? user = null
        )
    {
        var userToGet = user ?? Context.User;
        
        var embed = new EmbedProperties()
            .WithTitle($"{userToGet.Username}'s Avatar")
            .WithImage(new EmbedImageProperties($"{userToGet.GetAvatarUrl() ?? userToGet.DefaultAvatarUrl}"));
        
        var messageProperties = new InteractionMessageProperties()
            .AddEmbeds(embed);

        await Context.Interaction.SendResponseAsync(
            InteractionCallback.Message(messageProperties)
        );
    }
}