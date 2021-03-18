using System;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using LaCaliforniaBot.Commands;
using LaCaliforniaBot.Enums;
using System.Collections.Generic;
using LaCaliforniaBot.Model;
using Google.Cloud.Logging.Type;

namespace LaCaliforniaBot
{
    public class TwitchBot
    {
        private static TwitchBot instance;
        public static TwitchBot Instance
        {
            get { return instance ?? (instance = new TwitchBot()); }
        }

        private readonly List<AllowedUser> allowedUsersToTalk;
        public List<AllowedUser> AllowedUsersToTalk
        {
            get { return allowedUsersToTalk; }
        }

        public TwitchClient Client { get; }

        public TwitchBot()
        {
            allowedUsersToTalk = new List<AllowedUser>();

            ConnectionCredentials credentials = new ConnectionCredentials(Configuration.BasicConfiguration.BotUsername, Configuration.BasicConfiguration.BotPassword);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            Client = new TwitchClient(customClient);
            Client.Initialize(credentials, 
                channel: Configuration.BasicConfiguration.Channel, 
                chatCommandIdentifier: Configuration.BasicConfiguration.Prefix);

            Client.OnConnected += Client_OnConnected;
            Client.OnChatCommandReceived += Client_OnChatCommandReceived;
        }

        #region Public Methods

        public bool Connect()
        {
            try
            {
                if (Client == null)
                {
                    return false;
                }

                if (Client.IsConnected)
                {
                    return true;
                }
                else
                {
                    Client.Connect();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void LogMessage(LogSeverity logSeverity, string message, string description = null)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now}] {message}");
                if (logSeverity != LogSeverity.Default && logSeverity != LogSeverity.Debug)
                {
                    LoggingCloud.Instance.WriteLog(logSeverity, message, description);
                }
            }
            catch (Exception ex)
            {
                LoggingCloud.Instance.WriteLog(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                Client.SendMessage(Configuration.BasicConfiguration.Channel, $"/me {message}");
            }
            catch (Exception ex)
            {
                LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        public bool IsPlebAllowedToTalk(string username)
        {
            var allowedUser = allowedUsersToTalk.FirstOrDefault(x => x.Username == username.ToLowerInvariant());
            if (allowedUser == null)
                return false;

            if ((DateTime.UtcNow - allowedUser.AllowedAt).TotalMinutes < allowedUser.MinutesAllowed)
            {
                return true;
            }
            else
            {
                allowedUsersToTalk.RemoveAll(x => x.Username == username.ToLowerInvariant());
                return false;
            }
        }

        #endregion

        #region EventHandlers

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            LogMessage(LogSeverity.Notice, $"Conectado a #{Configuration.BasicConfiguration.Channel}");
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var command = CommandBuilder.Instance.Items
                .FirstOrDefault(x => x.Alias.ToLowerInvariant() == e.Command.CommandText.ToLowerInvariant());
            if (command == null)
                return;

            if (!CanUseCommand(command.Name, command.Allow, e.Command.ChatMessage))
                return;

            object[] args = new object[] { new object[] { e.Command } };
            command.MethodInfo.Invoke(command.Instance, args);
        }

        #endregion

        #region Private Methods

        private bool CanUseCommand(string name, ChatUserType allow, ChatMessage chatMessage)
        {
            if (allow.HasFlag(ChatUserType.Pleb))
                return true;

            if (chatMessage.IsSubscriber && allow.HasFlag(ChatUserType.Subscriber))
                return true;

            if (chatMessage.IsVip && allow.HasFlag(ChatUserType.Vip))
                return true;

            if (chatMessage.IsModerator && allow.HasFlag(ChatUserType.Moderator))
                return true;

            if (chatMessage.IsBroadcaster && allow.HasFlag(ChatUserType.Broadcaster))
                return true;

            return CheckCustomPermits(name, chatMessage);
        }

        private bool CheckCustomPermits(string name, ChatMessage chatMessage)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            switch (name)
            {
                case "California":
                    return IsPlebAllowedToTalk(chatMessage.Username);
                default:
                    return false;
            }
        }

        #endregion
    }
}
