using System;
using System.IO;
using System.Media;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Google.Cloud.TextToSpeech.V1;
using LaCaliforniaBot.Model;
using System.Threading.Tasks;

namespace LaCaliforniaBot
{
    internal class Bot
    {
        private readonly ConfigDTO config;
        private readonly TwitchClient client;
        private readonly TextToSpeechClient ttsClient;

        private bool playing = false;
        private bool ttsEnabled = true;

        public Bot(string ttsCredentials, ConfigDTO config)
        {
            this.config = config;

            ttsClient = new TextToSpeechClientBuilder { JsonCredentials = ttsCredentials }.Build();

            ConnectionCredentials credentials = new ConnectionCredentials(config.BotUsername, config.BotPassword);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, config.Channel);

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;

            if (config.VerboseLog)
                client.OnLog += Client_OnLog;
        }

        public void Connect()
        {
            if (client != null)
                client.Connect();
        }

        private void LogMessage(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] {message}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            LogMessage(e.Data);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            LogMessage($"Conectado a #{e.AutoJoinChannel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            try
            {
                var ttsCommand = $"{config.Prefix}{config.TextToSpeech} ";
                var settingsCommand = $"{config.Prefix}{config.Settings} ";

                string message = e.ChatMessage.Message.Trim();
                bool canUseSettings = e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator;
                bool canUseTTS = ttsEnabled && (e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator || e.ChatMessage.IsSubscriber || e.ChatMessage.IsVip);

                if (canUseTTS && message.StartsWith(ttsCommand))
                {
                    while (playing)
                    {
                        Task.Delay(500);
                    }
                    var msgToRead = message.Substring(ttsCommand.Length);
                    LogMessage($"{e.ChatMessage.Username}: {msgToRead}");
                    PlayMessage(msgToRead);
                }
                else if (canUseSettings && message.StartsWith(settingsCommand))
                {
                    var param = message.Substring(settingsCommand.Length);
                    if (param.ToLowerInvariant() == "focus")
                    {
                        ttsEnabled = false;
                        LogMessage($"*** TTS desactivado por {e.ChatMessage.Username} ***");
                    }
                    else if (param.ToLowerInvariant() == "resume")
                    {
                        ttsEnabled = true;
                        LogMessage($"*** TTS activado por {e.ChatMessage.Username} ***");
                    }
                }
            }
            catch (Exception ex)
            {
                if (config.VerboseLog)
                    LogMessage($"{ex.Message}\n{ex.StackTrace}");
                else
                    LogMessage(ex.Message);
            }
        }

        private async void PlayMessage(string message)
        {
            playing = true;

            SynthesisInput input = new SynthesisInput
            {
                Text = message
            };

            // You can specify a particular voice, or ask the server to pick based
            // on specified criteria.
            VoiceSelectionParams voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "es-ES",
                Name = "es-ES-Standard-A"
            };

            // The audio configuration determines the output format and speaking rate.
            AudioConfig audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Linear16
            };

            try
            {
                SynthesizeSpeechResponse response = ttsClient.SynthesizeSpeech(input, voiceSelection, audioConfig);
                using (Stream output = new MemoryStream(response.AudioContent.ToByteArray()))
                {
                    SoundPlayer soundPlayer = new SoundPlayer(output);
                    soundPlayer.PlaySync();
                    await Task.Delay(500);
                    playing = false;
                }
            }
            catch (Exception ex)
            {
                if (config.VerboseLog)
                    LogMessage($"{ex.Message}\n{ex.StackTrace}");
                else
                    LogMessage(ex.Message);
            }
        }
    }
}
