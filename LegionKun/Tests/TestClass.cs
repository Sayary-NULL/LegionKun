using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.SimplePermissions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Reflection;
using System.Collections.Generic;
using Discord.Addons.MpGame;
/*Игра в крестики нолики*/
//UCDnNz_stjQqcikCvIF2NTAw - Kanade
//UCScLnRAwAT2qyNcvaFSFvYA - Sharon
//UCuF8ghQWaa7K-28llm-K3Zg - Anilibria.TV
namespace LegionKun.Tests
{
    [Group("tests")]
    public class TestClass : ModuleBase<SocketCommandContext>
    {
        [Command("test")]/*Произведено исправление[10]*/
        [Alias("tem")]
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
                if (text == null)
                    return;
                /*EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Test").WithColor(Color.Magenta).WithDescription("text").WithFooter("text", Context.Guild.IconUrl);

                builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl(), Context.User.GetAvatarUrl());

                var mess = await Context.Channel.SendMessageAsync("", false, builder.Build());

                builder.WithDescription("edit test");

                await mess.ModifyAsync(msg => msg.Embed = builder.Build());*/

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
                }
                /*MemoryStream memory = new MemoryStream(res);

                //var bmp = new Bitmap(500, 500);
                var bmp = new Bitmap(memory);

                var g = Graphics.FromImage(bmp);
                g.DrawString(text.Username, new Font(FontFamily.Families[5], 10, FontStyle.Underline), new SolidBrush(System.Drawing.Color.Black), 0, 0);
                bmp.Save("result.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                await Context.Channel.SendFileAsync(memory, $"{text.Username}.ipg");

                g.Dispose();
                bmp.Dispose();

                await Context.Channel.SendFileAsync("result.jpg");*/
            }
            else await ReplyAsync($"{Context.User.Mention}, :sweat_smile:");



        }
    }
}
