using LaCaliforniaBot.Enums;
using System;
using System.Collections.Generic;

namespace LaCaliforniaBot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        public string Alias { get; }
        public string Name { get; }
        public string Description { get; }
        public ChatUserType Allow { get; }

        public CommandAttribute(string alias, ChatUserType allow, string name = null, string description = null)
        {
            Alias = alias;
            Allow = allow;
            Name = name;
            Description = description;
        }
    }
}
