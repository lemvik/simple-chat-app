using System.Runtime.Serialization;

namespace LemVic.Services.Chat.Relay.Protocol
{
    [DataContract(Name = "hs", Namespace = "http://Simple.Chat.App")]
    public class HubStatus : RootMessage
    {
        [DataMember(Name = "hu")]
        public string[] HubUsers { get; set; }
    }
}
