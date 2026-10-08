INSERT INTO members (gid, uid, watch) VALUES (@GuildId, @UserId, @Watch)
ON CONFLICT (gid) DO UPDATE SET watch = excluded.watch;