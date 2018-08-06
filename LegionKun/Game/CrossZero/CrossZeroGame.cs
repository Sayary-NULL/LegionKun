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
        internal async Task<bool> ReplycGameAsync()
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                await DataGameBase[Context.Channel].Message.DeleteAsync();

                DataGameBase[Context.Channel].GetImage();

                DataGameBase[Context.Channel].Sum = 0;

                DataGameBase[Context.Channel].field3X3 = new string[,] { {"", "", "" },
                                                                         {"", "", "" },
                                                                         {"", "", "" }};

                if (Context.User.Id == DataGameBase[Context.Channel].User1.Id)
                {
                    DataGameBase[Context.Channel].GoUser = DataGameBase[Context.Channel].User2;
                    DataGameBase[Context.Channel].ScoreUser1++;
                }
                else
                {
                    DataGameBase[Context.Channel].GoUser = DataGameBase[Context.Channel].User1;
                    DataGameBase[Context.Channel].ScoreUser2++;
                }

                DataGameBase[Context.Channel].GameStat = StatGame.Start;

                await ReplyAndDeleteAsync("Пересоздано", timeout: TimeSpan.FromSeconds(5));

                DataGameBase[Context.Channel].Message = await Context.Channel.SendFileAsync(ToStream(DataGameBase[Context.Channel].IField), "Filledfield.jpg");

                return true;
            }
            else return false;
        }
         
        internal Embed StatusGame(ISocketMessageChannel channel)
        {
            if (DataGameBase.ContainsKey(channel))
            {
                DataType DBase = DataGameBase[channel];
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
            if (DataGameBase.ContainsKey(channel))
            {
                DataType data = DataGameBase[channel];                
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

                return DataGameBase[channel].GameStat == StatGame.Win;
            }
            else return false;
        }

        internal async Task ApplyingParametersAsync()
        {
            DataGameBase[Context.Channel].Sum++;

            if (DataGameBase[Context.Channel].User1 == DataGameBase[Context.Channel].GoUser)
            {
                DataGameBase[Context.Channel].GoUser = DataGameBase[Context.Channel].User2;
            }
            else
            {
                DataGameBase[Context.Channel].GoUser = DataGameBase[Context.Channel].User1;
            }

            if (WinGame(Context.Channel))
            {
                await ReplyAndDeleteAsync($"{Context.User.Mention}, ты выйграл!", timeout: TimeSpan.FromSeconds(5));

                if (await ReplycGameAsync())
                {
                    return;
                }
                else await ReplyAndDeleteAsync("Ошибка!", timeout: TimeSpan.FromSeconds(5));
            }
            else
            {
                if (DataGameBase[Context.Channel].Sum >= 9)
                {
                    await ReplyAndDeleteAsync("Поле закончилось!", timeout: TimeSpan.FromSeconds(5));

                    if (await ReplycGameAsync())
                    {
                        return;
                    }
                    else await ReplyAndDeleteAsync("Ошибка!", timeout: TimeSpan.FromSeconds(5));
                }

                if (DataGameBase[Context.Channel].Message != null)
                {
                    await DataGameBase[Context.Channel].Message.DeleteAsync();
                }

                DataGameBase[Context.Channel].Message = await Context.Channel.SendFileAsync(ToStream(DataGameBase[Context.Channel].IField), "Filledfield.jpg");

                await Context.Message.DeleteAsync();
            }
        }

        internal async Task CommandHandlerAsync()
        {
            switch (DataGameBase[Context.Channel].GameStat)
            {
                case StatGame.Close:
                    {
                        await ReplyAndDeleteAsync("Игра в статусе закрытия!", timeout: TimeSpan.FromSeconds(5));
                        return;
                    }

                case StatGame.Create:
                    {
                        await ReplyAndDeleteAsync("Игра не запущена!", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }

                case StatGame.Stop:
                    {
                        await ReplyAndDeleteAsync("Игра остановлена!", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }

                case StatGame.Win:
                    {
                        await ReplyAndDeleteAsync("Еще не пересоздано!", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }

                case StatGame.Start:
                    {
                        if ((Context.User.Id == DataGameBase[Context.Channel].User1.Id) || (Context.User.Id == DataGameBase[Context.Channel].User2.Id))
                        {
                            await ReplyAndDeleteAsync("Не твой ход!", timeout: TimeSpan.FromSeconds(5));
                        }
                        break;
                    }
                default:
                    {
                        await ReplyAndDeleteAsync("Это как ты сделал значение параметра не из списка? О_О", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }
            }
        }
    }
}

