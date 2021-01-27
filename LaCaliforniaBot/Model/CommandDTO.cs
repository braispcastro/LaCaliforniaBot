using LaCaliforniaBot.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace LaCaliforniaBot.Model
{
    public class CommandDTO
    {
        public string Alias { get; }
        public string Name { get; }
        public string Description { get; }
        public ChatUserType Allow { get; }
        public MethodInfo MethodInfo { get; }

        public CommandDTO(string alias, string name, string description, ChatUserType allow, MethodInfo methodInfo)
        {
            Alias = alias;
            Name = name;
            Description = description;
            Allow = allow;
            MethodInfo = methodInfo;
        }
    }
}
