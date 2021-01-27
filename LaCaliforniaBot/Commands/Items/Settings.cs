using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot.Commands.Items
{
    public class Settings
    {
        private static Settings instance;
        public static Settings Instance
        {
            get { return instance ?? (instance = new Settings()); }
        }

        [Command("tts", ChatUserType.Broadcaster | ChatUserType.Moderator)]
        public void ToggleTextToSpeech()
        {

        }
    }
}
