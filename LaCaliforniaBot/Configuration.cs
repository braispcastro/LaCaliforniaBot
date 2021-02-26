using LaCaliforniaBot.Enums;
using LaCaliforniaBot.Model;
using System.Reflection;

namespace LaCaliforniaBot
{
    public class Configuration
    {
        public static Config BasicConfiguration { get; set; }

        public static bool TextToSpeechEnabled { get; set; }

        public static int TextToSpeechDelay { get; set; }

        public static int TextToSpeechMaxCharacters { get; set; }

        public static string AppVersion
        {
            get
            {
                var version = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}";
                if (Assembly.GetExecutingAssembly().GetName().Version.Build != 0)
                    version = string.Concat(version, $".{Assembly.GetExecutingAssembly().GetName().Version.Build}");
                return version;
            }
        }

        public static EnvironmentType Environment
        {
            get
            {
                #if DEBUG
                return EnvironmentType.Development;
                #elif RELEASE
                return EnvironmentType.Production;
                #endif
            }
        }
    }
}
