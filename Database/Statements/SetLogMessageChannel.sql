INSERT INTO conf (gid, message) VALUES (@GuildId, @ChannelId)
ON CONFLICT (gid) DO UPDATE SET message = excluded.message;