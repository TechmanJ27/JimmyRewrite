using System;
using System.Text;
using System.Data;

using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Rest;

namespace JimmyRewrite.Commands;

public partial class CommandsModule
{
    [SlashCommand("mod", "Moderate the server")]
    public class ModModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.BanUsers)]
        [RequireBotPermissions<ApplicationCommandContext>(Permissions.BanUsers)]
        [SubSlashCommand("ban", "Ban a user")]
        public async Task Ban(User user, string rule, string duration, string? note = null, string? modNote = null)
        {
            
        }
    }
}
