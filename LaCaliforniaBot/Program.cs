using System;
using System.IO;
using System.Reflection;
using LaCaliforniaBot.Model;
using Newtonsoft.Json;

namespace LaCaliforniaBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var jsonCredentials = File.ReadAllText("credentials.json");
            var json = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<ConfigDTO>(json);

            Console.Title = $"#{config.Channel} | LaCaliforniaBot " +
                $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}" +
                $".{Assembly.GetExecutingAssembly().GetName().Version.Minor}" +
                $".{Assembly.GetExecutingAssembly().GetName().Version.Build}";

            Bot bot = new Bot(jsonCredentials, config);
            bot.Connect();

            string param = string.Empty;
            while (param.ToLowerInvariant() != "exit")
                param = Console.ReadLine();
        }
    }
}
