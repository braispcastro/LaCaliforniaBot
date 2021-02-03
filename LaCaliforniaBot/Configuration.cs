using LaCaliforniaBot.Model;

namespace LaCaliforniaBot
{
    public class Configuration
    {
        public static ConfigDTO BasicConfiguration { get; set; }

        public static bool TextToSpeechEnabled { get; set; }

        public static int TextToSpeechDelay { get; set; }
    }
}
