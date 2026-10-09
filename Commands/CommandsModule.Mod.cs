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
            var messageProperties =
                new InteractionMessageProperties().WithContent(
                    ":x: You or I have less permission than the target user!");

            return context.Interaction.SendResponseAsync(
                InteractionCallback.Message(messageProperties)
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
            var bytes = MessagePackSerializer.Serialize(modAction);
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

        // TODO: Limit note length and maximum rule
        [RequireUserPermissions<ApplicationCommandContext>(Permissions.BanUsers)]
        [RequireBotPermissions<ApplicationCommandContext>(Permissions.BanUsers)]
        [SubSlashCommand("ban", "Ban a user")]
        public async Task Ban(User user, string rule, string duration = "0m", string? note = null,
            Attachment? attachment = null)
        {
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

            var parsedDuration = ParseHelper.ParseDuration(duration);
            if (parsedDuration == null)
            {
                await SendInvalidDurationError(Context);
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

            var modAction = new ModAction(ModerationHandler.PunishmentType.Ban,
                Context.User.Id,
                user.Id,
                parsedRules,
                parsedDuration.Value,
                note);

            if (actionConfirm)
            {
                await SendConfirmation(Context, modAction, attachmentId);
                return;
            }
            
            modAction = new ModAction(ModerationHandler.PunishmentType.Ban,
                Context.User.Id,
                user.Id,
                parsedRules,
                parsedDuration.Value,
                note,
                attachment?.Url);

            try
            {
                await ModerationHandler.DoModAction(guildId, modAction, Context.Client.Rest);
            }
            catch (ModerationHandler.InvalidRules)
            {
                await Context.Interaction.SendResponseAsync(
                    InteractionCallback.Message(
                        new InteractionMessageProperties()
                            .WithContent(":x: Invalid rule!")
                    )
                );
            }
            catch (ModerationHandler.NoRuleAllowedIsOff)
            {
                await Context.Interaction.SendResponseAsync(
                    InteractionCallback.Message(
                        new InteractionMessageProperties()
                            .WithContent(":x: Allow no rule is off!")
                    )
                );
                return;
            }

            await Context.Interaction.SendResponseAsync(
                InteractionCallback.Message(
                    new InteractionMessageProperties()
                        .WithContent(":x: Done!")
                )
            );
        }
    }
}