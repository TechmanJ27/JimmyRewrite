// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using NetCord;
using NetCord.Gateway;
using NetCord.Rest;

namespace JimmyRewrite;

public class UserHelper
{
    public static RolePosition? GetHighestRolePosition(GuildUser? guildUser, IReadOnlyDictionary<ulong, Role> roles)
    {
        if (guildUser == null || !guildUser.RoleIds.Any())
            return null;

        return guildUser.RoleIds
            .Select(roles.GetValueOrDefault)
            .Where(role => role != null)
            .Select(role => (RolePosition?)role!.Position)
            .Max();
    }

    public static bool CanModerate(GuildUser? actor, GuildUser? target, IReadOnlyDictionary<ulong, Role> roles, ulong? ownerId = null)
    {
        if (target == null)
            return true;

        if (actor == null)
            return false;

        if (actor.Id == target.Id)
            return false;

        if (ownerId.HasValue)
        {
            if (target.Id == ownerId.Value)
                return false;

            if (actor.Id == ownerId.Value)
                return true;
        }

        var actorPos = GetHighestRolePosition(actor, roles);
        var targetPos = GetHighestRolePosition(target, roles);

        if (actorPos == null)
            return false;

        if (targetPos == null)
            return true;

        return actorPos.Value.CompareTo(targetPos.Value) > 0;
    }

    public static bool CanModerate(GuildUser? actor, GuildUser? target, Guild guild)
    {
        return CanModerate(actor, target, guild.Roles, guild.OwnerId);
    }

    public static async Task<GuildUser?> GetGuildUserAsync(ulong guildId, ulong userId, RestClient client, Guild? cachedGuild = null)
    {
        var guildUser = cachedGuild?.Users.GetValueOrDefault(userId);
        if (guildUser != null)
            return guildUser;

        try
        {
            return await client.GetGuildUserAsync(guildId, userId);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<bool> CanModerateAsync(
        ulong actorId,
        ulong targetId,
        ulong guildId,
        RestClient client,
        Guild? cachedGuild = null)
    {
        return await CanModerateAsync(null, actorId, null, targetId, guildId, client, cachedGuild);
    }

    public static async Task<bool> CanModerateAsync(
        User actor,
        User target,
        ulong guildId,
        RestClient client,
        Guild? cachedGuild = null)
    {
        var actorGuildUser = actor as GuildUser;
        var targetGuildUser = target as GuildUser;

        return await CanModerateAsync(actorGuildUser, actor.Id, targetGuildUser, target.Id, guildId, client, cachedGuild);
    }

    public static async Task<bool> CanModerateAsync(
        GuildUser? actorGuildUser,
        ulong actorId,
        GuildUser? targetGuildUser,
        ulong targetId,
        ulong guildId,
        RestClient client,
        Guild? cachedGuild = null)
    {
        IReadOnlyDictionary<ulong, Role> roles;
        ulong? ownerId;

        if (cachedGuild != null)
        {
            roles = cachedGuild.Roles;
            ownerId = cachedGuild.OwnerId;
        }
        else
        {
            try
            {
                var guild = await client.GetGuildAsync(guildId);
                roles = guild.Roles;
                ownerId = guild.OwnerId;
            }
            catch
            {
                return false;
            }
        }

        targetGuildUser ??= await GetGuildUserAsync(guildId, targetId, client, cachedGuild);
        if (targetGuildUser == null)
            return true;

        actorGuildUser ??= await GetGuildUserAsync(guildId, actorId, client, cachedGuild);
        return actorGuildUser != null && CanModerate(actorGuildUser, targetGuildUser, roles, ownerId);
    }

    public static Permissions GetRequiredPermissions(ModerationHandler.PunishmentType type) => type switch
    {
        ModerationHandler.PunishmentType.Ban => Permissions.BanUsers,
        ModerationHandler.PunishmentType.Kick => Permissions.KickUsers,
        ModerationHandler.PunishmentType.Timeout => Permissions.ModerateUsers,
        ModerationHandler.PunishmentType.Warn => Permissions.ModerateUsers,
        ModerationHandler.PunishmentType.Appeal => Permissions.BanUsers,
        _ => Permissions.ModerateUsers
    };

    public static async Task<bool> HasPermissionsAsync(
        ulong userId,
        ulong guildId,
        Permissions requiredPermissions,
        RestClient client,
        Guild? cachedGuild = null)
    {
        if (cachedGuild != null)
        {
            if (cachedGuild.OwnerId == userId)
                return true;

            var user = cachedGuild.Users.GetValueOrDefault(userId) ?? await GetGuildUserAsync(guildId, userId, client, cachedGuild);
            if (user == null)
                return false;

            var perms = user.GetPermissions(cachedGuild);
            return (perms & requiredPermissions) == requiredPermissions;
        }

        try
        {
            var guild = await client.GetGuildAsync(guildId);
            if (guild.OwnerId == userId)
                return true;

            var user = await GetGuildUserAsync(guildId, userId, client);
            if (user == null)
                return false;

            var perms = user.GetPermissions(guild);
            return (perms & requiredPermissions) == requiredPermissions;
        }
        catch
        {
            return false;
        }
    }

    public static Role? GetHighestRole(GuildUser? guildUser, Guild guild)
    {
        if (guildUser == null || !guildUser.RoleIds.Any())
        {
            return guild.Roles.Values.FirstOrDefault(r => r.Id == guild.Id);
        }

        return guildUser.RoleIds
            .Select(guild.Roles.GetValueOrDefault)
            .Where(role => role != null)
            .OrderByDescending(role => role!.Position)
            .FirstOrDefault();
    }
}