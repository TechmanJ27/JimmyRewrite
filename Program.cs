using JimmyRewrite.events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using CommandsModule = JimmyRewrite.Commands.CommandsModule;

namespace JimmyRewrite;

internal static class Program
{
    public static ConfigurationManager ConfigManager = null!;
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        ConfigManager = builder.Configuration;
        var KLIPY_API_KEY = Config["Klipy.Api_Key"];
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

        var host = builder.Build();

        host.AddApplicationCommandModule<CommandsModule>();
        host.AddApplicationCommandModule<CommandsModule.ConfigModule>();

        await host.RunAsync();
    }
}
