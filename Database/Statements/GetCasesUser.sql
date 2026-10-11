SELECT id, gid, uid, type, rid, rule_title, mod_uid, note, mod_note, time, tban_expire
FROM cases
WHERE gid = @GuildId AND uid = @UserId
ORDER BY id;