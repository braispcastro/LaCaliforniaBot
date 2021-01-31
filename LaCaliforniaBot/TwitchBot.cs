using System;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using LaCaliforniaBot.Commands;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot
{
    public class TwitchBot
    {
        private static TwitchBot instance;
        public static TwitchBot Instance
        {
            get { return instance ?? (instance = new TwitchBot()); }
        }

        public TwitchClient Client { get; }

        public TwitchBot()
        {
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

        public void LogMessage(string message)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now}] {message}");
            }
            catch (Exception)
            {
                // Leave empty
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
                LogMessage(ex.Message);
            }
        }

        #endregion

        #region EventHandlers

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            LogMessage($"Conectado a #{e.AutoJoinChannel}");
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var command = CommandBuilder.Instance.Items
                .FirstOrDefault(x => x.Alias.ToLowerInvariant() == e.Command.CommandText.ToLowerInvariant());
            if (command == null)
                return;

            if (!CheckUserFlags(command.Allow, e.Command.ChatMessage))
                return;

            object[] args = new object[] { new object[] { e.Command } };
            command.MethodInfo.Invoke(command.Instance, args);
        }

        #endregion

        #region Private Methods

        private bool CheckUserFlags(ChatUserType allow, ChatMessage chatMessage)
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

            return false;
        }

        #endregion
    }
}
