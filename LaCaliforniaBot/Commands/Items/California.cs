using System;
using System.Collections.Generic;
using System.Linq;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;
using TwitchLib.Client.Models;

namespace LaCaliforniaBot.Commands.Items
{
    public class California : BaseItem
    {
        private static California instance;
        public static California Instance
        {
            get { return instance ?? (instance = new California()); }
        }

        private readonly Dictionary<string, DateTime> usersDictionary;

        public California()
        {
            usersDictionary = new Dictionary<string, DateTime>();
        }

        [Command("k", ChatUserType.Broadcaster | ChatUserType.Moderator | ChatUserType.Subscriber | ChatUserType.Vip)]
        public void ParseMessageToPlay(object[] args)
        {
            try
            {
                var cmd = ParseArgument(args);

                // Si el TTS está activado y no hay slowmode, se lee el mensaje
                if (Configuration.TextToSpeechEnabled && Configuration.TextToSpeechDelay <= 0)
                    PlayMessage(cmd.ArgumentsAsString);

                // Si el TTS está desactivado o hay delay, me lo salto para el streamer o los mods no excluidos
                else if (cmd.ChatMessage.IsBroadcaster || (cmd.ChatMessage.IsModerator && !IsExcludedMod(cmd.ChatMessage.Username)))
                    PlayMessage(cmd.ArgumentsAsString);

                // Si el TTS está desactivado aquí no hago nada más
                else if (!Configuration.TextToSpeechEnabled)
                    return;

                // Si llego aquí, el TTS está activado pero está puesto el slowmode
                else
                    PlayIfUserIsAllowed(cmd);
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(ex.Message);
            }
        }

        #region PublicMethods

        public void ClearUserList()
        {
            usersDictionary.Clear();
        }

        #endregion

        #region Private Methods

        private void PlayMessage(string message)
        {
            TwitchBot.Instance.LogMessage(message);
            TextToSpeechCloud.Instance.PlayAudio(message);
        }

        private bool IsExcludedMod(string username)
        {
            if (Configuration.BasicConfiguration.ExcludedMods == null || Configuration.BasicConfiguration.ExcludedMods.Count < 1)
                return false;

            return Configuration.BasicConfiguration.ExcludedMods.Select(x => x.ToLowerInvariant()).Contains(username.ToLowerInvariant());
        }

        private void PlayIfUserIsAllowed(ChatCommand cmd)
        {
            var username = cmd.ChatMessage.Username.ToLowerInvariant();

            if (usersDictionary.TryGetValue(username, out DateTime lastMessage))
            {
                if ((DateTime.UtcNow - lastMessage).TotalSeconds >= Configuration.TextToSpeechDelay)
                {
                    usersDictionary[username] = DateTime.UtcNow;
                    PlayMessage(cmd.ArgumentsAsString);
                }
            }
            else
            {
                usersDictionary.Add(username, DateTime.UtcNow);
                PlayMessage(cmd.ArgumentsAsString);
            }
        }

        #endregion
    }
}
