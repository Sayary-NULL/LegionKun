﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace LegionKun.Attribute
{
    class OwnerOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext Context, CommandInfo command, IServiceProvider services)
        {
            if(Context.User.Id == Module.ConstVariables.DateBase.OwnerID)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("Эта команда только для Sayary!"));
        }
    }
}
