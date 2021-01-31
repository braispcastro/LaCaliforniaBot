using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot.Commands.Items
{
    public class Settings : BaseItem
    {
        private static Settings instance;
        public static Settings Instance
        {
            get { return instance ?? (instance = new Settings()); }
        }

        [Command("tts", ChatUserType.Broadcaster | ChatUserType.Moderator)]
        public void ToggleTextToSpeech(object[] args)
        {
            var chatCommand = ParseArgument(args);
            var arg = GetFirstParameter(chatCommand.ArgumentsAsList);
            if (string.IsNullOrEmpty(arg))
                return;


            if (int.TryParse(arg, out int delay))
                ChangeTextToSpeechDelay(chatCommand.ChatMessage.Username, delay);

            else if (arg.ToLowerInvariant() == "on" && !Configuration.TextToSpeechEnabled)
                EnableTextToSpeech(chatCommand.ChatMessage.Username);

            else if (arg.ToLowerInvariant() == "off" && Configuration.TextToSpeechEnabled)
                DisableTextToSpeech(chatCommand.ChatMessage.Username);
        }

        #region Private Methods

        private void ChangeTextToSpeechDelay(string username, int delay)
        {
            Configuration.TextToSpeechDelay = delay;
            TwitchBot.Instance.LogMessage($"*** Slow mode de {delay} segundos establecido por {username} ***");
        }

        private void EnableTextToSpeech(string username)
        {
            Configuration.TextToSpeechEnabled = true;
            TwitchBot.Instance.LogMessage($"*** TTS activado por {username} ***");
        }

        private void DisableTextToSpeech(string username)
        {
            Configuration.TextToSpeechEnabled = false;
            TwitchBot.Instance.LogMessage($"*** TTS desactivado por {username} ***");
        }

        #endregion
    }
}
