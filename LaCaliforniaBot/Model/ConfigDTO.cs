using System.Collections.Generic;

namespace LaCaliforniaBot.Model
{
    public class ConfigDTO
    {
        public string Channel { get; set; }
        public char Prefix { get; set; }
        public string TextToSpeech { get; set; }
        public string Settings { get; set; }
        public string EnableTTS { get; set; }
        public string DisableTTS { get; set; }
        public string SlowInfo { get; set; }
        public int MessageDelay { get; set; }
        public bool LogTTSMessage { get; set; }
        public bool VerboseLog { get; set; }
        public string BotUsername { get; set; }
        public string BotPassword { get; set; }
        public List<string> ExcludedMods { get; set; }
    }
}
