DELETE FROM rules WHERE gid = @GuildId AND rid = @RuleId;
UPDATE rules SET rid = rid - 1 WHERE gid = @GuildId AND rid > @RuleId;
UPDATE guild_dat SET curr_rid = (SELECT COALESCE(MAX(rid), 0) FROM rules WHERE gid = @GuildId) WHERE gid = @GuildId;
