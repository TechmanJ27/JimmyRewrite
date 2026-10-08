INSERT INTO conf (gid, mod_action_confirm) VALUES (@GuildId, @Value)
ON CONFLICT (gid) DO UPDATE SET mod_action_confirm = excluded.mod_action_confirm;
