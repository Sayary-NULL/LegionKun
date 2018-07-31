using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace LegionKun.Game.CrossZero
{
    public class CrossZeroGame : CrossZeroBase
    {

        internal Embed StatusGame(ISocketMessageChannel channel)
        {
            if (DataDictionary.ContainsKey(channel))
            {
                DataType DBase = DataDictionary[channel];
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Game status");

                return builder.Build();
            }
            else return null;
        }
    }
}

