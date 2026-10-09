// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using MessagePack;
using NetCord;

namespace JimmyRewrite;

[MessagePackObject]
public record ModAction
{
    [Key(0)] public ModerationHandler.PunishmentType Type { get; }
    [Key(1)] public ulong ModId { get; }
    [Key(2)] public ulong UserId { get; }
    [Key(3)] public int[] Rules { get; } = null!;
    [Key(4)] public long DurationMinute { get; }
    [Key(5)] public string? Note { get; }
    [Key(6)] public string? Image { get; }

    [SerializationConstructor]
    public ModAction(
        ModerationHandler.PunishmentType type,
        ulong modId,
        ulong userId,
        int[] rules,
        long durationMinute,
        string? note,
        string? image = null
    )
    {
        Type = type;
        ModId = modId;
        UserId = userId;
        Rules = rules;
        DurationMinute = durationMinute;
        Note = note;
        Image = image;
    }
}