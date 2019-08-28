using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LemVic.Services.Chat.Relay
{
    public static class Registrations
    {
        public static IServiceCollection AddAzureRelay(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddTransient<ITopicClient>(provider => {
                                        var opts = provider.GetService<IOptions<AzureRelayOptions>>().Value;
                                        return new TopicClient(opts.ConnectionString, opts.Topic);
                                    })
                                    .AddSingleton<IMessageRelay, AzureMessageRelay>();
        }
    }
}
