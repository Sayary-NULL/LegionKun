using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using Discord.Addons.Interactive;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.IO;
using System.Collections;
using SixLabors.ImageSharp.Formats.Png;

namespace LegionKun.Game.CrossZero
{
    public class CrossZeroBase : InteractiveBase
    {
        public static DataBase<IMessageChannel, DataType> DataGameBase { get; private set; } = new DataBase<IMessageChannel, DataType>();

        public static MemoryStream ToStream(Image<Rgba32> img)
        {
            var imageStream = new MemoryStream();
            img.SaveAsPng(imageStream, new PngEncoder() { CompressionLevel = 9 });
            imageStream.Position = 0;
            return imageStream;
        }
    }

    public class DataBase<TValue, TKey> : IEnumerable
    {
        public static Dictionary<TValue, TKey> Data;

        public DataBase()
        {
            Data = new Dictionary<TValue, TKey>();
        }

        public DataBase(int Long)
        {
            Data = new Dictionary<TValue, TKey>(Long);
        }

        public void Add(TValue value, TKey key)
        {
            Data.Add(value, key);
        }

        public bool ContainsKey(TValue value)
        {
            return Data.ContainsKey(value);
        }

        public bool ContainsValue(TKey key)
        {
            return Data.ContainsValue(key);
        }

        public bool Remove(TValue value)
        {
            return Data.Remove(value);
        }

        public bool Reset(TValue value, TKey key)
        {
            if (Data.ContainsKey(value))
            {
                if (Data.Remove(value))
                {
                    Data.Add(value, key);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public TKey this[TValue tvalue]
        {
            get
            {
                return Data[tvalue];
            }

            set
            {
                Data[tvalue] = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
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
        public string GUser1 { get; private set; } = "X";
        public string GUser2 { get; private set; } = "O";
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
