INSERT INTO conf (gid, allow_custom_rule) VALUES (@GuildId, @Value)
ON CONFLICT (gid) DO UPDATE SET allow_custom_rule = excluded.allow_custom_rule;
