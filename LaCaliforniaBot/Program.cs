using System;
using System.IO;
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

            Bot bot = new Bot(jsonCredentials, config);
            bot.Connect();

            string param = string.Empty;
            while (param.ToLowerInvariant() != "exit")
                param = Console.ReadLine();
        }
    }
}
