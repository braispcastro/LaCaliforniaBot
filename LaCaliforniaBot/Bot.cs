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
using System.Collections.Generic;
using System.Threading;

namespace LaCaliforniaBot
{
    internal class Bot
    {
        private readonly ConfigDTO config;
        private readonly TwitchClient client;
        private readonly TextToSpeechClient ttsClient;
        private readonly Dictionary<string, DateTime> usersDictionary;
        private readonly Dictionary<string, DateTime?> commandsDictionary;

        private bool playing = false;
        private bool ttsEnabled = true;
        private int messageDelay = 0;

        public Bot(string ttsCredentials, ConfigDTO config)
        {
            this.config = config;

            messageDelay = config.MessageDelay;
            usersDictionary = new Dictionary<string, DateTime>();
            commandsDictionary = new Dictionary<string, DateTime?>
            {
                { config.SlowInfo, null }
            };

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

        private void WriteMessage(string message)
        {
            client.SendMessage(config.Channel, $"/me {message}");
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
                var slowCommand = $"{config.Prefix}{config.SlowInfo}";

                string message = e.ChatMessage.Message.Trim();
                bool canUseSettings = e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator;
                bool canUseTTS = canUseSettings || (ttsEnabled && (e.ChatMessage.IsSubscriber || e.ChatMessage.IsVip));


                if (message.ToLowerInvariant().StartsWith(ttsCommand.ToLowerInvariant()) 
                    && canUseTTS && IsAllowedToSpeak(e.ChatMessage))
                {
                    CaliforniaCommand(ttsCommand, message, e.ChatMessage.Username);
                }

                else if (message.ToLowerInvariant().StartsWith(settingsCommand.ToLowerInvariant()) 
                    && canUseSettings)
                {
                    SettingsCommand(settingsCommand, message, e.ChatMessage.Username);
                }

                else if (!string.IsNullOrEmpty(config.SlowInfo) 
                    && message.ToLowerInvariant().StartsWith(slowCommand.ToLowerInvariant()) 
                    && messageDelay > 0 && CanNotifyCommand(config.SlowInfo))
                {
                    WriteMessage($"No podrás usar la !k de nuevo hasta que pasen {messageDelay} segundos. Los mensajes no se quedan en cola, así que no spamees!");
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

        private bool CanNotifyCommand(string command)
        {
            bool result = commandsDictionary.ContainsKey(command) && (!commandsDictionary[command].HasValue
                || (commandsDictionary[command].HasValue && (DateTime.UtcNow - commandsDictionary[command].Value).TotalSeconds > 30));

            if (result)
                commandsDictionary[command] = DateTime.UtcNow;

            return result;
        }

        private void CaliforniaCommand(string ttsCommand, string message, string username)
        {
            while (playing)
            {
                Thread.Sleep(100);
            }
            var msgToRead = message.Substring(ttsCommand.Length);
            if (config.LogTTSMessage)
                LogMessage($"{username}: {msgToRead}");
            PlayMessage(msgToRead);
        }

        private void SettingsCommand(string settingsCommand, string message, string username)
        {
            var param = message.Substring(settingsCommand.Length);
            if (ttsEnabled && param.ToLowerInvariant() == config.DisableTTS)
            {
                ttsEnabled = false;
                LogMessage($"*** TTS desactivado por {username} ***");
            }
            else if (!ttsEnabled && param.ToLowerInvariant() == config.EnableTTS)
            {
                ttsEnabled = true;
                LogMessage($"*** TTS activado por {username} ***");
            }
            else if (int.TryParse(param, out messageDelay))
            {
                usersDictionary.Clear();
                LogMessage($"*** Slow mode de {messageDelay} segundos establecido por {username} ***");
            }
        }

        private bool IsAllowedToSpeak(ChatMessage chatMessage)
        {
            if (messageDelay <= 0 || chatMessage.IsBroadcaster || chatMessage.IsModerator)
                return true;

            if (usersDictionary.TryGetValue(chatMessage.Username, out DateTime lastMessage))
            {
                if ((DateTime.UtcNow - lastMessage).TotalSeconds >= messageDelay)
                {
                    usersDictionary[chatMessage.Username] = DateTime.UtcNow;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                usersDictionary.Add(chatMessage.Username, DateTime.UtcNow);
                return true;
            }
        }

        private void PlayMessage(string message)
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
                    Thread.Sleep(500);
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
