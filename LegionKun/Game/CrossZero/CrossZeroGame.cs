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
                builder.WithTitle("Игровая статистика");
                builder.AddField("/", "Игроки",true).AddField("Игрок1", DBase.User1.Mention, true).AddField("Игрок2", DBase.User2.Mention, true);
                builder.AddField("----------", "Победы", true).AddField("--", DBase.ScoreUSer1, true).AddField("--", DBase.ScoreUser2, true);
                builder.AddField("----------", "Информация", true).AddField("Channel", $"<#{DBase.Channelsgame.Id}>", true).AddField("Игр проведено", DBase.ScoreUSer1 + DBase.ScoreUser2, true);
                
                return builder.Build();
            }
            else return null;
        }
    }
}

