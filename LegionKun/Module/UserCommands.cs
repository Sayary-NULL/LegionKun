using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mime;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace LegionKun.Module
{
    class UserCommands : InteractiveBase
    {
        private async Task<bool> Access(string name)
        {
            if (!Module.ConstVariables.ThisTest)
            {
                if (!ConstVariables.CServer[Context.Guild.Id].IsOn)
                {
                    await ReplyAndDeleteAsync($"{Context.User.Mention}, все команды сейчас выключены!", timeout: TimeSpan.FromSeconds(5));
                    return false;
                }

                bool isresult = false;

                foreach (var key in ConstVariables.UserCommand)
                {
                    if (key.ContainerName("report"))
                    {
                        if (key.IsOn)
                        {
                            isresult = true;
                            break;
                        }
                    }
                }

                if (!isresult)
                {
                    await ReplyAndDeleteAsync($"{Context.User.Mention}, это команда сейчас выключена!", timeout: TimeSpan.FromSeconds(5));
                    return false;
                }
            }
            else await ReplyAndDeleteAsync($"{Context.User.Mention}, включен тестовый режим!", timeout: TimeSpan.FromSeconds(5));

            return true;
        }

        [Command("hello")]
        public async Task HelloAsyng(SocketUser user = null)
        {
            if (!(await Access("hello")))
            {
                return;
            }

            TimeSpan current_time = DateTime.Now.TimeOfDay;

            int h = current_time.Hours;

            string good = "";

            if (user != null)
                good = $"{user.Mention}, ";
            else good = $"{Context.Message.Author.Mention}, ";

            if (h < 6)
                good += "Доброй ночи!";
            else if (h < 12)
                good += "Доброе утро!";
            else if (h < 18)
                good += "Добрый день!";
            else if (h < 20)
                good += "Добрый вечер!";
            else if (h < 24)
                good += "Доброй ночи!";

            ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'hello' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is socetuser '{(user == null ? "false" : user.Username)}'");

            await Context.Channel.SendMessageAsync(good);
        }

        [Command("say")]
        public async Task SayMessAsync([Remainder] string mess)
        {
            if (!(await Access("say")))
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl()).WithColor(Color.DarkPurple);
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);
            builder.WithDescription(mess);

            ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'say' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is mess '{mess}'");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("warn")]
        public async Task WarnAsync(SocketUser user, [Remainder] string coment = null)
        {
            if (!(await Access("warn")))
            {
                return;
            }

            try
            {
                await Context.Message.DeleteAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine("Ошибка доступа!" + e);
            }

            if (user.Id == ConstVariables._Client.CurrentUser.Id)
            {
                EmbedBuilder errors = new EmbedBuilder();

                errors.WithTitle("Ошибка!").WithDescription("нельзя жаловатся на бота!");
                errors.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

                await ReplyAndDeleteAsync("", embed: errors.Build(), timeout: TimeSpan.FromSeconds(5));
                return;
            }

            string mess;

            EmbedBuilder builder = new EmbedBuilder();

            if (user.Id == Context.User.Id)
            {
                builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                builder.WithDescription("Нельзя жаловаться на себя!");
                builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

                await ReplyAndDeleteAsync("", embed: builder.Build(), timeout: TimeSpan.FromSeconds(5));
                return;
            }

            builder.WithTitle("Жалоба!").WithColor(Color.Red);

            if (user.Id == ConstVariables.CreatorId)
            {
                mess = $"Пользователь {Context.User.Mention} пожаловался на {user.Mention}!";
            }
            else mess = $"Пользователь {user.Mention} пожаловался на {Context.User.Mention}!";

            builder.WithDescription(mess);

            if (coment != null)
            {
                if(user.Id != ConstVariables.CreatorId)
                    builder.AddField("Коментарий", coment);
            }

            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

            ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'warn' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is user2 '{user.Username}' is coment' {coment}'");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("roleinfo")]
        [Priority(0)]
        public async Task RoleInfoAsync(SocketRole Role)
        {
            if (!(await Access("roleinfo")))
            {
                return;
            }
            
            EmbedBuilder builder = new EmbedBuilder();

            string strock = "";
            int i = 1;

            foreach (var userr in Role.Members)
            {
                if (userr.Nickname != "")
                    strock += $"{i}: {userr.Username}#{userr.Discriminator}\r\n";
                else strock += $"{i}: {userr.Nickname}#{userr.Discriminator}\r\n";
                i++;
            }

            builder.WithTitle($"Информация о роле для {Context.User.Username}");
            builder.WithAuthor(Role.Name);
            builder.AddField("Кол-во пользователей с ролью", i - 1, true);
            builder.AddField("Цвет роли", $"{Role.Color}", true);
            if (strock == "")
                strock = "Пользователей нет";
            builder.AddField("Пользователи", strock);
            builder.AddField("Администратор?", Role.Permissions.Administrator, true).AddField("Дата создания", $"{Role.CreatedAt.Day}.{Role.CreatedAt.Month}.{Role.CreatedAt.Year} {Role.CreatedAt.Hour + Role.CreatedAt.Offset.Hours}:{Role.CreatedAt.Minute}:{Role.CreatedAt.Second}.{Role.CreatedAt.Millisecond}", true);
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl).WithColor(Role.Color);

            Module.ConstVariables.Log?.Invoke($" is Guid {Context.Guild.Name} is command 'RoleInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is role {Role.Name}");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("roleinfo")]
        [Priority(1)]
        public async Task InfoRoleAsync([Remainder]string message)
        {
            if (!(await Access("roleinfo")))
            {
                return;
            }

            bool FoundARole = false;

            ulong roleId = 0;

            foreach (var rol in Context.Guild.Roles)
            {
                if (rol.Name == message)
                {
                    roleId = rol.Id;
                    FoundARole = true;
                }
            }

            if (!FoundARole)
            {
                await ReplyAndDeleteAsync("Роль не найдена", timeout: TimeSpan.FromSeconds(5));
                return;
            }

            SocketRole Role = Context.Guild.GetRole(roleId);

            EmbedBuilder builder = new EmbedBuilder();

            string strock = "";
            int i = 1;

            foreach (var userr in Role.Members)
            {
                if (userr.Nickname != "")
                    strock += $"{i++}: {userr.Username}#{userr.Discriminator}({userr.Nickname})\r\n";
                else strock += $"{i++}: {userr.Nickname}#{userr.Discriminator}\r\n";
            }

            if (strock == "")
                strock = "Пользователей нет";

            builder.WithTitle($"Информация о роле для {Context.User.Username}");
            builder.WithAuthor(Role.Name);
            builder.AddField("Кол-во пользователей с ролью", i - 1, true);
            builder.AddField("Цвет роли", $"{Role.Color}", true);
            builder.AddField("Пользователи", strock);
            builder.AddField("Администратор?", Role.Permissions.Administrator, true).AddField("Дата создания", $"{Role.CreatedAt.Day}.{Role.CreatedAt.Month}.{Role.CreatedAt.Year} {Role.CreatedAt.Hour + Role.CreatedAt.Offset.Hours}:{Role.CreatedAt.Minute}:{Role.CreatedAt.Second}.{Role.CreatedAt.Millisecond}", true);
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl).WithColor(Role.Color);

            ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'RoleInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is role '{Role.Name}'");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("time")]
        public async Task TimeAsync()
        {
            if (!(await Access("time")))
            {
                return;
            }

            TimeSpan current_time = DateTime.Now.TimeOfDay;
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Time").WithDescription($"local time: {current_time.Hours}:{current_time.Minutes}:{current_time.Seconds}");
            builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
            builder.WithThumbnailUrl("https://media.discordapp.net/attachments/462236317926031370/464149984934100992/time.png?width=473&height=473");
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl).WithColor(Color.DarkTeal);

            Module.ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'time' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("random")]
        public async Task RandomAsync(int min, int max)
        {
            if (!(await Access("random")))
            {
                return;
            }

            if (min > max)
            {
                min += max;
                max = min - max;
                min = min - max;
            }

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            Random ran = new Random();
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Случайное число с промежутка: от {min} до {max}").WithColor(Discord.Color.DarkGreen);
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);
            builder.WithDescription($"Выпало число: {ran.Next(min, max)}");

            var mess = await Context.Channel.SendMessageAsync("", false, builder.Build());

            await mess.AddReactionAsync(ConstVariables.DEmoji.EReturn);

            guild.RMessages.MaxValue = max;
            guild.RMessages.MinValue = min;
            guild.RMessages.Embed = builder;
            guild.RMessages.RestUser = mess;
            guild.RMessages.UserId = Context.User.Id;
            Module.ConstVariables.DMessage.Add(mess.Id, guild.GuildId);

            Module.ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'help' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is Random Min:{min} Max: {max}");
        }

        [Command("search")]
        public async Task SearchAsync([Remainder]string video)
        {
            if (!(await Access("search")))
            {
                return;
            }

            ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);
            builder.WithThumbnailUrl("https://media.discordapp.net/attachments/462236317926031370/473478987126013952/yt_logo_rgb_dark.png");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDIuH33zi6aod6jSHm31V1VIVKYIIGxvEo",
                ApplicationName = this.GetType().ToString()
            });

            var SearchVideo = youtubeService.Search.List("snippet");
            SearchVideo.Q = video;
            SearchVideo.Type = "video";
            SearchVideo.ChannelId = "UCScLnRAwAT2qyNcvaFSFvYA";
            SearchVideo.MaxResults = 5;

            var SearchResult = await SearchVideo.ExecuteAsync();
            int i = 1;
            string strock = "";
            foreach (var Search in SearchResult.Items)
            {
                strock += $"{i++}: {Search.Snippet.Title}\r\nurl:https://www.youtube.com/video/" + Search.Id.VideoId + "\r\n";
            }
            builder.AddField("YouTube video search", strock);
            await Context.Channel.SendMessageAsync("", false, builder.Build());

            ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'help' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is Content '{video}'");
        }

        [Command("perevorot")]
        public async Task PerevorotAsync()
        {
            if ((Context.User.Id == 252459542057713665) || (Context.User.Id == 329653972728020994))//Костя
            {
                if (!ConstVariables.CServer[Context.Guild.Id].IsOn)
                {
                    if (!ConstVariables.ThisTest)
                    {
                        await ReplyAndDeleteAsync($"{Context.User.Mention}, все команды сейчас выключены!", timeout: TimeSpan.FromSeconds(5));
                        return;
                    }
                }

                DateTimeOffset time = Context.Message.CreatedAt;

                Random ran = new Random();

                int year = time.Year;

                int month = ran.Next(1, 13);

                int day = ran.Next(1, 31);

                time = time.AddDays(day).AddMonths(month);

                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, переворот назначен на {time.Day}.{time.Month}.{time.Year}");
            }
            else
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch (Exception e)
                {
                    ConstVariables.Mess?.Invoke("Ошибка доступа:" + e.Message);
                }

                await ReplyAndDeleteAsync("Тихо! Об этом никто не должен знать!", timeout: TimeSpan.FromSeconds(5));
            }

        }

        [Command("userinfo")]
        public async Task UserInfoAsync(IGuildUser user = null)
        {
            if (!(await Access("userinfo")))
            {
                return;
            }

            var User = user ?? Context.User as IGuildUser;

            EmbedBuilder builder = new EmbedBuilder();

            if (!string.IsNullOrWhiteSpace(User.Nickname))
            {
                builder.AddField("Nickname", user.Nickname);
            }

            builder.AddField("Activity", User.Activity.Name, true)
                .AddField("Имя пользователя", User.Username, true)
                .AddField("Дата создания", $"{User.CreatedAt:dd.MM.yyyy HH:mm}", true)
                .AddField("Дата присоединения", User.JoinedAt?.ToString("dd.MM.yyyy HH:mm"), true)
                .AddField("Кол-во ролей", User.RoleIds.Count - 1);

            var avatar = user.GetAvatarUrl();

            if (avatar != null)
            {
                builder.WithThumbnailUrl(avatar);
            }

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
        }

        [Command("report")]
        public async Task ReportAsync(string command, [Remainder]string text)
        {
            if(!(await Access("report")))
            {
                return;
            }

            string UMention = Context.User.Username + "#" + Context.User.Discriminator;
            string report = $"Пользователь: {UMention}\r\nКоманда: {command}\r\nСообщение: {text}";

            ConstVariables.Mess?.Invoke(report);
            await ConstVariables.CServer[Context.Guild.Id].GetGuild().GetUser(329653972728020994).SendMessageAsync(report);

            try
            {
                await Context.Message.DeleteAsync();
            }catch(Exception e)
            {
                Console.WriteLine("Ошибка доступа: " + e);
            }

            await ReplyAndDeleteAsync($"{Context.User.Mention}, спасибо за ваш отчет! Ваше сообщение очень важно для нас))", timeout: TimeSpan.FromSeconds(5)); 
        }

        [Command("help")]
        public async Task HelpAsync() 
        {
            if (!(await Access("help")))
            {
                return;
            }

            SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            EmbedBuilder builder = new EmbedBuilder();

            string TitlList = "Префикс команд для бота 'sh!' или 'Sh!'";

            builder.AddField("Параметры", "[] - обязательно \r\n<> - не обязательно");

            builder.WithTitle(TitlList);

            builder.AddField("Group: Default", Module.ConstVariables.UTHelp, true);

            if (IsRole)
            {
                builder.AddField("Group: Admin", Module.ConstVariables.ATHelp, true);
            }

            builder.WithColor(Color.Orange);
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

            Module.ConstVariables.Log?.Invoke($" is Guid '{Context.Guild.Name}' is command 'help' is user '{user.Username}' is channel '{Context.Channel.Name}' is IsRole {IsRole} ");

            await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention},", false, builder.Build());

        }
    }
}