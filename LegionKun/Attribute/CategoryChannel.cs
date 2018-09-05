using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LegionKun.Module;

namespace LegionKun.Attribute
{
    class CategoryChannel : PreconditionAttribute
    {
        bool IsDefault = false;

        bool IsNews = false;

        bool IsCommand = false;

        public CategoryChannel(bool ID = false, bool IN = false, bool IC = false)
        {
            IsDefault = ID;
            IsNews = IN;
            IsCommand = IC;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext Context, CommandInfo command, IServiceProvider services)
        {
            if (!ConstVariables.CServer.ContainsKey(Context.Guild.Id))
                return Task.FromResult(PreconditionResult.FromError("Не найдена гильдия"));

            if (ConstVariables.CServer[Context.Guild.Id].IsEntryOrСategoryChannel(Context.Channel.Id, IsDefault, IsNews, IsCommand))
            {
                Console.WriteLine("start");
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError($"Не найден или отсутствует данная категория"));
        }
    }
}
