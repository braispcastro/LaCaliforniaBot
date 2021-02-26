using Google.Cloud.Logging.Type;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaCaliforniaBot.Commands.Items
{
    public class MaxCharactersTextToSpeech : BaseItem
    {
        private static MaxCharactersTextToSpeech instance;
        public static MaxCharactersTextToSpeech Instance
        {
            get { return instance ?? (instance = new MaxCharactersTextToSpeech()); }
        }

        [Command("ttsmax", ChatUserType.Broadcaster | ChatUserType.Moderator)]
        public void MaxCharacters(object[] args)
        {
            try
            {
                var chatCommand = ParseArgument(args);
                var arg = GetFirstParameter(chatCommand.ArgumentsAsList);
                if (string.IsNullOrEmpty(arg))
                    return;

                if (int.TryParse(arg, out int maxCharacters) && maxCharacters >= 0)
                    UpdateMaxCharacters(chatCommand.ChatMessage.Username, maxCharacters);
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        #region Private Methods

        private void UpdateMaxCharacters(string username, int maxCharacters)
        {
            Configuration.TextToSpeechMaxCharacters = maxCharacters;
            TwitchBot.Instance.LogMessage(LogSeverity.Notice, $"Se han establecido {maxCharacters} caracteres máximos por {username}");
        }

        #endregion
    }
}
