INSERT INTO conf (gid, welcome) VALUES (@GuildId, @ChannelId)
ON CONFLICT (gid) DO UPDATE SET welcome = excluded.welcome;