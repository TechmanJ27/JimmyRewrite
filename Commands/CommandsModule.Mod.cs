// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using Humanizer;
using MessagePack;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace JimmyRewrite.Commands;

public partial class CommandsModule
{
    [SlashCommand("mod", "Moderate the server",
        DefaultGuildPermissions = Permissions.ModerateUsers,
        Contexts = [InteractionContextType.Guild])]
    public class ModModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        public const string AttachmentLeadingUrl = "https://cdn.discordapp.com/ephemeral-attachments/";

        private static Task<InteractionCallbackResponse?> SendInvalidRuleError(ApplicationCommandContext context)
        {
            var messageProperties =
                new InteractionMessageProperties().WithContent(":x: Invalid rule!");

            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        private static Task<InteractionCallbackResponse?> SendInvalidDurationError(ApplicationCommandContext context)
        {
            var messageProperties =
                new InteractionMessageProperties().WithContent(
                    ":x: Invalid duration! Supported day, hour, and minute but NOT second\n" +
                    "Example: \"1d2h3m\"");

            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        private static Task<InteractionCallbackResponse?> SendImageOnlyError(ApplicationCommandContext context)
        {
            var messageProperties =
                new InteractionMessageProperties().WithContent(":x: Only images are allowed!");

            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        private static Task<InteractionCallbackResponse?> SendPermLessThanError(ApplicationCommandContext context)
        {
            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(
                    new InteractionMessageProperties()
                        .WithContent(":x: You or I (the bot) have less permission than the target user or doesn't have the permission required!\n" +
                                     "Check your and my role if it's higher than the target user's role or not and if you have the permission to actually run the command!")
                )
            );
        }

        private static Task<InteractionCallbackResponse?> SendNoteLengthError(ApplicationCommandContext context)
        {
            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(
                    new InteractionMessageProperties()
                        .WithContent(":x: Note is too long! Max: 1024 characters")
                )
            );
        }

        private static Task<InteractionCallbackResponse?> SendRuleLengthError(ApplicationCommandContext context)
        {
            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(
                    new InteractionMessageProperties()
                        .WithContent(":x: Too many rules! Max: 10 rules")
                )
            );
        }

        private static EmbedProperties GetConfirmationEmbed(ModAction modAction)
        {
            List<EmbedFieldProperties> embedFieldList =
            [
                new EmbedFieldProperties()
                    .WithName("Punishment Type")
                    .WithValue(modAction.Type.ToString()),
                new EmbedFieldProperties()
                    .WithName("Moderator")
                    .WithValue($"<@{modAction.ModId}> ({modAction.ModId})")
                    .WithInline(),
                new EmbedFieldProperties()
                    .WithName("User")
                    .WithValue($"<@{modAction.UserId}> ({modAction.UserId})")
                    .WithInline()
            ];

            if (modAction.Rules.Length > 0)
            {
                embedFieldList.Add(new EmbedFieldProperties()
                    .WithName("Rule IDs")
                    .WithValue(string.Join(", ", modAction.Rules))
                    .WithInline()
                );
            }

            if (modAction.DurationMinute > 0)
            {
                embedFieldList.Add(new EmbedFieldProperties()
                    .WithName("Duration")
                    .WithValue($"{TimeSpan.FromMinutes(modAction.DurationMinute).Humanize()}")
                    .WithInline()
                );
            }

            if (modAction.Note != null)
            {
                embedFieldList.Add(new EmbedFieldProperties()
                    .WithName("Note")
                    .WithValue(modAction.Note)
                    .WithInline()
                );
            }

            var embed = new EmbedProperties()
                .WithTitle("Moderation Action Confirmation")
                .WithFields(embedFieldList);

            if (modAction.Image != null)
            {
                embed.WithImage(new EmbedImageProperties(modAction.Image));
            }

            return embed;
        }

        private static string GetConfirmationCustomId(ModAction modAction, long? attachmentId = null)
        {
            var modActionNoAttachment = new ModAction(
                modAction.Type,
                modAction.ModId,
                modAction.UserId,
                modAction.Rules,
                modAction.DurationMinute,
                modAction.Note
            );
            var bytes = MessagePackSerializer.Serialize(modActionNoAttachment);
            var base64 = Convert.ToBase64String(bytes);
            var attachmentIdBase64 = attachmentId != null
                ? Convert.ToBase64String(BitConverter.GetBytes(attachmentId.Value))
                : "";
            return $"mac:{base64}:{attachmentIdBase64}";
        }

        private static InteractionMessageProperties GetConfirmationMessage(ModAction modAction, string customId)
        {
            var embed = GetConfirmationEmbed(modAction);
            return new InteractionMessageProperties()
                .WithEmbeds([
                    embed
                ]).WithComponents([
                    new ActionRowProperties
                    {
                        new ButtonProperties(customId, "Confirm", ButtonStyle.Primary)
                    }
                ]);
        }

        public static InteractionMessageProperties GetConfirmationMessage(ModAction modAction,
            long? attachmentId = null)
        {
            var customId = GetConfirmationCustomId(modAction, attachmentId);
            return GetConfirmationMessage(modAction, customId);
        }

        private static Task<InteractionCallbackResponse?> SendConfirmation(ApplicationCommandContext context,
            string customId, ModAction modAction)
        {
            var messageProperties = GetConfirmationMessage(modAction, customId);

            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
            );
        }

        private static Task<InteractionCallbackResponse?> SendConfirmation(ApplicationCommandContext context,
            ModAction modAction, long? attachmentId = null)
        {
            var customId = GetConfirmationCustomId(modAction, attachmentId);
            return SendConfirmation(context, customId, modAction);
        }

        private static long? CacheAttachment(Attachment? attachment)
        {
            if (attachment == null)
                return null;

            var attachmentUrlCut = attachment.Url.Remove(0, AttachmentLeadingUrl.Length);
            var expirationInfo = attachment.GetExpirationInfo();
            return AttachmentUrlCache.Add(attachmentUrlCut, expirationInfo.ExpiresAt);
        }

        [SubSlashCommand("offend", "Make a punishment")]
        public async Task Offend(
            [SlashCommandParameter(Name = "user", Description = "The user to do a punishment")] User user,
            [SlashCommandParameter(Name = "type", Description = "Type of the punishment")] ModerationHandler.PunishmentType type,
            [SlashCommandParameter(Name = "rule", Description = "Rule that they broke (Max: 10) (Example \"1,5,6,7\")")] string rule = "0",
            [SlashCommandParameter(Name = "duration", Description = "The duration of the punishment (timeout/ban)")] string duration = "0m",
            [SlashCommandParameter(Name = "note", Description = "Additional note about the punishment (Max: 1024)")] string? note = null,
            [SlashCommandParameter(Name = "attachment", Description = "Additional image about the punishment")] Attachment? attachment = null)
        {
            if (note?.Length > 1024)
            {
                await SendNoteLengthError(Context);
                return;
            }
            
            if (Context.Interaction.GuildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }

            var guildId = Context.Interaction.GuildId.Value;

            if (attachment != null && (attachment.ContentType == null || !attachment.ContentType.StartsWith("image")))
            {
                await SendImageOnlyError(Context);
                return;
            }

            var parsedRules = ParseHelper.ParseRuleIds(rule);
            if (parsedRules == null)
            {
                await SendInvalidRuleError(Context);
                return;
            }

            if (parsedRules.Length > 10)
            {
                await SendRuleLengthError(Context);
                return;
            }

            var parsedDuration = ParseHelper.ParseDuration(duration);
            if (parsedDuration == null && type is ModerationHandler.PunishmentType.Ban or ModerationHandler.PunishmentType.Timeout)
            {
                await SendInvalidDurationError(Context);
                return;
            }
            
            var requiredPerms = UserHelper.GetRequiredPermissions(type);
            var hasModPerm = await UserHelper.HasPermissionsAsync(Context.User.Id, guildId, requiredPerms, Context.Client.Rest, Context.Guild);
            var hasBotPerm = await UserHelper.HasPermissionsAsync(Context.Client.Id, guildId, requiredPerms, Context.Client.Rest, Context.Guild);

            if (!hasModPerm || !hasBotPerm)
            {
                await SendPermLessThanError(Context);
                return;
            }

            var canModModerate =
                await UserHelper.CanModerateAsync(Context.User, user, guildId, Context.Client.Rest, Context.Guild);
            var canBotModerate = await UserHelper.CanModerateAsync(Context.Client.Id, user.Id, guildId,
                Context.Client.Rest, Context.Guild);

            if (!canModModerate || !canBotModerate)
            {
                await SendPermLessThanError(Context);
                return;
            }

            var actionConfirm = Database.Database.GetModActionConfirm(Context.Interaction.GuildId.Value);

            var attachmentId = CacheAttachment(attachment);

            var modAction = new ModAction(type,
                Context.User.Id,
                user.Id,
                parsedRules,
                parsedDuration ?? 0,
                note,
                attachment?.Url);

            if (actionConfirm)
            {
                await SendConfirmation(Context, modAction, attachmentId);
                return;
            }
            
            await Context.Interaction.SendResponseAsync(
                InteractionCallback.DeferredMessage()
            );
            
            try
            {
                await ModerationHandler.DoModAction(guildId, modAction, Context.Client.Rest);
            }
            catch (ModerationHandler.InvalidRules)
            {
                await Context.Interaction.ModifyResponseAsync(options =>
                    {
                        options.WithContent(":x: Invalid rule!");
                    }
                );
            }
            catch (ModerationHandler.NoRuleAllowedIsOff)
            {
                await Context.Interaction.ModifyResponseAsync(options =>
                    {
                        options.WithContent(":x: Allow no rule is off!");
                    }
                );
                return;
            }
            catch (ImageUploader.ImageUploadFailedException)
            {
                await Context.Interaction.ModifyResponseAsync(options =>
                    {
                        options.WithContent(":x: Failed to upload image!\n" +
                                            "Consider trying again later or run the command again but without the image?");
                    }
                );
                return;
            }

            await Context.Interaction.ModifyResponseAsync(options =>
                {
                    options.WithContent(":x: Done!");
                }
            );
        }
        
                [SubSlashCommand("offend_id", "Make a punishment with user ID")]
        public async Task OffendId(
            [SlashCommandParameter(Name = "user", Description = "The user to do a punishment")] ulong user,
            [SlashCommandParameter(Name = "type", Description = "Type of the punishment")] ModerationHandler.PunishmentType type,
            [SlashCommandParameter(Name = "rule", Description = "Rule that they broke (Max: 10) (Example \"1,5,6,7\")")] string rule = "0",
            [SlashCommandParameter(Name = "duration", Description = "The duration of the punishment (timeout/ban)")] string duration = "0m",
            [SlashCommandParameter(Name = "note", Description = "Additional note about the punishment (Max: 1024)")] string? note = null,
            [SlashCommandParameter(Name = "attachment", Description = "Additional image about the punishment")] Attachment? attachment = null)
        {
            if (note?.Length > 1024)
            {
                await SendNoteLengthError(Context);
                return;
            }
            
            if (Context.Interaction.GuildId == null)
            {
                await SendGuildOnlyError(Context);
                return;
            }

            var guildId = Context.Interaction.GuildId.Value;

            if (attachment != null && (attachment.ContentType == null || !attachment.ContentType.StartsWith("image")))
            {
                await SendImageOnlyError(Context);
                return;
            }

            var parsedRules = ParseHelper.ParseRuleIds(rule);
            if (parsedRules == null)
            {
                await SendInvalidRuleError(Context);
                return;
            }

            if (parsedRules.Length > 10)
            {
                await SendRuleLengthError(Context);
                return;
            }

            var parsedDuration = ParseHelper.ParseDuration(duration);
            if (parsedDuration == null && type is ModerationHandler.PunishmentType.Ban or ModerationHandler.PunishmentType.Timeout)
            {
                await SendInvalidDurationError(Context);
                return;
            }
            
            var requiredPerms = UserHelper.GetRequiredPermissions(type);
            var hasModPerm = await UserHelper.HasPermissionsAsync(Context.User.Id, guildId, requiredPerms, Context.Client.Rest, Context.Guild);
            var hasBotPerm = await UserHelper.HasPermissionsAsync(Context.Client.Id, guildId, requiredPerms, Context.Client.Rest, Context.Guild);

            if (!hasModPerm || !hasBotPerm)
            {
                await SendPermLessThanError(Context);
                return;
            }

            var actionConfirm = Database.Database.GetModActionConfirm(Context.Interaction.GuildId.Value);

            var attachmentId = CacheAttachment(attachment);

            var modAction = new ModAction(type,
                Context.User.Id,
                user,
                parsedRules,
                parsedDuration ?? 0,
                note,
                attachment?.Url);

            if (actionConfirm)
            {
                await SendConfirmation(Context, modAction, attachmentId);
                return;
            }
            
            await Context.Interaction.SendResponseAsync(
                InteractionCallback.DeferredMessage()
            );
            
            try
            {
                await ModerationHandler.DoModAction(guildId, modAction, Context.Client.Rest);
            }
            catch (ModerationHandler.InvalidRules)
            {
                await Context.Interaction.ModifyResponseAsync(options =>
                    {
                        options.WithContent(":x: Invalid rule!");
                    }
                );
            }
            catch (ModerationHandler.NoRuleAllowedIsOff)
            {
                await Context.Interaction.ModifyResponseAsync(options =>
                    {
                        options.WithContent(":x: Allow no rule is off!");
                    }
                );
                return;
            }
            catch (ImageUploader.ImageUploadFailedException)
            {
                await Context.Interaction.ModifyResponseAsync(options =>
                    {
                        options.WithContent(":x: Failed to upload image!\n" +
                                            "Consider trying again later or run the command again but without the image?");
                    }
                );
                return;
            }

            await Context.Interaction.ModifyResponseAsync(options =>
                {
                    options.WithContent(":x: Done!");
                }
            );
        }
        // TODO: View Cases with filter functionality, Etc
    }
}
