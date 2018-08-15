using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using LegionKun.Module;

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

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'hello' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is socetuser '{(user == null ? "false" : user.Username)}'");

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

            if (Context.User.Id != ConstVariables.CreatorId)
            {
                builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
            }

            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithDescription(mess)
                .WithColor(ConstVariables.UserColor);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'say' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is mess '{mess}'");

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
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

                errors.WithTitle("Ошибка!").WithDescription("нельзя жаловатся на бота!")
                    .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                    . WithColor(ConstVariables.InfoColor);

                await ReplyAndDeleteAsync("", embed: errors.Build(), timeout: TimeSpan.FromSeconds(5));
                return;
            }

            string mess;

            EmbedBuilder builder = new EmbedBuilder();

            if (user.Id == Context.User.Id)
            {
                builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl())
                    .WithDescription("Нельзя жаловаться на себя!")
                    .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                    .WithColor(ConstVariables.InfoColor);

                await ReplyAndDeleteAsync("", embed: builder.Build(), timeout: TimeSpan.FromSeconds(5));
                return;
            }

            builder.WithTitle("Жалоба!").WithColor(ConstVariables.UserColor);

            if (user.Id == ConstVariables.CreatorId)
            {                
                mess = $"Пользователь {user.Mention} пожаловался на {Context.User.Mention}!";
            }
            else mess = $"Пользователь {Context.User.Mention} пожаловался на {user.Mention}!";

            builder.WithDescription(mess);

            if (coment != null)
            {
                if(user.Id != ConstVariables.CreatorId)
                    builder.AddField("Коментарий", coment);
            }

            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'warn' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is user2 '{user.Username}' is coment '{coment}'");

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
        }

        [Command("roleinfo")]
        [Priority(0)]
        public async Task RoleIhfoAsync()
        {
            if (!(await Access("roleinfo")))
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            int CountRole = Context.Guild.Roles.Count - 1;

            string roleinfo = "";

            List<SocketRole> Inforole = new List<SocketRole>(CountRole + 1);

            try
            {
                for (int z = 0; z < CountRole; z++)
                {
                    Inforole.Add(null);
                }

                foreach (var role in Context.Guild.Roles)
                    if (role.Id != Context.Guild.EveryoneRole.Id)
                    {
                        if (ConstVariables.CServer[Context.Guild.Id].Debug || ConstVariables.ThisTest)
                            Console.WriteLine($"{role.Name}, {role.Position}"); /*для отладки*/

                        Inforole.RemoveAt(role.Position - 1);
                        Inforole.Insert(role.Position - 1, role);
                    }

                for (int i = CountRole; i > 0; i--)
                {
                    if (Inforole[i - 1] == null)
                        continue;

                    try
                    {
                        roleinfo += $"{CountRole - i + 1}: **{Inforole[i - 1].Name}** ({Inforole[i - 1].CreatedAt:dd.MM.yyyy HH:mm})\r\n";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{CountRole}, {i}:  {e.Message}");
                    }
                }

                builder.WithTitle($"Количество ролей на сервере: {CountRole}")
                    .WithDescription(roleinfo)
                    .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                    .WithColor(ConstVariables.UserColor);

                ConstVariables.logger.Info($"is guild {Context.Guild.Name} is command 'roleinfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                await Context.Channel.SendMessageAsync("", embed: builder.Build());
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error($"is guild {Context.Guild.Name} is command 'roleinfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is exception '{e}'");
            }
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
                if (!string.IsNullOrWhiteSpace(userr.Nickname))
                    strock += $"{i++}: {userr.Username}#{userr.Discriminator}({userr.Nickname})\r\n";
                else strock += $"{i++}: {userr.Nickname}#{userr.Discriminator}\r\n";
            }

            if (strock == "")
                strock = "Пользователей нет";

            builder.WithTitle($"Информация о роле для {Context.User.Username}")
                .WithAuthor(Role.Name)
                .AddField("Кол-во пользователей с ролью", i - 1, true)
                .AddField("Цвет роли", $"{Role.Color}", true)
                .AddField("Администратор?", Role.Permissions.Administrator, true)
                .AddField("Дата создания", $"{Role.CreatedAt.Day}.{Role.CreatedAt.Month}.{Role.CreatedAt.Year} {Role.CreatedAt.Hour + Role.CreatedAt.Offset.Hours}:{Role.CreatedAt.Minute}:{Role.CreatedAt.Second}.{Role.CreatedAt.Millisecond}", true)
                .AddField("Пользователи", strock)
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithColor(ConstVariables.UserColor);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'RoleInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is role '{Role.Name}'");

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
        }

        [Command("roleinfo")]
        [Priority(2)]
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
                if (string.IsNullOrWhiteSpace(userr.Nickname))
                    strock += $"{i}: {userr.Username}#{userr.Discriminator}\r\n";
                else strock += $"{i}: {userr.Username}#{userr.Discriminator}({userr.Nickname})\r\n";
                i++;
            }

            if (strock == "")
                strock = "Пользователей нет";

            builder.WithTitle($"Информация о роле для {Context.User.Username}")
                .WithAuthor(Role.Name)
                .AddField("Кол-во пользователей с ролью", i - 1, true)
                .AddField("Администратор?", Role.Permissions.Administrator, true)
                .AddField("Цвет роли", $"{Role.Color}", true)
                .AddField("Дата создания", $"{Role.CreatedAt.Day}.{Role.CreatedAt.Month}.{Role.CreatedAt.Year} {Role.CreatedAt.Hour + Role.CreatedAt.Offset.Hours}:{Role.CreatedAt.Minute}:{Role.CreatedAt.Second}.{Role.CreatedAt.Millisecond}", true)
                .AddField("Пользователи", strock)
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithColor(ConstVariables.UserColor);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'RoleInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is role '{Role.Name}'");

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
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

            builder.WithTitle("Time").WithDescription($"local time: {current_time.Hours}:{current_time.Minutes}:{current_time.Seconds}")
                .WithAuthor(Context.User.Username, Context.User.GetAvatarUrl())
                .WithThumbnailUrl("https://media.discordapp.net/attachments/462236317926031370/464149984934100992/time.png?width=473&height=473")
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithColor(ConstVariables.InfoColor);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'time' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
        }

        [Command("coin")]
        public async Task ThrowACoinAsync(int count = 1)
        {
            if (!(await Access("coin")))
            {
                return;
            }

            if ((count <= 0) && (count > 100))
                return;

            Random ran = new Random();
            EmbedBuilder builder = new EmbedBuilder();
            int ResultArray = 0;

            for(int i = 0; i < count; i++)
            {
                ResultArray += ran.Next(0, 2);
            }

            builder.WithTitle("Результаты броска монеты")
                .WithDescription($"Орел: {ResultArray}\r\nРешка:{count - ResultArray}");

            await ReplyAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'coin' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
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
            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithThumbnailUrl("https://media.discordapp.net/attachments/462236317926031370/473478987126013952/yt_logo_rgb_dark.png");

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

            builder.AddField("YouTube video search", strock)
                .WithColor(ConstVariables.InfoColor);

            await Context.Channel.SendMessageAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'help' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is Content '{video}'");
        }

        [Command("perevorot")]
        public async Task PerevorotAsync()
        {
            if ((Context.User.Id == 252459542057713665) || (Context.User.Id == ConstVariables.CreatorId))//Костя
            {
                if (!ConstVariables.CServer[Context.Guild.Id].IsOn)
                {
                    if (!ConstVariables.ThisTest)
                    {
                        await ReplyAndDeleteAsync($"{Context.User.Mention}, все команды сейчас выключены!", timeout: TimeSpan.FromSeconds(5));
                        return;
                    }
                }

                if(ConstVariables.Perevorot)
                {
                    await ReplyAndDeleteAsync("Этой командой можно пользоваться только один раз в день!", timeout: TimeSpan.FromSeconds(5));
                    return;
                }

                ConstVariables.Perevorot = true;

                DateTimeOffset time = Context.Message.CreatedAt;

                Random ran = new Random();

                int year = time.Year;

                int month = ran.Next(1, 13);

                int day = ran.Next(1, 31);

                time = time.AddDays(day).AddMonths(month);

                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, переворот назначен на {time.Day}.{time.Month}.{time.Year}");

                ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'perevorot' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
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

                ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'perevorot' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
            }

        }

        [Command("userinfo")]
        public async Task UserInfoAsync(IGuildUser user = null)
        {
            if (!(await Access("userinfo")))
            {
                return;
            }

            IGuildUser User = Context.User as IGuildUser;

            if (user != null)
            {
                User = user;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.AddField("Имя пользователя", User.Username + "#" + User.Discriminator, true);

            if (!string.IsNullOrWhiteSpace(User.Nickname))
            {
                builder.AddField("Nickname", User.Nickname, true);
            }

            if(User.Activity != null)
            {
                builder.AddField("Activity", User.Activity.Name, true);
            }

            builder.AddField("Дата создания", $"{User.CreatedAt:dd.MM.yyyy HH:mm}", true)
            .AddField("Дата присоединения", User.JoinedAt?.ToString("dd.MM.yyyy HH:mm"), true)
            .AddField("Кол-во ролей", User.RoleIds.Count - 1, true)
            .WithColor(ConstVariables.InfoColor);
            

            string avatar = User.GetAvatarUrl();

            if (Uri.IsWellFormedUriString(avatar, UriKind.Absolute))
            {
                builder.WithThumbnailUrl(avatar);
            }
            
            string role = "";
            int i = 1;

            foreach(var key in User.RoleIds)
            {
                if(!Context.Guild.GetRole(key).IsEveryone)
                {
                    role += $"{i++}: **{Context.Guild.GetRole(key).Name}** ({Context.Guild.GetRole(key).CreatedAt:dd.MM.yyyy HH:mm})\r\n";
                }
            }

            builder.AddField("Роли", role);

            await Context.Channel.SendMessageAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'userinfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("serverinfo")]
        public async Task ServerInfoAsync()
        {
            if(!await Access("serverinfo"))
            {
                return;
            }

            SocketGuild Guild = Context.Guild;

            IGuildUser ownerName = Guild.GetUser(Guild.OwnerId);

            DateTime createdAt = new DateTime(2015, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Guild.Id >> 22);

            string GuildName = Guild.Name;

            int TextChan = Guild.TextChannels.Count;

            int VoiseChan = Guild.VoiceChannels.Count;

            ulong GuildId = Guild.Id;

            EmbedBuilder builder = new EmbedBuilder();

            builder.AddField("Имя", GuildName, true)
                .AddField("Id", GuildId, true)
                .AddField("Создатель", ownerName, true)
                .AddField("Дата создания", createdAt, true)
                .AddField("Члены", Guild.MemberCount, true)
                .AddField("Кол-во ролей", Guild.Roles.Count, true)
                .AddField("Кол-во текстовых каналов", TextChan, true)
                .AddField("Кол-во голосовых каналов", VoiseChan, true)
                .WithColor(ConstVariables.InfoColor);

            if(Uri.IsWellFormedUriString(Guild.IconUrl, UriKind.Absolute))
            {
                builder.WithThumbnailUrl(Guild.IconUrl);
            }

            await Context.Channel.SendMessageAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'perevorot' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("ctinfo")]
        public async Task CTIhfoAsync()
        {
            if (!(await Access("ctinfo")))
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            int CountChannels = Context.Guild.TextChannels.Count;

            string chanel = "";

            List<SocketTextChannel> Channels = new List<SocketTextChannel>(CountChannels);

            try
            {
                for (int z = 0; z < CountChannels; z++)
                {
                    Channels.Add(null);
                }

                foreach (var chan in Context.Guild.TextChannels)
                {
                    Channels.RemoveAt(chan.Position);
                    Channels.Insert(chan.Position, chan);
                    // Console.WriteLine($"{chan.Position}: {chan.Id} {chan.Name}");/*для отладки*/
                }

                for (int i = 0; i < CountChannels; i++)
                {
                    if (Channels[i] == null)
                        continue;
                    //Console.WriteLine($"{i + 1}: {Channels[i].Name}");/*для отладки*/
                    chanel += $"{ i + 1 }: **{Channels[i]}** ({Channels[i].CreatedAt:dd.MM.yyyy HH:mm})\r\n";
                }

                builder.WithTitle($"Количество текстовых каналов на сервере: {CountChannels}")
                    .WithDescription(chanel)
                    .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                    .WithColor(ConstVariables.UserColor);

                ConstVariables.logger.Info($" is command 'cvinfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error($"is guild '{Context.Guild.Name}' is command 'ctinfo' is channel '{Context.Channel.Name}' is user '{Context.User.Username}' is param '{CountChannels}, {Channels.Capacity}, {e.Message}'");
                await ReplyAndDeleteAsync("Ошибка получения информации!", timeout: TimeSpan.FromSeconds(5));
            }

        }

        [Command("cvinfo")]
        public async Task CVIhfoAsync()
        {
            if (!(await Access("cvinfo")))
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            int CountChannel = Context.Guild.VoiceChannels.Count;

            List<SocketVoiceChannel> channels = new List<SocketVoiceChannel>(CountChannel);

            string chanel = "";

            for (int i = 0; i < CountChannel; i++)
            {
                channels.Add(null);
            }

            foreach (var chan in Context.Guild.VoiceChannels)
            {
                // Console.WriteLine($"{chan.Position}: {chan.Name}, {channels.Count}");/*для отладки*/
                channels.RemoveAt(chan.Position);
                channels.Insert(chan.Position, chan);
            }

            for (int i = 0; i < CountChannel; i++)
            {
                chanel += $"{i + 1}: **{channels[i]}** ({channels[i].CreatedAt:dd.MM.yyyy HH:mm})\r\n";
            }

            builder.WithTitle($"Количество голосовых каналов на сервере: {CountChannel}")
                .WithDescription(chanel)
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithColor(ConstVariables.UserColor);

            ConstVariables.logger.Info($" is command 'cvinfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("ping")]
        public async Task Ping()
        {
            if (!(await Access("ping")))
            {
                return;
            }

            var sw = Stopwatch.StartNew();
            var msg = await Context.Channel.SendMessageAsync("😝").ConfigureAwait(false);
            sw.Stop();
            await msg.DeleteAsync();

            ConstVariables.logger.Info($"is guild '{Context.Guild.Name}' is command 'ping' is channel '{Context.Channel.Name}' is user '{Context.User.Username}'");

            await Context.Channel.SendMessageAsync($"{Context.User.Mention}, пинг составляет: {(int)sw.Elapsed.TotalMilliseconds}ms").ConfigureAwait(false);
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

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'report' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("help")]
        public async Task HelpAsync() 
        {
            if (!(await Access("help")))
            {
                return;
            }

            SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

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

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'help' is user '{user.Username}' is channel '{Context.Channel.Name}' is IsRole {IsRole}");

            await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention},", false, builder.Build());
        }
    }
}