UPDATE rules
SET title = @Title,
    desc = @Desc,
    color = @Color,
    img = @Img
WHERE gid = @GuildId AND rid = @RuleId;
