SELECT rid, title, desc, color, img
FROM rules
WHERE gid = @GuildId AND rid = @RuleId;
