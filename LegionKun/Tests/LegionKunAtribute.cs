using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionKun.Tests
{
    internal class LegionKunAtribute : System.Attribute
    {
        public string Name;

        public bool Access = false;

        public LegionKunAtribute(string name)
        {
            Name = name;
        }

        public LegionKunAtribute(SocketGuildUser user, Module.ConstVariables.CDiscord guild)
        {

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    Access = true;
                    break;
                }
        }
    }
}
