using Google.Cloud.Logging.Type;
using Google.Cloud.TextToSpeech.V1;
using LaCaliforniaBot.Extensions;
using System;
using System.IO;
using System.Media;
using System.Threading;

namespace LaCaliforniaBot
{
    public class TextToSpeechCloud
    {
        private static TextToSpeechCloud instance;
        public static TextToSpeechCloud Instance
        {
            get { return instance ?? (instance = new TextToSpeechCloud()); }
        }

        public TextToSpeechClient Client { get; }

        public TextToSpeechCloud()
        {
            var ttsCredentials = File.ReadAllText("credentials.json");
            Client = new TextToSpeechClientBuilder { JsonCredentials = ttsCredentials }.Build();
        }

        #region Public Methods

        public void PlayAudio(string message)
        {
            var filteredMsg = FilterMessage(message);
            using (Stream output = GetAudioStream(filteredMsg))
            {
                SoundPlayer soundPlayer = new SoundPlayer(output);
                soundPlayer.PlaySync();
                Thread.Sleep(500);
            }
        }

        #endregion

        #region Private Methods

        private Stream GetAudioStream(string message)
        {
            SynthesisInput input = new SynthesisInput
            {
                Text = message
            };

            VoiceSelectionParams voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "es-ES",
                Name = "es-ES-Standard-A"
            };

            AudioConfig audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Linear16,
                SpeakingRate = Configuration.BasicConfiguration.MessageSpeed
            };

            try
            {
                SynthesizeSpeechResponse response = Client.SynthesizeSpeech(input, voiceSelection, audioConfig);
                return new MemoryStream(response.AudioContent.ToByteArray());
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
                return null;
            }
        }

        private string FilterMessage(string msg)
        {
            return msg
                .RemoveEmojis()
                .LimitWords(Configuration.TextToSpeechMaxCharacters);
        }

        #endregion
    }
}
