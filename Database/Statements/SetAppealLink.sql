INSERT INTO conf (gid, appeal_link) VALUES (@GuildId, @Text)
ON CONFLICT (gid) DO UPDATE SET appeal_link = excluded.appeal_link;