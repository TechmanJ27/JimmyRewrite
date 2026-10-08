INSERT INTO tbans (gid, uid, expire) VALUES (@GuildId, @UserId, @CaseId, @Expire)
ON CONFLICT (gid, uid) DO UPDATE SET expire = excluded.expire;