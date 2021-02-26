using System;
using System.IO;
using LaCaliforniaBot.Model;
using Newtonsoft.Json;

namespace LaCaliforniaBot
{
    internal class Program
    {
        static void Main()
        {
            // Configuration
            var json = File.ReadAllText("config.json");
            Configuration.BasicConfiguration = JsonConvert.DeserializeObject<Config>(json);
            Configuration.TextToSpeechEnabled = true;
            Configuration.TextToSpeechDelay = 0;
            Configuration.TextToSpeechMaxCharacters = 100;

            // Windows title
            Console.Title = $"#{Configuration.BasicConfiguration.Channel} | LaCaliforniaBot v{Configuration.AppVersion}";

            // Connect twitch bot
            TwitchBot.Instance.Connect();

            // Hold till exit
            string param = string.Empty;
            while (param.ToLowerInvariant() != "exit")
                param = Console.ReadLine();
        }
    }
}
