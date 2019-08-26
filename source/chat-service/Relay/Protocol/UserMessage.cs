using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Namespace = "http://Simple.Chat.App")]
    [KnownType(typeof(UserConnected))]
    [KnownType(typeof(UserDisconnected))]
    [KnownType(typeof(UserPostedMessage))]
    public class UserMessage
    {
        [DataMember(Name = "n")]
        public string UserName { get; set; }
    }
}
