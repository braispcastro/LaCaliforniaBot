using System;
using System.IO;
using System.Media;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Google.Cloud.TextToSpeech.V1;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace LaCaliforniaBot
{
    internal class Bot
    {
        private readonly TwitchClient client;
        private readonly TextToSpeechClient ttsClient;
        private readonly Dictionary<string, DateTime> usersDictionary;
        private readonly Dictionary<string, DateTime?> commandsDictionary;

        private bool playing = false;
        private bool ttsEnabled = true;
        private int messageDelay = 0;

        public Bot(string ttsCredentials)
        {
            messageDelay = Configuration.BasicConfiguration.MessageDelay;
            usersDictionary = new Dictionary<string, DateTime>();
            commandsDictionary = new Dictionary<string, DateTime?>
            {
                { Configuration.BasicConfiguration.SlowInfo, null }
            };

            ttsClient = new TextToSpeechClientBuilder { JsonCredentials = ttsCredentials }.Build();

            ConnectionCredentials credentials = new ConnectionCredentials(Configuration.BasicConfiguration.BotUsername, Configuration.BasicConfiguration.BotPassword);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, Configuration.BasicConfiguration.Channel);

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;

            if (Configuration.BasicConfiguration.VerboseLog)
                client.OnLog += Client_OnLog;
        }

        public void Connect()
        {
            if (client != null)
                client.Connect();
        }

        private void LogMessage(string message)
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

        private void WriteMessage(string message)
        {
            client.SendMessage(Configuration.BasicConfiguration.Channel, $"/me {message}");
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
                var ttsCommand = $"{Configuration.BasicConfiguration.Prefix}{Configuration.BasicConfiguration.TextToSpeech} ";
                var settingsCommand = $"{Configuration.BasicConfiguration.Prefix}{Configuration.BasicConfiguration.Settings} ";
                var slowCommand = $"{Configuration.BasicConfiguration.Prefix}{Configuration.BasicConfiguration.SlowInfo}";

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

                else if (!string.IsNullOrEmpty(Configuration.BasicConfiguration.SlowInfo) 
                    && message.ToLowerInvariant().StartsWith(slowCommand.ToLowerInvariant()) 
                    && messageDelay > 0 && CanNotifyCommand(Configuration.BasicConfiguration.SlowInfo))
                {
                    WriteMessage($"No podrás usar la !k de nuevo hasta que pasen {messageDelay} segundos. Los mensajes no se quedan en cola, así que no spamees!");
                }
            }
            catch (Exception ex)
            {
                if (Configuration.BasicConfiguration.VerboseLog)
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
            if (Configuration.BasicConfiguration.LogTTSMessage)
                LogMessage($"{username}: {msgToRead}");
            PlayMessage(msgToRead);
        }

        private void SettingsCommand(string settingsCommand, string message, string username)
        {
            var param = message.Substring(settingsCommand.Length);
            if (ttsEnabled && param.ToLowerInvariant() == Configuration.BasicConfiguration.DisableTTS)
            {
                ttsEnabled = false;
                LogMessage($"*** TTS desactivado por {username} ***");
            }
            else if (!ttsEnabled && param.ToLowerInvariant() == Configuration.BasicConfiguration.EnableTTS)
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
            bool isExcludedMod = Configuration.BasicConfiguration.ExcludedMods.Select(x => x.ToLowerInvariant()).Contains(chatMessage.Username.ToLowerInvariant());
            if (isExcludedMod && !ttsEnabled)
                return false;

            if (messageDelay <= 0 || (!isExcludedMod && (chatMessage.IsBroadcaster || chatMessage.IsModerator)))
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
                if (Configuration.BasicConfiguration.VerboseLog)
                    LogMessage($"{ex.Message}\n{ex.StackTrace}");
                else
                    LogMessage(ex.Message);
            }
        }
    }
}
