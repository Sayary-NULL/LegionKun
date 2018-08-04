using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.IO;

namespace LegionKun.Game.CrossZero
{
    public class CrossZeroBase : ModuleBase<SocketCommandContext>
    {
        public static Dictionary<IMessageChannel, DataType> DataDictionary { get; private set; }
            = new Dictionary<IMessageChannel, DataType>(DiscordComparers.ChannelComparer);
    }

    public enum StatGame
    {
        Create,
        Close,
        Stop,
        Start,
        Win
    }

    public class DataType
    {
        public RestUserMessage Message;
        public IChannel Channelsgame;
        public IGuild Guild;
        public IUser User1;
        public IUser User2;
        public IUser GoUser;
        public int ScoreUser1 = 0;
        public int ScoreUser2 = 0;
        public string GUser1 = "X";
        public string GUser2 = "O";
        public int Sum = 0;
        public Image<Rgba32> IField { get; set; } = null;
        public Image<Rgba32> ICross { get; private set; } = null;
        public Image<Rgba32> IZero { get; private set; } = null;
        public SixLabors.ImageSharp.Processing.Drawing.Pens.Pen<Rgba32> pen = new SixLabors.ImageSharp.Processing.Drawing.Pens.Pen<Rgba32>(new Rgba32(255, 0, 0), 16);
        public StatGame GameStat = StatGame.Create; 
        public  string[,] field3X3 = new string[,] { {"", "", "" },
                                                    {"", "", "" },
                                                    {"", "", "" }};
        public void GetImage()
        {
            Bitmap bmp = new Bitmap(Module.ConstVariables.Filed);
            MemoryStream memory = new MemoryStream();
            bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            IField = SixLabors.ImageSharp.Image.Load(memory.ToArray());

            bmp = new Bitmap(Module.ConstVariables.Cross);
            memory = new MemoryStream();
            bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            ICross = SixLabors.ImageSharp.Image.Load(memory.ToArray());

            bmp = new Bitmap(Module.ConstVariables.Zero);
            memory = new MemoryStream();
            bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            IZero = SixLabors.ImageSharp.Image.Load(memory.ToArray());
        }
    }
}
