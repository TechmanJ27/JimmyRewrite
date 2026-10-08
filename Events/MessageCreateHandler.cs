using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace JimmyRewrite.events;

public class MessageCreateHandler() : IMessageCreateGatewayHandler
{
    public ValueTask HandleAsync(Message message)
    {
        Console.WriteLine($"Message from {message.Author.Username}: {message.Content}");
        return default;
    }
}