using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Name = "ud", Namespace = "http://Simple.Chat.App")]
    public class UserDisconnected : UserMessage
    {
    }
}
