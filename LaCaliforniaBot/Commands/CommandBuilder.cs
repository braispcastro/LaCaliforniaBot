﻿using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;
using LaCaliforniaBot.Model;

namespace LaCaliforniaBot.Commands
{
    public class CommandBuilder
    {
        private static CommandBuilder instance;
        public static CommandBuilder Instance
        {
            get { return instance ?? (instance = new CommandBuilder()); }
        }

        public List<Command> Items { get; }

        public CommandBuilder()
        {
            var methods = Assembly.Load("LaCaliforniaBot").GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToList();

            Items = BuildCommandList(methods).ToList();
        }

        private IEnumerable<Command> BuildCommandList(List<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var constructorArgs = method.CustomAttributes.Single(x => x.AttributeType == typeof(CommandAttribute)).ConstructorArguments;
                var alias = constructorArgs[0].Value?.ToString();
                var allow = (ChatUserType)constructorArgs[1].Value;
                var name = constructorArgs[2].Value?.ToString();
                var desc = constructorArgs[3].Value?.ToString();
                var inst = method.ReflectedType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(typeof(void), null);
                yield return new Command(alias, name, desc, allow, method, inst);
            }
        }
    }
}
