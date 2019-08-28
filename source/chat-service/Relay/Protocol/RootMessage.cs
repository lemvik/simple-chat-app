using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Namespace = "http://Simple.Chat.App")]
    [KnownType(typeof(UserMessage))]
    [KnownType(typeof(UserPostedMessage))]
    public class RootMessage
    {
    }
}
