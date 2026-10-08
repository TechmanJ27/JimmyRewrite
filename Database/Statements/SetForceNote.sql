INSERT INTO conf (gid, force_note) VALUES (@GuildId, @Value)
ON CONFLICT (gid) DO UPDATE SET force_note = excluded.force_note;
