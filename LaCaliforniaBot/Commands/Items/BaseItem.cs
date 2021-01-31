using TwitchLib.Client.Models;

namespace LaCaliforniaBot.Commands.Items
{
    public class BaseItem
    {
        internal ChatCommand ParseArgument(object[] args)
        {
            return args[0] as ChatCommand;
        }
    }
}
