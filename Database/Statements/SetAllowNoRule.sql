INSERT INTO conf (gid, allow_no_rule) VALUES (@GuildId, @Value)
ON CONFLICT (gid) DO UPDATE SET allow_no_rule = excluded.allow_no_rule;
