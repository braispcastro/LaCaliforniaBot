using System;
using Google.Cloud.Logging.Type;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;
using LaCaliforniaBot.Model;
using TwitchLib.Client.Models;

namespace LaCaliforniaBot.Commands.Items
{
    public class PermitTextToSpeech : BaseItem
    {
        private static PermitTextToSpeech instance;
        public static PermitTextToSpeech Instance
        {
            get { return instance ?? (instance = new PermitTextToSpeech()); }
        }

        [Command("allowtts", ChatUserType.Broadcaster | ChatUserType.Moderator)]
        public void AllowTextToSpeech(object[] args)
        {
            try
            {
                var parsedArgs = ParseArgument(args);

                var customArgs = ProcessAllowArguments(parsedArgs);
                if (customArgs == null)
                    return;

                TwitchBot.Instance.AllowedUsersToTalk.RemoveAll(x => x.Username == customArgs.Username);
                TwitchBot.Instance.AllowedUsersToTalk.Add(new AllowedUserDTO
                {
                    Username = customArgs.Username,
                    MinutesAllowed = customArgs.MinutesAllowed,
                    AllowedAt = DateTime.UtcNow
                });

                TwitchBot.Instance.LogMessage(LogSeverity.Info, $"Permitido el TTS a {customArgs.Username} durante {customArgs.MinutesAllowed} minuto(s) por {parsedArgs.ChatMessage.Username}");
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        [Command("denytts", ChatUserType.Broadcaster | ChatUserType.Moderator)]
        public void DenyTextToSpeech(object[] args)
        {
            try
            {
                var parsedArgs = ParseArgument(args);

                var username = ProcessDenyArguments(parsedArgs);
                if (username == null)
                    return;

                if (TwitchBot.Instance.AllowedUsersToTalk.RemoveAll(x => x.Username == username) > 0)
                {
                    TwitchBot.Instance.LogMessage(LogSeverity.Info, $"Denegado el TTS a {username} por {parsedArgs.ChatMessage.Username}");
                }
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        #region Private Methods

        private PermitTextToSpeechArguments ProcessAllowArguments(ChatCommand chatCommand)
        {
            var parsedArgs = chatCommand.ArgumentsAsList;

            if (parsedArgs == null || parsedArgs.Count != 2)
                return null;

            if (!int.TryParse(parsedArgs[1], out int minutes))
                return null;

            // Eliminamos la @ por si usan el comando con una mención
            var username = parsedArgs[0].ToLowerInvariant().Replace("@", "");

            return new PermitTextToSpeechArguments
            {
                Username = username,
                MinutesAllowed = minutes
            };
        }

        private string ProcessDenyArguments(ChatCommand chatCommand)
        {
            var parsedArgs = chatCommand.ArgumentsAsList;

            if (parsedArgs == null || parsedArgs.Count != 1)
                return null;

            // Eliminamos la @ por si usan el comando con una mención
            return parsedArgs[0].ToLowerInvariant().Replace("@", "");
        }

        #endregion
    }

    public class PermitTextToSpeechArguments
    {
        public string Username { get; set; }
        public int MinutesAllowed { get; set; }
    }
}
