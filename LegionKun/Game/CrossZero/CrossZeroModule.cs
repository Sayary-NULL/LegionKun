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
        [Command("new")]
        public async Task NewGameAsync(IUser User2, bool New = false)
        {
            if (!Module.ConstVariables.ThisTest)
            {
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "Вразработке", deleteAfter: 5);
                return;
            }

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

            if (!DataDictionary.ContainsKey(Context.Channel) || New)
            {
                if (!New)
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

                    await Module.ConstVariables.SendMessageAsync(Context.Channel, "Создано", deleteAfter: 5);
                }
                else
                {
                    await DataDictionary[Context.Channel].Message.DeleteAsync();
                    DataDictionary[Context.Channel].GetImage();
                    DataDictionary[Context.Channel].Sum++;
                    if(Context.User.Id == DataDictionary[Context.Channel].User1.Id)
                    {
                        DataDictionary[Context.Channel].ScoreUser1++;
                    }
                    else
                    {
                        DataDictionary[Context.Channel].ScoreUser2++;
                    }

                    await Module.ConstVariables.SendMessageAsync(Context.Channel, "Пересоздано", deleteAfter: 5);

                    DataDictionary[Context.Channel].Message = await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(DataDictionary[Context.Channel].IField), "Filledfield.jpg");
                }
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Уже создано!", deleteAfter: 5);
        }

        [Command("start")]
        public async Task StartGameAsync()
        {
            if (!Module.ConstVariables.ThisTest)
            {
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "В разработке!", deleteAfter: 5);
                return;
            }

            if (DataDictionary.ContainsKey(Context.Channel))
            {
                DataType DBase = DataDictionary[Context.Channel];
                if (DBase.GameStat != StatGame.Start)
                {
                    DBase.GameStat = StatGame.Start;

                    await Context.Message.DeleteAsync();

                    await Module.ConstVariables.SendMessageAsync(Context.Channel, "Запуск", deleteAfter: 5);
                }
                else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Уже запущено!", deleteAfter: 5);
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Не создано!", deleteAfter: 5);
        }

        [Command("delete")]
        [Alias("close")]
        public async Task CloseGameAsync()
        {
            if (!Module.ConstVariables.ThisTest)
            {
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "В разработке!", deleteAfter: 5);
                return;
            }

            if (DataDictionary.ContainsKey(Context.Channel))
            {
                await DataDictionary[Context.Channel].Message.DeleteAsync();
                DataDictionary.Remove(Context.Channel);
                await Context.Message.DeleteAsync();
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "Удалено", deleteAfter: 5);
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Не создано", deleteAfter: 5);
        }

        [Command("stop")]
        public async Task StopGameAsync()
        {
            if (DataDictionary.ContainsKey(Context.Channel))
            {
                await Context.Message.DeleteAsync();
                DataDictionary[Context.Channel].GameStat = StatGame.Stop;
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "Остановлено", deleteAfter: 5);
            }
        }

        [Command("status")]
        public async Task StatusGameAync()
        {
            if (!Module.ConstVariables.ThisTest)
            {
                await Module.ConstVariables.SendMessageAsync(Context.Channel, "В разработке!", deleteAfter: 5);
                return;
            }

            Embed embed = StatusGame(Context.Channel);
            if (embed != null)
            {
                await Context.Message.DeleteAsync();

                await Module.ConstVariables.SendMessageAsync(Context.Channel, "", embed: embed, deleteAfter: 15);
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Не создано", deleteAfter: 5);
            
        }

        [Command("A")]
        [Alias("а")]
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
                        }
                        else
                        {
                            data.GoUser = data.User1;
                        }

                        if (WinGame(Context.Channel))
                        {
                            await Module.ConstVariables.SendMessageAsync(Context.Channel, $"{Context.User.Mention}, ты выйграл!", deleteAfter: 5);
                            await NewGameAsync(data.User2, true);
                        }else
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

        [Command("B")]
        [Alias("в")]
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
                            await Module.ConstVariables.SendMessageAsync(Context.Channel, $"{Context.User.Mention}, ты выйграл!", deleteAfter: 5);
                            await NewGameAsync(data.User2, true);
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

        [Command("C")]
        [Alias("с")]
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
                            await Module.ConstVariables.SendMessageAsync(Context.Channel, $"{Context.User.Mention}, ты выйграл!", deleteAfter: 5);
                            await NewGameAsync(data.User2, true);
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
