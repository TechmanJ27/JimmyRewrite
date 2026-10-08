INSERT INTO conf (gid, watch) VALUES (@GuildId, @ChannelId)
ON CONFLICT (gid) DO UPDATE SET watch = excluded.watch;