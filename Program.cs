using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

var builder = Host.CreateApplicationBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddDiscordGateway(options =>
{
    options.Token = config["Discord:Token"];
    options.Intents = GatewayIntents.MessageContent | GatewayIntents.GuildModeration | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildUsers;
});

var host = builder.Build();

await host.RunAsync();