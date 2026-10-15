// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using NetCord;
using NetCord.Services.ApplicationCommands;

namespace JimmyRewrite.Commands;

public partial class CommandsModule
{
    [SlashCommand("rules", "Moderate the server",
        DefaultGuildPermissions = Permissions.ModerateUsers,
        Contexts = [InteractionContextType.Guild])]
    public class RulesModule : ApplicationCommandModule<ApplicationCommandContext>
    {
    }
}