using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.ImageSharp.Processing.Drawing;
using Discord.Commands;

namespace LegionKun.Game.CrossZero
{
    public class CrossZeroGame : CrossZeroBase
    {
        internal async Task<bool> ReplycGameAsync(SocketCommandContext Context)
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                await DataDictionary[Context.Channel].Message.DeleteAsync();

                DataDictionary[Context.Channel].GetImage();

                DataDictionary[Context.Channel].field3X3 = new string[,] { {"", "", "" },
                                                                           {"", "", "" },
                                                                           {"", "", "" }};

                if (Context.User.Id == DataDictionary[Context.Channel].User1.Id)
                {
                    DataDictionary[Context.Channel].GoUser = DataDictionary[Context.Channel].User2;
                    DataDictionary[Context.Channel].ScoreUser1++;
                }
                else
                {
                    DataDictionary[Context.Channel].GoUser = DataDictionary[Context.Channel].User1;
                    DataDictionary[Context.Channel].ScoreUser2++;
                }

                await ReplyAndDeleteAsync("Пересоздано", timeout: TimeSpan.FromSeconds(5));

                DataDictionary[Context.Channel].Message = await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(DataDictionary[Context.Channel].IField), "Filledfield.jpg");

                return true;
            }
            else return false;
        }
         
        internal Embed StatusGame(ISocketMessageChannel channel)
        {
            if (DataDictionary.ContainsKey(channel))
            {
                DataType DBase = DataDictionary[channel];
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Игровая статистика");
                builder.AddField("/", "Игроки",true).AddField("Игрок1", DBase.User1.Mention, true).AddField("Игрок2", DBase.User2.Mention, true);
                builder.AddField("----------", "Победы", true).AddField("--", DBase.ScoreUser1, true).AddField("--", DBase.ScoreUser2, true);
                builder.AddField("----------", "Информация", true).AddField("Статус игры", $"{DBase.GameStat.ToString()}", true).AddField("Игр проведено", DBase.ScoreUser1 + DBase.ScoreUser2, true);
                builder.AddField("----------", "Очередь", true);
                if(DBase.User1 == DBase.GoUser)
                {
                    builder.AddField("----------", "Ходящий", true);
                    builder.AddField("----------", "Ждет", true);
                }
                else
                {
                    builder.AddField("----------", "Ждет", true);
                    builder.AddField("----------", "Ходящий", true);
                }
                
                return builder.Build();
            }
            else return null;
        }

        internal bool WinGame(ISocketMessageChannel channel)
        {
            if (DataDictionary.ContainsKey(channel))
            {
                DataType data = DataDictionary[channel];                
                //горизонтально
                if (data.GameStat != Game.CrossZero.StatGame.Win)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if ((data.field3X3[i, 0] == "X") && (data.field3X3[i, 1] == "X") && (data.field3X3[i, 2] == "X"))
                        {
                            data.GameStat = Game.CrossZero.StatGame.Win;
                            data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(0, 85 + 170 * i), new SixLabors.Primitives.PointF(510, 85 + 170 * i) }));
                        }
                        else if ((data.field3X3[i, 0] == "O") && (data.field3X3[i, 1] == "O") && (data.field3X3[i, 2] == "O"))
                        {
                            data.GameStat = Game.CrossZero.StatGame.Win;
                            data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(0, 85 + 170 * i), new SixLabors.Primitives.PointF(510, 85 + 170 * i) }));
                        }
                    }
                }
                //вертикаль
                if (data.GameStat != Game.CrossZero.StatGame.Win)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if ((data.field3X3[0, i] == "X") && (data.field3X3[1, i] == "X") && (data.field3X3[2, i] == "X"))
                        {
                            data.GameStat = Game.CrossZero.StatGame.Win;
                            data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(85 + 170 * i, 0), new SixLabors.Primitives.PointF(85 + 170 * i, 510) }));
                        }
                        else if ((data.field3X3[0, i] == "O") && (data.field3X3[1, i] == "O") && (data.field3X3[2, i] == "O"))
                        {
                            data.GameStat = Game.CrossZero.StatGame.Win;
                            data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(85 + 170 * i, 0), new SixLabors.Primitives.PointF(85 + 170 * i, 510) }));
                        }
                    }
                }
                //побочные линии
                if (data.GameStat != Game.CrossZero.StatGame.Win)
                {
                    if ((data.field3X3[0, 0] == "X") && (data.field3X3[1, 1] == "X") && (data.field3X3[2, 2] == "X"))
                    {
                        data.GameStat = Game.CrossZero.StatGame.Win;
                        data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(0, 0), new SixLabors.Primitives.PointF(510, 510) }));
                    }
                    else if ((data.field3X3[0, 0] == "O") && (data.field3X3[1, 1] == "O") && (data.field3X3[2, 2] == "O"))
                    {
                        data.GameStat = Game.CrossZero.StatGame.Win;
                        data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(0, 0), new SixLabors.Primitives.PointF(510, 510) }));
                    }
                    else if ((data.field3X3[0, 2] == "X") && (data.field3X3[1, 1] == "X") && (data.field3X3[2, 0] == "X"))
                    {
                        data.GameStat = Game.CrossZero.StatGame.Win;
                        data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(510, 0), new SixLabors.Primitives.PointF(0, 510) }));
                    }
                    else if ((data.field3X3[0, 2] == "O") && (data.field3X3[1, 1] == "O") && (data.field3X3[2, 0] == "O"))
                    {
                        data.GameStat = Game.CrossZero.StatGame.Win;
                        data.IField.Mutate(x => x.DrawLines<Rgba32>(data.pen, new SixLabors.Primitives.PointF[] { new SixLabors.Primitives.PointF(510, 0), new SixLabors.Primitives.PointF(0, 510) }));
                    }
                }

                return data.GameStat == StatGame.Win;
            }
            else return false;
        }
    }
}

