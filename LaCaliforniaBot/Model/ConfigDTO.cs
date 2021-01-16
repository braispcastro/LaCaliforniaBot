namespace LaCaliforniaBot.Model
{
    internal class ConfigDTO
    {
        public char Prefix { get; set; }
        public string TextToSpeech { get; set; }
        public string Settings { get; set; }
        public bool VerboseLog { get; set; }
        public string BotUsername { get; set; }
        public string BotPassword { get; set; }
    }
}
