INSERT INTO cases (id, gid, uid, type, rid, rule_title, mod_uid, note, time, tban_expire) VALUES (@CaseId, @GuildId, @UserId, @Type, @RuleId, @RuleTitle, @ModUid, @Note, @Time, @TbanExpire)
ON CONFLICT (id, gid) DO NOTHING;
UPDATE guild_dat SET curr_cid = curr_cid + 1 WHERE gid = @GuildId;
