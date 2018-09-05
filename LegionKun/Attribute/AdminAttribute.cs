using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace LegionKun.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext Context, CommandInfo command, IServiceProvider services)
        {
            var Guild = Module.ConstVariables.CServer[Context.Guild.Id];

            var User = Guild.GetGuild().GetUser(Context.User.Id);

            foreach (var role in User.Roles)
                if (Guild.EntryRole(role.Id))
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }

            return Task.FromResult(PreconditionResult.FromError("Нет доступа"));
        }
    }
}
