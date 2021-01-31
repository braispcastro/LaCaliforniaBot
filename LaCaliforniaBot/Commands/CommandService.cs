using TwitchLib.Client.Models;

namespace LaCaliforniaBot.Commands
{
    public class CommandService
    {
        private static CommandService instance;
        public static CommandService Instance
        {
            get { return instance ?? (instance = new CommandService()); }
        }

        public void ParseCommand(ChatCommand chatCommand)
        {

        }
    }
}
