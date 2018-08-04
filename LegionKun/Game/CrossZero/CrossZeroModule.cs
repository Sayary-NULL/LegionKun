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
        [Command("new", RunMode = RunMode.Async)]
        public async Task NewGameAsync(IUser User2)
        {
            if (Context.User == User2)
            {
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "Самим с собой нельзя!", deleteAfter: 5);

                return;
            }

            if (User2.IsBot)
            {
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "C ботом нельзя!", deleteAfter: 5);
                return;
            }

            if (!DataDictionary.ContainsKey(Context.Channel))
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
                DataDictionary.Add(Context.Channel, DBase);
                await Context.Message.DeleteAsync();
                DBase.Message = await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(DBase.IField), "Filledfield.jpg");

                await ReplyAndDeleteAsync("Создано", timeout: TimeSpan.FromSeconds(5));
                
            }
            else await ReplyAndDeleteAsync("Уже создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("start")]
        public async Task StartGameAsync()
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                DataType DBase = DataDictionary[Context.Channel];
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
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                await DataDictionary[Context.Channel].Message.DeleteAsync();
                DataDictionary.Remove(Context.Channel);
                await Context.Message.DeleteAsync();
                await ReplyAndDeleteAsync("Удалено", timeout: TimeSpan.FromSeconds(5));
            }
            else await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("stop")]
        public async Task StopGameAsync()
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                await Context.Message.DeleteAsync();
                DataDictionary[Context.Channel].GameStat = StatGame.Stop;

                await ReplyAndDeleteAsync("Остановка. . .", timeout: TimeSpan.FromSeconds(5));
            }
        }

        [Command("status")]
        public async Task StatusGameAync()
        {
            Embed embed = StatusGame(Context.Channel);
            if (embed != null)
            {
                await Context.Message.DeleteAsync();
                
                await ReplyAndDeleteAsync("", embed: embed, timeout: TimeSpan.FromSeconds(15));
            }
            else await ReplyAndDeleteAsync("Не создано", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("A", RunMode = RunMode.Async), Alias("а")]
        public async Task AFieldAsync(int number)
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                DataType data = DataDictionary[Context.Channel];

                string mess = Context.Message.Content.ToLower();

                if ((data.GameStat == Game.CrossZero.StatGame.Start) && (Context.User == data.GoUser))
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
                        if (data.User1 == data.GoUser)
                        {
                            data.GoUser = data.User2;
                            data.ScoreUser1++;
                        }
                        else
                        {
                            data.GoUser = data.User1;
                            data.ScoreUser2++;
                        }

                        if (WinGame(Context.Channel))
                        {
                            await ReplyAndDeleteAsync($"{Context.User.Mention}, ты выйграл!", timeout: TimeSpan.FromSeconds(5));

                            if(await ReplycGameAsync(Context))
                            {
                                return;
                            }
                        }
                        else
                        {
                            await Context.Message.DeleteAsync();

                            if (data.Message != null)
                            {
                                await data.Message.DeleteAsync();
                            }

                            data.Message = await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(data.IField), "Filledfield.jpg");

                            DataDictionary.Remove(Context.Channel);
                            DataDictionary.Add(Context.Channel, data);

                        }
                    }
                }
            }
        }

        [Command("B", RunMode = RunMode.Async), Alias("в")]
        public async Task BFieldAsync(int number)
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                DataType data = DataDictionary[Context.Channel];

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
                        if (data.User1 == data.GoUser)
                        {
                            data.GoUser = data.User2;
                        }
                        else
                        {
                            data.GoUser = data.User1;
                        }

                        if (WinGame(Context.Channel))
                        {
                            await ReplyAndDeleteAsync($"{Context.User.Mention}, ты выйграл!", timeout: TimeSpan.FromSeconds(5));

                            if (await ReplycGameAsync(Context))
                            {
                                return;
                            }
                        }

                        await Context.Message.DeleteAsync();

                        if (data.Message != null)
                        {
                            await data.Message.DeleteAsync();
                        }

                        data.Message = await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(data.IField), "Filledfield.jpg");

                        DataDictionary.Remove(Context.Channel);
                        DataDictionary.Add(Context.Channel, data);
                    }
                }
            }
        }

        [Command("C", RunMode = RunMode.Async), Alias("с")]
        public async Task CFieldAsync(int number)
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                DataType data = DataDictionary[Context.Channel];

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
                        if (data.User1 == data.GoUser)
                        {
                            data.GoUser = data.User2;
                        }
                        else
                        {
                            data.GoUser = data.User1;
                        }

                        if (WinGame(Context.Channel))
                        {
                            await ReplyAndDeleteAsync($"{Context.User.Mention}, ты выйграл!", timeout: TimeSpan.FromSeconds(5));

                            if (await ReplycGameAsync(Context))
                            {
                                return;
                            }
                        }

                        await Context.Message.DeleteAsync();

                        if (data.Message != null)
                        {
                            await data.Message.DeleteAsync();
                        }

                        data.Message = await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(data.IField), "Filledfield.jpg");

                        DataDictionary.Remove(Context.Channel);
                        DataDictionary.Add(Context.Channel, data);
                    }
                }
            }
        }
    }
}
