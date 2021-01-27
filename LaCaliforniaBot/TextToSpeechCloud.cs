using Google.Cloud.TextToSpeech.V1;
using System;
using System.IO;

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
            var ttsCredentials = File.ReadAllText("files/credentials.json");
            Client = new TextToSpeechClientBuilder { JsonCredentials = ttsCredentials }.Build();
        }

        #region Public Methods

        public Stream GetAudioStream(string message)
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
                AudioEncoding = AudioEncoding.Linear16
            };

            try
            {
                SynthesizeSpeechResponse response = Client.SynthesizeSpeech(input, voiceSelection, audioConfig);
                return new MemoryStream(response.AudioContent.ToByteArray());
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(ex.Message);
                return null;
            }
        }

        #endregion
    }
}
