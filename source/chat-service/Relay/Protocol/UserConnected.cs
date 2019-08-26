using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Name = "uc", Namespace = "http://Simple.Chat.App")]
    public class UserConnected : UserMessage
    {
    }
}
