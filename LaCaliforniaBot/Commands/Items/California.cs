using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot.Commands.Items
{
    public class California
    {
        private static California instance;
        public static California Instance
        {
            get { return instance ?? (instance = new California()); }
        }

        [Command("k", ChatUserType.Broadcaster | ChatUserType.Moderator | ChatUserType.Subscriber | ChatUserType.Vip)]
        public void PlayMessage()
        {
            
        }
    }
}
