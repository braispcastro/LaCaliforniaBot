using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace LaCaliforniaBot.Commands.Items
{
    public class BaseItem
    {
        internal ChatCommand ParseArgument(object[] args)
        {
            return args[0] as ChatCommand;
        }

        internal string GetFirstParameter(List<string> argumentsAsList)
        {
            if (argumentsAsList == null || argumentsAsList.Count < 1)
                return null;

            return argumentsAsList.First();
        }
    }
}
