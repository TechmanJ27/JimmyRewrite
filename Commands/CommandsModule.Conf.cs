// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace JimmyRewrite.Commands;

public partial class CommandsModule
{
    [SlashCommand("config", "Configure the bot",
        DefaultGuildPermissions = Permissions.Administrator,
        Contexts = [InteractionContextType.Guild])]
    public class ConfigModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_log_message", "Set the channel to log message deletion and edits")]
        public async Task SetLogMessage(TextGuildChannel channel)
        {
            var guildId = channel.GuildId;
            
            var old = Database.Database.GetLogMessageChannel(guildId);

            Database.Database.SetLogMessageChannel(channel.Id, guildId);
            await ModerationHandler.LogConfigChange(guildId, Context.User, "Log Message Channel", old != null ? $"<#{old}>" : null, $"<#{channel.Id}>", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent($":white_check_mark: Set the log message channel to <#{channel.Id}>");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }
        
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_log_watch", "Set the channel to log media, message deletion, and edits sent by watched member")]
        public async Task SetLogWatch(TextGuildChannel channel)
        {
            var guildId = channel.GuildId;
            
            var old = Database.Database.GetLogWatchChannel(guildId);

            Database.Database.SetLogWatchChannel(channel.Id, guildId);
            
            await ModerationHandler.LogConfigChange(guildId, Context.User, "Log Watch Channel", old != null ? $"<#{old}>" : null, $"<#{channel.Id}>", Context.Client.Rest);


            var messageProperties =
                new InteractionMessageProperties().WithContent($":white_check_mark: Set the log watch channel to <#{channel.Id}>");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }
        
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_log_welcome", "Set the channel to log join/leave activity")]
        public async Task SetLogWelcome(TextGuildChannel channel)
        {
            var guildId = channel.GuildId;
            var old = Database.Database.GetLogWelcomeChannel(guildId);
            
            Database.Database.SetLogWelcomeChannel(channel.Id, guildId);
            
            await ModerationHandler.LogConfigChange(guildId, Context.User, "Log Welcome Channel", old != null ? $"<#{old}>" : null, $"<#{channel.Id}>", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent($":white_check_mark: Set the log welcome channel to <#{channel.Id}>");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }
        
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_log_mod", "Set the channel to log moderation activity")]
        public async Task SetLogMod(TextGuildChannel channel)
        {
            var guildId = channel.GuildId;
            var old = Database.Database.GetLogModChannel(guildId);
            
            Database.Database.SetLogModChannel(channel.Id, guildId);
            
            await ModerationHandler.LogConfigChange(guildId, Context.User, "Log Mod Channel", old != null ? $"<#{old}>" : null, $"<#{channel.Id}>", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent($":white_check_mark: Set the log mod channel to <#{channel.Id}>");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }
        
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("get_appeal_link", "Set the appeal link that gets send to the offender.")]
        public async Task GetAppealLink(string text)
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetAppealLink(guildId.Value);
            
            Database.Database.SetAppealLink(text, guildId.Value);

            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Appeal Link", old, text, Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Set appeal link!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_force_note", "Require a note when taking moderation actions")]
        public async Task SetForceNote(bool value)
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetForceNote(guildId.Value);
            
            Database.Database.SetForceNote(value, guildId.Value);

            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Force Note", old.ToString(), value.ToString(), Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Set force note!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_allow_no_rule", "Allow no rule when taking moderation actions")]
        public async Task SetAllowNoRule(bool value)
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetAllowNoRule(guildId.Value);
            
            Database.Database.SetAllowNoRule(value, guildId.Value);

            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Allow No Rule", old.ToString(), value.ToString(), Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Set allow no rule!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("set_mod_action_confirm", "Require confirmation when taking moderation actions")]
        public async Task SetModActionConfirm(bool value)
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetModActionConfirm(guildId.Value);
            
            Database.Database.SetModActionConfirm(value, guildId.Value);

            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Mod Action Confirm", old.ToString(), value.ToString(), Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Set mod action confirm!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("reset_log_message", "Reset the channel to log message deletion and edits")]
        public async Task ResetLogMessage()
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetLogMessageChannel(guildId.Value);

            Database.Database.ResetLogMessageChannel(guildId.Value);
            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Log Message Channel", old != null ? $"<#{old}>" : null, "*None*", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Reset the log message channel!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("reset_log_watch", "Reset the channel to log media, message deletion, and edits sent by watched member")]
        public async Task ResetLogWatch()
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetLogWatchChannel(guildId.Value);

            Database.Database.ResetLogWatchChannel(guildId.Value);
            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Log Watch Channel", old != null ? $"<#{old}>" : null, "*None*", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Reset the log watch channel!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }
        
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("reset_log_welcome", "Reset the channel to log join/leave activity")]
        public async Task ResetLogWelcome()
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetLogWelcomeChannel(guildId.Value);
            
            Database.Database.ResetLogWelcomeChannel(guildId.Value);
            
            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Log Welcome Channel", old != null ? $"<#{old}>" : null, "*None*", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent($":white_check_mark: Reset the log welcome channel!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("reset_log_mod", "Reset the channel to log moderation activity")]
        public async Task ResetLogMod()
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetLogModChannel(guildId.Value);

            Database.Database.ResetLogModChannel(guildId.Value);
            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Log Mod Channel", old != null ? $"<#{old}>" : null, "*None*", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Reset the log mod channel!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
        [SubSlashCommand("reset_appeal_link", "Reset the appeal link that gets send to the offender.")]
        public async Task ResetAppealLink()
        {
            var guildId = Context.Interaction.GuildId;
            if (guildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }
            var old = Database.Database.GetAppealLink(guildId.Value);

            Database.Database.ResetAppealLink(guildId.Value);
            await ModerationHandler.LogConfigChange(guildId.Value, Context.User, "Appeal Link", old, "*None*", Context.Client.Rest);

            var messageProperties =
                new InteractionMessageProperties().WithContent(":white_check_mark: Reset appeal link!");

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }
    }
}