using LaCaliforniaBot.Enums;
using LaCaliforniaBot.Model;
using System.Reflection;

namespace LaCaliforniaBot
{
    public class Configuration
    {
        public static ConfigDTO BasicConfiguration { get; set; }

        public static bool TextToSpeechEnabled { get; set; }

        public static int TextToSpeechDelay { get; set; }

        public static string AppVersion
        {
            get { return $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}"; }
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
