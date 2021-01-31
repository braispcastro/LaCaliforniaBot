using LaCaliforniaBot.Commands;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

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

        #region Private Methods

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            LogMessage($"Conectado a #{e.AutoJoinChannel}");
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            CommandService.Instance.ParseCommand(e.Command);
        }

        #endregion
    }
}
