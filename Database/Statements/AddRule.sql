INSERT INTO rules (gid, rid, title, desc, color, img)
VALUES (@GuildId, (SELECT curr_rid + 1 FROM guild_dat WHERE gid = @GuildId), @Title, @Desc, @Color, @Img);
UPDATE guild_dat SET curr_rid = curr_rid + 1 WHERE gid = @GuildId;
