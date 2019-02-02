using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;


namespace LegionKun.Game.CrossZero
{
    sealed class CrossZeroModule : CrossZeroGame
    {
        [Command("help")]
        public async Task HelpGameAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            string help = "";

            help = "1)new [Mention]\r\n";
            help += "2)start\r\n";
            help += "3)status\r\n";
            help += "4)reset\r\n";
            help += "5)a [number]\r\n";
            help += "6)b [number]\r\n";
            help += "7)c [number]\r\n";
            builder.WithTitle("Информация по игре: 'Крестики-Нолики от LegionKun'a ' ");
            builder.AddField("Параметры", "Префикс команд для бота 'c!'\r\n[] - обязательно \r\n<> - не обязательно");
            builder.AddField("Команды бота ", help);
            builder.AddField("Краткое описание", "!!Игра находится в стутусе альфа тестирования!!\r\nДля начала игры требуется создать ее: c!new @Sayary#2523\r\n" +
                                                 "После создания игры требуется ее запуск: c!start\r\n" +
                                                 "У игры есть внутренняя статистка: c!status\r\n" +
                                                 "Поле поделено на сектора.У сектора есть имя из буквы(a, b, c) и цифры(1, 2, 3).Буква и цифра разделены пробелом.\r\n" +
                                                 "Ход осуществляется написанием команды состоящая из префикса и номера сектора: c!a 1, c!b 3\r\n" +
                                                 "После победы или нехватки грогового поля, то игра пересоздаст поле и начнется новый игровой цикл.\r\n" +
                                                 "Если игра работает 'некорректно', то сообщайте мне в лс.Меня зовут(@Sayary#2523)\r\n");

            await ReplyAndDeleteAsync("", embed: builder.Build(), timeout: TimeSpan.FromMinutes(1));

        }

        [Command("new", RunMode = RunMode.Async)]
        public async Task NewGameAsync(IUser User2)
        {
            if (Context.User == User2)
            {
                await ReplyAndDeleteAsync("Самим с собой нельзя!", timeout: TimeSpan.FromSeconds(5));

                return;
            }

            if (User2.IsBot)
            {
                await ReplyAndDeleteAsync("C ботом нельзя!", timeout: TimeSpan.FromSeconds(5));
                return;
            }

            if (!DataGameBase.ContainsKey(Context.Channel))
            {
                DataType DBase = new DataType
                {
                    User1 = Context.User,
                    User2 = User2,
                    Channelsgame = Context.Channel,
                    Guild = Context.Guild,
                    Message = null,
                    GameStat = StatGame.Create,
                    GoUser = Context.User,
                    Sum = 1
                };
                
                DBase.GetImage();
                DataGameBase.Add(Context.Channel, DBase);
                await Context.Message.DeleteAsync();
                DBase.Message = await Context.Channel.SendFileAsync(ToStream(DBase.IField), "Filledfield.jpg");

                await ReplyAndDeleteAsync("Создано", timeout: TimeSpan.FromSeconds(5));
            }
            else await ReplyAndDeleteAsync("Уже создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("start")]
        public async Task StartGameAsync()
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                DataType DBase = DataGameBase[Context.Channel];

                if ((Context.User.Id != DBase.User1.Id) && (Context.User.Id != DBase.User2.Id))
                {
                    return;
                }

                if (DBase.GameStat != StatGame.Start)
                {
                    DBase.GameStat = StatGame.Start;

                    await Context.Message.DeleteAsync();
                    
                    await ReplyAndDeleteAsync("Запуск", timeout: TimeSpan.FromSeconds(5));
                }
                else await ReplyAndDeleteAsync("Уже запущено", timeout: TimeSpan.FromSeconds(5));
            }
            else await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("delete"), Alias("close")]
        public async Task CloseGameAsync()
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                if ((Context.User.Id != DataGameBase[Context.Channel].User1.Id) && (Context.User.Id != DataGameBase[Context.Channel].User2.Id))
                {
                    return;
                }

                await DataGameBase[Context.Channel].Message.DeleteAsync();
                DataGameBase.Remove(Context.Channel);
                await Context.Message.DeleteAsync();
                await ReplyAndDeleteAsync("Удалено", timeout: TimeSpan.FromSeconds(5));
            }
            else await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("stop")]
        public async Task StopGameAsync()
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                if ((Context.User.Id != DataGameBase[Context.Channel].User1.Id) && (Context.User.Id != DataGameBase[Context.Channel].User2.Id))
                {
                    return;
                }

                await Context.Message.DeleteAsync();
                DataGameBase[Context.Channel].GameStat = StatGame.Stop;

                await ReplyAndDeleteAsync("Остановка. . .", timeout: TimeSpan.FromSeconds(5));
            } else await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("status")]
        public async Task StatusGameAync()
        {
            if(!DataGameBase.ContainsKey(Context.Channel))
            {
                await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
                return;
            }

            Embed embed = StatusGame(Context.Channel);

            await Context.Message.DeleteAsync();

            await ReplyAndDeleteAsync("", embed: embed, timeout: TimeSpan.FromSeconds(20));            
        }

        [Command("reset")]
        public async Task ReSetAsync()
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                if ((Context.User.Id != DataGameBase[Context.Channel].User1.Id) && (Context.User.Id != DataGameBase[Context.Channel].User2.Id))
                {
                    return;
                }

                await ReplycGameAsync();
            }
            else await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("A", RunMode = RunMode.Async), Alias("ф")]
        public async Task AFieldAsync(int number)
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                DataType data = DataGameBase[Context.Channel];

                string mess = Context.Message.Content.ToLower();

                if ((data.GameStat == StatGame.Start) && (Context.User == data.GoUser))
                {
                    mess = mess.Remove(0, 1);

                    bool course = false;

                    if ((number == 1) && (data.field3X3[0, 0] == ""))
                    {
                        if ((data.User1.Id == Context.User.Id) && (data.field3X3[0, 0] == ""))
                        {
                            data.field3X3[0, 0] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(0, 0)));
                        }
                        else
                        {
                            data.field3X3[0, 0] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(0, 0)));
                        }
                        course = true;
                    }
                    else if ((number == 2) && (data.field3X3[0, 1] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[0, 1] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(170, 0)));
                        }
                        else
                        {
                            data.field3X3[0, 1] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(170, 0)));
                        }
                        course = true;
                    }
                    else if ((number == 3) && (data.field3X3[0, 2] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[0, 2] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(340, 0)));
                        }
                        else
                        {
                            data.field3X3[0, 2] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(340, 0)));
                        }
                        course = true;
                    }

                    if(course)
                    {
                        await ApplyingParametersAsync();
                    }
                }
                else
                {
                    await CommandHandlerAsync();
                }
            }
        }

        [Command("B", RunMode = RunMode.Async), Alias("и")]
        public async Task BFieldAsync(int number)
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                DataType data = DataGameBase[Context.Channel];

                string mess = Context.Message.Content.ToLower();

                if ((data.GameStat == Game.CrossZero.StatGame.Start) && (Context.User == data.GoUser))
                {
                    mess = mess.Remove(0, 1);

                    bool course = false;

                    if ((number == 1) && (data.field3X3[1, 0] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[1, 0] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(0, 170)));
                        }
                        else
                        {
                            data.field3X3[1, 0] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(0, 170)));
                        }
                        course = true;
                    }
                    else if ((number == 2) && (data.field3X3[1, 1] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[1, 1] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(170, 170)));
                        }
                        else
                        {
                            data.field3X3[1, 1] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(170, 170)));
                        }
                        course = true;
                    }
                    else if ((number == 3) && (data.field3X3[1, 2] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[1, 2] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(340, 170)));
                        }
                        else
                        {
                            data.field3X3[1, 2] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(340, 170)));
                        }
                        course = true;
                    }

                    if (course)
                    {
                        await ApplyingParametersAsync();
                    }
                }
                else
                {
                    await CommandHandlerAsync();
                }
            }
        }

        [Command("C", RunMode = RunMode.Async), Alias("с")]
        public async Task CFieldAsync(int number)
        {
            if (DataGameBase.ContainsKey(Context.Channel))
            {
                DataType data = DataGameBase[Context.Channel];

                string mess = Context.Message.Content.ToLower();

                if ((data.GameStat == Game.CrossZero.StatGame.Start) && (Context.User == data.GoUser))
                {
                    mess = mess.Remove(0, 1);

                    bool course = false;

                    if ((number == 1) && (data.field3X3[2, 0] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[2, 0] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(0, 340)));
                        }
                        else
                        {
                            data.field3X3[2, 0] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(0, 340)));
                        }
                        course = true;
                    }
                    else if ((number == 2) && (data.field3X3[2, 1] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[2, 1] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(170, 340)));
                        }
                        else
                        {
                            data.field3X3[2, 1] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(170, 340)));
                        }
                        course = true;
                    }
                    else if ((number == 3) && (data.field3X3[2, 2] == ""))
                    {
                        if (data.User1.Id == Context.User.Id)
                        {
                            data.field3X3[2, 2] = data.GUser1;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.ICross, new SixLabors.Primitives.Point(340, 340)));
                        }
                        else
                        {
                            data.field3X3[2, 2] = data.GUser2;
                            data.IField.Mutate(x => x.DrawImage(GraphicsOptions.Default, data.IZero, new SixLabors.Primitives.Point(340, 340)));
                        }
                        course = true;
                    }

                    if (course)
                    {
                        await ApplyingParametersAsync();
                    }
                }
                else
                {
                    await CommandHandlerAsync();
                }
            }
        }
    }
}
