﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Google.Cloud.Logging.Type;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;
using LaCaliforniaBot.Extensions;
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

        [Command("k", ChatUserType.Broadcaster | ChatUserType.Moderator | ChatUserType.Subscriber | ChatUserType.Vip, name: "California")]
        public void ParseMessageToPlay(object[] args)
        {
            try
            {
                var cmd = ParseArgument(args);

                if (ShouldIgnoreMessage(cmd.ArgumentsAsString))
                {
                    TwitchBot.Instance.LogMessage(LogSeverity.Info, "Se ha ignorado un mensaje por contener una URL", cmd.ArgumentsAsString);
                    return;
                }

                var message = FilterMessage(cmd);

                // Si el TTS está activado y no hay slowmode, se lee el mensaje
                if (Configuration.TextToSpeechEnabled && Configuration.TextToSpeechDelay <= 0)
                    PlayMessage(message);

                // Si el TTS está desactivado o hay delay, me lo salto para el streamer o los mods no excluidos
                else if (cmd.ChatMessage.IsBroadcaster || (cmd.ChatMessage.IsModerator && !IsExcludedMod(cmd.ChatMessage.Username)))
                    PlayMessage(message);

                // Si es un usuario con permiso específico, se lee el mensaje
                else if (TwitchBot.Instance.IsPlebAllowedToTalk(cmd.ChatMessage.Username))
                    PlayMessage(message);

                // Si el TTS está desactivado aquí no hago nada más
                else if (!Configuration.TextToSpeechEnabled)
                    return;

                // Si llego aquí, el TTS está activado pero está puesto el slowmode
                else if (IsUserIsAllowed(cmd))
                    PlayMessage(message);
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
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
            TextToSpeechCloud.Instance.PlayAudio(message);
        }

        private bool IsExcludedMod(string username)
        {
            if (Configuration.BasicConfiguration.ExcludedMods == null || Configuration.BasicConfiguration.ExcludedMods.Count < 1)
                return false;

            return Configuration.BasicConfiguration.ExcludedMods.Select(x => x.ToLowerInvariant()).Contains(username.ToLowerInvariant());
        }

        private bool IsUserIsAllowed(ChatCommand cmd)
        {
            var username = cmd.ChatMessage.Username.ToLowerInvariant();

            if (usersDictionary.TryGetValue(username, out DateTime lastMessage))
            {
                if ((DateTime.UtcNow - lastMessage).TotalSeconds >= Configuration.TextToSpeechDelay)
                {
                    usersDictionary[username] = DateTime.UtcNow;
                    return true;
                }
            }
            else
            {
                usersDictionary.Add(username, DateTime.UtcNow);
                return true;
            }

            return false;
        }

        private bool ShouldIgnoreMessage(string msg)
        {
            // Por ahora solo se comprueba si el mensaje contiene un link
            return Regex.IsMatch(msg.ToLowerInvariant(), @"(http[^\s]+)|(www\.[^\s]+)");
        }

        private string FilterMessage(ChatCommand cmd)
        {
            var result = cmd.ArgumentsAsString;

            return result
                .RemoveEmojis()
                .LimitWords(Configuration.TextToSpeechMaxCharacters);
        }

        #endregion
    }
}
