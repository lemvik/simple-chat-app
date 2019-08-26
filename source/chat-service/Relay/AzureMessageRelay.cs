using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using LemVic.Services.Chat.Relay.Protocol;
using Microsoft.Azure.ServiceBus;

namespace LemVic.Services.Chat.Relay
{
    public class AzureMessageRelay : IMessageRelay
    {
        private readonly ITopicClient        TopicClient;
        private readonly ISubscriptionClient SubscriptionClient;

        private Func<string, Task>         UserConnectedHandler;
        private Func<string, Task>         UserDisconnectedHandler;
        private Func<string, string, Task> UserPostedMessageHandler;
        private Func<string[], Task>       HubStatusHandler;

        public AzureMessageRelay(ITopicClient topicClient, ISubscriptionClient subscriptionClient)
        {
            TopicClient        = topicClient;
            SubscriptionClient = subscriptionClient;
            SubscriptionClient.RegisterMessageHandler(OnMessage,
                                                      new MessageHandlerOptions(OnException) {AutoComplete = true});
        }

        public Task RelayUserConnected(string user)
        {
            return TopicClient.SendAsync(SerializeMessage(new UserConnected {UserName = user}));
        }

        public Task RelayUserDisconnected(string user)
        {
            return TopicClient.SendAsync(SerializeMessage(new UserDisconnected {UserName = user}));
        }

        public Task RelayUserMessage(string user, string message)
        {
            return TopicClient.SendAsync(SerializeMessage(new UserPostedMessage {UserName = user, Message = message}));
        }

        public Task HubStatus(string[] users)
        {
            return TopicClient.SendAsync(SerializeMessage(new HubStatus {HubUsers = users}));
        }

        private static Message SerializeMessage<T>(T message) where T : class
        {
            var memoryStream = new MemoryStream();
            var serializer   = new DataContractJsonSerializer(typeof(RootMessage));
            serializer.WriteObject(memoryStream, message);
            return new Message(memoryStream.ToArray());
        }

        private Task OnException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }

        private async Task OnMessage(Message message, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var userMessage = DeserializeMessage(message);
            switch (userMessage)
            {
                case UserConnected userConnected:
                {
                    if (UserConnectedHandler != null)
                    {
                        await UserConnectedHandler(userConnected.UserName);
                    }

                    break;
                }

                case UserDisconnected userDisconnected:
                {
                    if (UserDisconnectedHandler != null)
                    {
                        await UserDisconnectedHandler(userDisconnected.UserName);
                    }

                    break;
                }

                case UserPostedMessage userPostedMessage:
                {
                    if (UserPostedMessageHandler != null)
                    {
                        await UserPostedMessageHandler(userPostedMessage.UserName, userPostedMessage.Message);
                    }

                    break;
                }

                case HubStatus hubStatus:
                {
                    if (HubStatusHandler != null)
                    {
                        await HubStatusHandler(hubStatus.HubUsers);
                    }

                    break;
                }
            }
        }

        private static RootMessage DeserializeMessage(Message message)
        {
            var memoryStream = new MemoryStream(message.Body);
            var serializer   = new DataContractJsonSerializer(typeof(RootMessage));
            return serializer.ReadObject(memoryStream) as RootMessage;
        }

        public void OnUserConnected(Func<string, Task> onUserConnected)
        {
            if (UserConnectedHandler != null)
            {
                // NOTE: for simplicity sake we don't create custom exception and don't allow more than one handler.
                throw new Exception("Cannot set more than on OnUserConnected delegate.");
            }

            UserConnectedHandler = onUserConnected;
        }

        public void OnUserDisconnected(Func<string, Task> onUserDisconnected)
        {
            if (UserDisconnectedHandler != null)
            {
                // NOTE: for simplicity sake we don't create custom exception and don't allow more than one handler.
                throw new Exception("Cannot set more than on OnUserDisconnected delegate.");
            }

            UserDisconnectedHandler = onUserDisconnected;
        }

        public void OnUserPostedMessage(Func<string, string, Task> onUserPostedMessage)
        {
            if (UserPostedMessageHandler != null)
            {
                // NOTE: for simplicity sake we don't create custom exception and don't allow more than one handler.
                throw new Exception("Cannot set more than on OnUserPostedMessage delegate.");
            }

            UserPostedMessageHandler = onUserPostedMessage;
        }

        public void OnHubStatus(Func<string[], Task> onHubStatus)
        {
            if (HubStatusHandler != null)
            {
                // NOTE: for simplicity sake we don't create custom exception and don't allow more than one handler.
                throw new Exception("Cannot set more than on OnHubStatus delegate.");
            }

            HubStatusHandler = onHubStatus;
        }
    }
}