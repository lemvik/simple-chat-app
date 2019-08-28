using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using LemVic.Services.Chat.Relay.Protocol;
using Microsoft.Azure.ServiceBus;

namespace LemVic.Services.Chat.Relay
{
    public class AzureMessageRelay : IMessageRelay
    {
        private readonly ITopicClient TopicClient;

        public AzureMessageRelay(ITopicClient topicClient)
        {
            TopicClient = topicClient;
        }

        public Task RelayUserMessage(string user, string message)
        {
            return TopicClient.SendAsync(SerializeMessage(new UserPostedMessage {UserName = user, Message = message}));
        }

        private static Message SerializeMessage<T>(T message) where T : class
        {
            var memoryStream = new MemoryStream();
            var serializer   = new DataContractJsonSerializer(typeof(RootMessage));
            serializer.WriteObject(memoryStream, message);
            return new Message(memoryStream.ToArray());
        }
    }
}
