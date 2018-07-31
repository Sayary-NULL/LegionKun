using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Addons.MpGame;
using Discord.Commands;

namespace LegionKun.Game.CrossZero
{
    public class CrossZeroBase : ModuleBase<SocketCommandContext>
    {
        public static Dictionary<IMessageChannel, DataType> DataDictionary { get; }
            = new Dictionary<IMessageChannel, DataType>(DiscordComparers.ChannelComparer);
    }

    public class DataType
    {
        public static int Game = 1;
        public int GameId = 0;
        public IChannel Channelsgame;
        public IGuild Guild;
        public IUser User1;
        public IUser User2;
        public string[,] field3X3 = new string[,] { {"", "", "" },
                                                    {"", "", "" },
                                                    {"", "", "" }};
        public int ScoreUSer1 = 0;
        public int ScoreUser2 = 0;
    }
}
