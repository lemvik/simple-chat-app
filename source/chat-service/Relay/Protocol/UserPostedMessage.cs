using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Name = "upm", Namespace = "http://Simple.Chat.App")]
    public class UserPostedMessage : UserMessage
    {
        [DataMember(Name = "m")]
        public string Message  { get; set; }
    }
}
