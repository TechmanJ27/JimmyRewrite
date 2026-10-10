INSERT INTO members (gid, uid, watch) VALUES (@GuildId, @UserId, @Watch)
ON CONFLICT (gid, uid) DO UPDATE SET watch = excluded.watch;