using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot.Commands.Items
{
    public class SlowModeInfo : BaseItem
    {
        private static SlowModeInfo instance;
        public static SlowModeInfo Instance
        {
            get { return instance ?? (instance = new SlowModeInfo()); }
        }

        [Command("slowinfo", ChatUserType.Pleb)]
        public void ToggleTextToSpeech(object[] args)
        {
            var chatMessage = ParseArgument(args);
        }
    }
}
