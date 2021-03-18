using LaCaliforniaBot.Enums;
using System.Reflection;

namespace LaCaliforniaBot.Model
{
    public class Command
    {
        public string Alias { get; }
        public string Name { get; }
        public string Description { get; }
        public ChatUserType Allow { get; }
        public MethodInfo MethodInfo { get; }
        public object Instance { get; }

        public Command(string alias, string name, string description, ChatUserType allow, MethodInfo methodInfo, object instance)
        {
            Alias = alias;
            Name = name;
            Description = description;
            Allow = allow;
            MethodInfo = methodInfo;
            Instance = instance;
        }
    }
}
