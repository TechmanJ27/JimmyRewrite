using JimmyRewrite;
using JimmyRewrite.events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using CommandsModule = JimmyRewrite.Commands.CommandsModule;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDiscordGateway(options =>
{
    options.Intents = GatewayIntents.MessageContent | GatewayIntents.GuildMessages | GatewayIntents.GuildUsers;
});

builder.Services.AddApplicationCommands();
            
builder.Services.AddGatewayHandler<MessageDeleteHandler>();
builder.Services.AddGatewayHandler<MessageDeleteBulkHandler>();
builder.Services.AddGatewayHandler<MessageUpdateHandler>();
builder.Services.AddGatewayHandler<MessageCreateHandler>();
builder.Services.AddGatewayHandler<GuildUserAddHandler>();
builder.Services.AddGatewayHandler<GuildUserRemoveHandler>();

builder.Services.AddHostedService<TempBanManager>();

// TODO: Handling when the channel that is configured becomes unavailable from permission or being deleted

var host = builder.Build();

host.AddApplicationCommandModule<CommandsModule>();
host.AddApplicationCommandModule<CommandsModule.ConfigModule>();

await host.RunAsync();