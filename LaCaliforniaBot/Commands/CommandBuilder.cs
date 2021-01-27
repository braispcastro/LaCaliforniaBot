using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;
using LaCaliforniaBot.Model;

namespace LaCaliforniaBot.Commands
{
    public class CommandBuilder
    {
        public List<CommandDTO> CommandList { get; }

        public CommandBuilder()
        {
            var methods = Assembly.Load("LaCaliforniaBot").GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToList();

            CommandList = BuildCommandList(methods).ToList();
        }

        private IEnumerable<CommandDTO> BuildCommandList(List<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var constructorArgs = method.CustomAttributes.Single(x => x.AttributeType == typeof(CommandAttribute)).ConstructorArguments;
                var alias = constructorArgs[0].Value?.ToString();
                var allow = (ChatUserType)constructorArgs[1].Value;
                var desc = constructorArgs[2].Value?.ToString();
                var name = constructorArgs[3].Value?.ToString();
                yield return new CommandDTO(alias, name, desc, allow, method);
            }
        }
    }
}
