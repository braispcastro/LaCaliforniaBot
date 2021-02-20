using System;
using Google.Cloud.Logging.Type;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot.Commands.Items
{
    public class ToggleTextToSpeech : BaseItem
    {
        private static ToggleTextToSpeech instance;
        public static ToggleTextToSpeech Instance
        {
            get { return instance ?? (instance = new ToggleTextToSpeech()); }
        }

        [Command("tts", ChatUserType.Broadcaster | ChatUserType.Moderator)]
        public void Toggle(object[] args)
        {
            try
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
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        #region Private Methods

        private void ChangeTextToSpeechDelay(string username, int delay)
        {
            Configuration.TextToSpeechDelay = delay;
            TwitchBot.Instance.LogMessage(LogSeverity.Info, $"Slow mode de {delay} segundos establecido por {username}");
        }

        private void EnableTextToSpeech(string username)
        {
            Configuration.TextToSpeechEnabled = true;
            TwitchBot.Instance.LogMessage(LogSeverity.Info, $"TTS activado por {username}");
        }

        private void DisableTextToSpeech(string username)
        {
            Configuration.TextToSpeechEnabled = false;
            California.Instance.ClearUserList(); // Se resetea la lista de slowmode para liberar memoria
            TwitchBot.Instance.LogMessage(LogSeverity.Info, $"TTS desactivado por {username}");
        }

        #endregion
    }
}
