using System.Collections.Generic;

namespace LaCaliforniaBot.Model
{
    public class Config
    {
        public string Channel { get; set; }
        public char Prefix { get; set; }
        public double MessageSpeed { get; set; }
        public string BotUsername { get; set; }
        public string BotPassword { get; set; }
        public List<string> ExcludedMods { get; set; }
    }
}
