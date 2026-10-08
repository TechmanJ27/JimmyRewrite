INSERT INTO members (gid, uid, watch)
VALUES (@GuildId, @UserId, 0)
ON CONFLICT DO NOTHING;