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
    public class ModModule : ApplicationCommandModule<ApplicationCommandContext> 
    {
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.KickUsers)]
        [SubSlashCommand("kick", "kick a user from the server")]
        public async Task<string> Power(
          [SubSlashCommandParameter(Name = "user", Description = "The user to kick")] GuildUser targetUser,
          [SubSlashCommandParameter(Name = "reason", Description = "The reason for the kick")] string kickReason) {
            string User(User? user = null)
                {
                    user ??= Context.User;
                    return user.Username;
                }
            try {
                await targetUser.KickAsync(new RestRequestProperties().WithAuditLogReason(kickReason));
                return User() + " has kicked " + targetUser.Username + " for: " + kickReason;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return "Failed to kick user";
            }
      }
}

}
