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
using LegionKun.Attribute;
using System.Data.SqlClient;

namespace LegionKun.Module
{
    class UserCommands : InteractiveBase
    {
        private async Task<bool> Access(string name)
        {
            if (ConstVariables.ThisTest)
            {
                await ReplyAndDeleteAsync($"{Context.User.Mention}, включен тестовый режим!", timeout: TimeSpan.FromSeconds(5));
                return true;
            }

            if (!ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                await ReplyAndDeleteAsync($"{Context.User.Mention}, все команды сейчас выключены!", timeout: TimeSpan.FromSeconds(5));
                return false;
            }

            bool isresult = false;

            foreach (var key in ConstVariables.UserCommand)
            {
                if (key.ContainerName(name))
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
                return;            

            EmbedBuilder builder = new EmbedBuilder();

            if (Context.User.Id != ConstVariables.CreatorId)
                builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());

            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithDescription(mess)
                .WithColor(ConstVariables.UserColor);

            if(Context.User.Id == ConstVariables.CreatorId)
                try
                {
                   await Context.Message.DeleteAsync();
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is error '{e.Message}' is guild '{Context.Guild.Id}' is channel '{Context.Channel.Id}' is user '{Context.User.Username}#{Context.User.Discriminator}'");
                }

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
            catch (Exception e)
            {
                Console.WriteLine("Ошибка доступа!" + e);
            }

            if (user.Id == ConstVariables._Client.CurrentUser.Id)
            {
                EmbedBuilder errors = new EmbedBuilder();

                errors.WithTitle("Ошибка!").WithDescription("нельзя жаловатся на бота!")
                    .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                    .WithColor(ConstVariables.InfoColor);

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
                if (user.Id != ConstVariables.CreatorId)
                    builder.AddField("Коментарий", coment);
            }

            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'warn' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is user2 '{user.Username}' is coment '{coment}'");

            await Context.Channel.SendMessageAsync("", embed: builder.Build());
        }

        [Command("roleinfo")]
        [Priority(0), CategoryChannel(IC:true)]
        public async Task RoleIhfoAsync()
        {
            if (!(await Access("roleinfo")))
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            int CountRole = 0;

            foreach (var key in Context.Guild.Roles)
            {
                if (CountRole < key.Position)
                    CountRole = key.Position;
            }

            string roleinfo = "";

            Dictionary<int, SocketRole> InfoRole = new Dictionary<int, SocketRole>();

            if (ConstVariables.CServer[Context.Guild.Id].Debug || ConstVariables.ThisTest)
            {
                Console.WriteLine($"Полный список ролей сервера");
                Console.WriteLine($"{CountRole}"); /*для отладки*/
            }

            try
            {
                if (ConstVariables.CServer[Context.Guild.Id].Debug || ConstVariables.ThisTest)
                {
                    Console.WriteLine($"Загрузка ролей в базу данных по их номнерам");
                }

                foreach (var role in Context.Guild.Roles)
                    if (role.Id != Context.Guild.EveryoneRole.Id)
                    {
                        try
                        {
                            InfoRole.Add(role.Position - 1, role);

                            if (ConstVariables.CServer[Context.Guild.Id].Debug || ConstVariables.ThisTest)
                            {
                                Console.WriteLine($"{role.Name}, {role.Position}"); /*для отладки*/
                            }
                        }
                        catch (Exception e)
                        {
                            if (ConstVariables.CServer[Context.Guild.Id].Debug || ConstVariables.ThisTest)
                                Console.WriteLine($"{role.Name} {role.Position}({role.Position - 1}): {e.Message}");/*для отладки*/
                            ConstVariables.logger.Error($"{role.Name} {role.Position}({role.Position - 1}): {e.Message}");
                        }
                    }

                for (int i = CountRole; i > 0; i--)
                {
                    try
                    {
                        if (!InfoRole.ContainsKey(i - 1))
                            continue;

                        if (InfoRole[i - 1] == null)
                            continue;

                        roleinfo += $"{CountRole - i + 1}: **{InfoRole[i - 1].Name}** ({InfoRole[i - 1].CreatedAt:dd.MM.yyyy HH:mm})\r\n";
                    }
                    catch (Exception e)
                    {
                        if (ConstVariables.CServer[Context.Guild.Id].Debug || ConstVariables.ThisTest)
                            Console.WriteLine($"{CountRole}, {i}:  {e.Message}");/*для отладки*/
                        ConstVariables.logger.Error($"{CountRole}, {i}:  {e.Message}");
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
        [Priority(1), CategoryChannel(IC: true)]
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
        [Priority(2), CategoryChannel(IC: true)]
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

        [Command("banlistuser")]
        public async Task UserBanList()
        {
            if (!(await Access("banlistuser")))
            {
                return;
            }

            string SqlExpression = "sp_SelectBanListAnd";
            string TextRequest = "";

            EmbedBuilder builder = new EmbedBuilder();

            var User = Context.User;

            builder.WithTitle($"Бан лист(Выборка)");

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                try
                {
                    conect.Open();
                    using (SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        SqlParameter BannedParam = new SqlParameter()
                        {
                            ParameterName = "@BannedId",
                            DbType = System.Data.DbType.Int64,
                            Value = User.Id
                        };

                        command.Parameters.Add(BannedParam);                        

                        SqlParameter GuildIdParam = new SqlParameter()
                        {
                            ParameterName = "@GuildId",
                            DbType = System.Data.DbType.Int64,
                            Value = Context.Guild.Id
                        };

                        command.Parameters.Add(GuildIdParam);

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ulong BannedId = (ulong)reader.GetInt64(0);
                                ulong AdminId = (ulong)reader.GetInt64(1);
                                string Comment = reader.GetString(2);
                                DateTime Date = reader.GetDateTime(3);

                                var banned = Context.Guild.GetUser(BannedId);

                                var admin = Context.Guild.GetUser(AdminId);

                                TextRequest += $"**Пользователь:** {(banned == null ? "----" : banned.Mention)} **Администратор:** {(admin == null ? "----" : admin.Mention)}\r\nЗаметка: {Comment}\r\nДата: {Date}\r\n";
                            }

                            builder.WithDescription(TextRequest)
                                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                                .WithColor(ConstVariables.UserColor);

                            await ReplyAsync("", embed: builder.Build());
                        }
                        else
                        {
                            builder.WithDescription("Данных в базе не обнаружено!")
                                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                                .WithColor(ConstVariables.UserColor);

                            await ReplyAsync("", embed: builder.Build());
                        }                   

                        ConstVariables.logger.Info($"is group 'admin' is command 'BanList' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is UserBanned '{User.Username}#{User.Discriminator}'");
                    }
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is group 'admin' is command 'BanList' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                    await ReplyAndDeleteAsync("Произошла ошибка! Обратитесь к администратору.", timeout: TimeSpan.FromSeconds(5));
                }
            }
        }

        [Command("coin")]
        public async Task ThrowACoinAsync(int count = 1)
        {
            if (!(await Access("coin")))
            {
                return;
            }

            Random ran = new Random();

            if ((count <= 0) || (count > 100))
                count = ran.Next(0, 101);

            EmbedBuilder builder = new EmbedBuilder();
            int ResultArray = 0;

            for (int i = 0; i < count; i++)
            {
                ResultArray += ran.Next(0, 2);//[0;2)
            }

            builder.WithTitle("Результаты броска монеты")
                .WithDescription($"Орел: {ResultArray}\r\nРешка: {count - ResultArray}");

            await ReplyAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'coin' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("search"), CategoryChannel(IC: true)]
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
                if (!ConstVariables.CServer[Context.Guild.Id].IsOn && !ConstVariables.ThisTest)
                {
                    await ReplyAndDeleteAsync($"{Context.User.Mention}, все команды сейчас выключены!", timeout: TimeSpan.FromSeconds(5));
                    return;
                }

                if (ConstVariables.Perevorot && Context.User.Id != ConstVariables.CreatorId)
                {
                    await ReplyAndDeleteAsync("Этой командой можно пользоваться только один раз в день!", timeout: TimeSpan.FromSeconds(5));
                    return;
                }

                if(Context.User.Id != ConstVariables.CreatorId)
                    ConstVariables.Perevorot = true;

                DateTimeOffset time = Context.Message.CreatedAt;

                Random ran = new Random();

                int year = time.Year;

                int month = ran.Next(1, 12);

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
                    ConstVariables.logger.Error("Ошибка доступа:" + e.Message);
                }

                await ReplyAndDeleteAsync("Тихо! Об этом никто не должен знать!", timeout: TimeSpan.FromSeconds(5));
            }

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'perevorot' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("userinfo"), CategoryChannel(IC: true)]
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

            if (User.Activity != null)
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

            foreach (var key in User.RoleIds)
            {
                if (!Context.Guild.GetRole(key).IsEveryone)
                {
                    role += $"{i++}: **{Context.Guild.GetRole(key).Name}** ({Context.Guild.GetRole(key).CreatedAt:dd.MM.yyyy HH:mm})\r\n";
                }
            }

            builder.AddField("Роли", role);

            await Context.Channel.SendMessageAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'userinfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("serverinfo"), CategoryChannel(IC: true)]
        public async Task ServerInfoAsync()
        {
            if (!await Access("serverinfo"))
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

            if (Uri.IsWellFormedUriString(Guild.IconUrl, UriKind.Absolute))
            {
                builder.WithThumbnailUrl(Guild.IconUrl);
            }

            await Context.Channel.SendMessageAsync("", embed: builder.Build());

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'perevorot' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }
       
        [Command("ping"), CategoryChannel(IC: true)]
        public async Task PingAsync()
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
         
        [Command("report"), CategoryChannel(IC: true)]
        public async Task ReportAsync(string command, [Remainder]string text)
        {
            if (!(await Access("report")))
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка доступа: " + e);
            }

            await ReplyAndDeleteAsync($"{Context.User.Mention}, спасибо за ваш отчет! Ваше сообщение очень важно для нас))", timeout: TimeSpan.FromSeconds(5));

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'report' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("help"), CategoryChannel(IC: true)]
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
                if (guild.EntryRole(role.Id))
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