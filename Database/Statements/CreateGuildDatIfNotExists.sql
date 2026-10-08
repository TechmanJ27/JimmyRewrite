INSERT INTO guild_dat (gid, curr_cid, curr_rid)
VALUES (@GuildId, 0, 0)
ON CONFLICT DO NOTHING;