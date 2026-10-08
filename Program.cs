using JimmyRewrite.events;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using CommandsModule = JimmyRewrite.Commands.Global.CommandsModule;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordGateway(options =>
{
    options.Intents = GatewayIntents.MessageContent | GatewayIntents.GuildModeration | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildUsers;
});

builder.Services.AddApplicationCommands();
builder.Services.AddGatewayHandler<MessageCreateHandler>();

var host = builder.Build();

host.AddApplicationCommandModule<CommandsModule>();

await host.RunAsync();