using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using System.Data.SqlClient;
using LegionKun.Attribute;

//UCDnNz_stjQqcikCvIF2NTAw - Kanade
//UCScLnRAwAT2qyNcvaFSFvYA - Sharon
//UCuF8ghQWaa7K-28llm-K3Zg - Anilibria.TV
namespace LegionKun.Tests
{
    [Group("tests")]
    [Tests, OwnerOnly]
    class TestClass : InteractiveBase
    {
        [Command("test")]/*Произведено исправление[100]*/
        [Alias("задрал!")]
        public async Task TestAsync(SocketUser text = null)
        {
            if(!Module.ConstVariables.ThisTest)
            {
                return;
            }
            
            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                var mess = await ReplyAsync($"{Context.User.Mention}, :grin:");
                /*if (text == null)
                    return;
                
                var client = new WebClient();
                var res = client.DownloadData(text.GetAvatarUrl());

                using (Image<Rgba32> img = new Image<Rgba32>(Configuration.Default, 500, 500, Rgba32.Black))
                {
                    int i = 1;
                    var fam1 = SixLabors.Fonts.SystemFonts.Find("Arial");
                    
                    foreach (var fam in SixLabors.Fonts.SystemFonts.Families)
                    {
                        if (i < 2)
                        {
                            i++;
                            continue;
                        }

                        SixLabors.Fonts.Font front = new SixLabors.Fonts.Font(fam, (float)20);

                        img.Mutate(x => x.DrawText(text.Username, front, Rgba32.Pink, new SixLabors.Primitives.PointF(img.Height/2, img.Width/2)));
                        break;
                    }

                    await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(img), $"{text.Username}.jpeg");
                }*/

                /*using (Image<Rgba32> img = new Image<Rgba32>(500,500))
                {
                    var bmp = new Bitmap(Module.ConstVariables.Filed);
                    MemoryStream memory = new MemoryStream();
                    bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    var todraw = SixLabors.ImageSharp.Image.Load(memory.ToArray());


                    img.Mutate(x => x.DrawImage(GraphicsOptions.Default, Module.ConstVariables.IFiled));
                    img.Mutate(x => x.DrawImage(GraphicsOptions.Default, Module.ConstVariables.ICross, new SixLabors.Primitives.Point(0,0)));
                    img.Mutate(x => x.DrawImage(GraphicsOptions.Default, Module.ConstVariables.IZero, new SixLabors.Primitives.Point(170, 0)));
                    await Context.Channel.SendFileAsync(Module.ConstVariables.ToStream(img), "filed.png");
                }*/
            }
            else await ReplyAsync($"{Context.User.Mention}, :sweat_smile:");
        }

        [Command("connect")]
        public async Task ConnectAsync()
        {
            await Module.ConstVariables.SendMessageAsync(Context.Channel, "Заморожено!", deleteAfter: 5);
            return;

            string Ask = $"INSERT INTO Users (UserId, Name) VALUES (1, {Context.User.Id}, '{Context.User.Username}')";
            //string Ask = $"UPDATE Users Set Name = '{Context.User.Username}', UserId = {Context.User.Id} WHERE id = 1";
            //string Ask = "DELETE  FROM Users WHERE Name='Sayary'";

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKey))
            {
                conect.Open();
                Console.WriteLine("Подключено!");
                try
                {
                    SqlCommand command = new SqlCommand(Ask, conect);
                    int number = command.ExecuteNonQuery();
                    await Context.Channel.SendMessageAsync(conect.Database);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Отключено!");
        }

        [Command("start")]
        public async Task Async(IGuildUser user)
        {
            try
            {
                await user.ModifyAsync(u => u.Nickname = "string").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }    
}