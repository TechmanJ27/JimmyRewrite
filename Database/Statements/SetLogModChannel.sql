INSERT INTO conf (gid, mod) VALUES (@GuildId, @ChannelId)
ON CONFLICT (gid) DO UPDATE SET mod = excluded.mod;