using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionKun.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class TestsAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (Module.ConstVariables.ThisTest)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("Выполняется только во время тестов!"));
            }
        }
    }
}
