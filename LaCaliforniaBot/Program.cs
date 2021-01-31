using System;
using System.IO;
using System.Reflection;
using LaCaliforniaBot.Commands;
using LaCaliforniaBot.Model;
using Newtonsoft.Json;

namespace LaCaliforniaBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configuration
            var json = File.ReadAllText("files/config.json");
            Configuration.BasicConfiguration = JsonConvert.DeserializeObject<ConfigDTO>(json);
            Configuration.TextToSpeechEnabled = true;
            Configuration.TextToSpeechDelay = 0;

            // Windows title
            Console.Title = $"#{Configuration.BasicConfiguration.Channel} | LaCaliforniaBot " +
                $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}" +
                $".{Assembly.GetExecutingAssembly().GetName().Version.Minor}";

            // Connect twitch bot
            TwitchBot.Instance.Connect();

            // Hold till exit
            string param = string.Empty;
            while (param.ToLowerInvariant() != "exit")
                param = Console.ReadLine();
        }
    }
}
