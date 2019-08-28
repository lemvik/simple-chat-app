using System;
using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Name = "upm", Namespace = "http://Simple.Chat.App")]
    public class UserPostedMessage : UserMessage
    {
        [DataMember(Name = "m")]
        public string Message { get; set; }

        [DataMember(Name = "id")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        [DataMember(Name = "t")]
        public long Timestamp { get; set; } = DateTime.UtcNow.ToFileTimeUtc();
    }
}
