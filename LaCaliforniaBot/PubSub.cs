using Google.Cloud.Logging.Type;
using System;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace LaCaliforniaBot
{
    public class PubSub
    {
        private static PubSub instance;
        public static PubSub Instance
        {
            get { return instance ?? (instance = new PubSub()); }
        }

        public TwitchPubSub Client { get; }

        public PubSub()
        {
            Client = new TwitchPubSub();

            Client.OnPubSubServiceConnected += Client_OnPubSubServiceConnected;
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

                Client.Connect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Private Methods

        private void Client_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            TwitchBot.Instance.LogMessage(LogSeverity.Notice, $"Servicio conectado - PubSub");
        }

        #endregion
    }
}
