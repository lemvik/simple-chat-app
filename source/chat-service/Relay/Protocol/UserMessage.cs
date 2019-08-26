using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Namespace = "http://Simple.Chat.App")]
    public class UserMessage : RootMessage
    {
        [DataMember(Name = "n")]
        public string UserName { get; set; }
    }
}
