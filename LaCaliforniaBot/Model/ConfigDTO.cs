namespace LaCaliforniaBot.Model
{
    internal class ConfigDTO
    {
        public string Channel { get; set; }
        public char Prefix { get; set; }
        public string TextToSpeech { get; set; }
        public string Settings { get; set; }
        public string EnableTTS { get; set; }
        public string DisableTTS { get; set; }
        public bool VerboseLog { get; set; }
        public string BotUsername { get; set; }
        public string BotPassword { get; set; }
    }
}
