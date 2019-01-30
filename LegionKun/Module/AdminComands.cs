using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System;
using System.Threading.Tasks;
using LegionKun.Attribute;
using System.Data.SqlClient;
using Discord.WebSocket;
using System.Diagnostics;

namespace LegionKun.Module
{
    [Admin]
    class AdminComands : InteractiveBase
    {
        private async Task<bool> Access(string name)
        {
            if(name == "off" || name == "on" || name == "news")
            {
                await ReplyAndDeleteAsync($"{Context.User.Mention}, Эти команды недоступны в тестовом режиме!", timeout: TimeSpan.FromSeconds(5));
                return false;
            }

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

            foreach (var key in ConstVariables.AdminCommand)
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

        [Command("off", RunMode = RunMode.Async)]
        public async Task OffBotAsync(byte level = 2)
        {
            if (!(await Access("off")))
                return;

            await ConstVariables._Client.SetStatusAsync(UserStatus.DoNotDisturb);

            switch (level)
            {
                case 0:
                    {
                        EmbedBuilder builder = new EmbedBuilder();

                        builder.WithTitle("!!!Внимание!!!").WithDescription("бот будет выключен на обновление :stuck_out_tongue_winking_eye:").WithColor(Discord.Color.Magenta);

                        foreach (var server in ConstVariables.CServer)
                        {
                            ConstVariables.CServer[server.Key].IsOn = false;

                            ConstVariables.CDiscord guild = server.Value;

                            var result = await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                            IEmote em = new Emoji("😜");

                            await result.AddReactionAsync(em);
                        }

                        break;
                    }

                case 1:
                    {
                        ConstVariables.CServer[Context.Guild.Id].IsOn = false;
                        break;
                    }
                case 2:
                    {
                        foreach (var key in ConstVariables.CServer)
                        {
                            ConstVariables.CServer[key.Key].IsOn = false;
                        }

                        break;
                    }
                case 3:
                    {
                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle("!!!Внимание!!!").WithDescription("бот будет выключен на обновление :stuck_out_tongue_winking_eye:").WithColor(Discord.Color.Magenta);
                        foreach (var key in Module.ConstVariables.CServer)
                        {
                            ConstVariables.CServer[key.Key].IsOn = false;
                        }

                        var result = await ConstVariables.CServer[Context.Guild.Id].GetDefaultChannel().SendMessageAsync("", false, builder.Build());
                        IEmote em = new Emoji("😜");
                        await result.AddReactionAsync(em);

                        break;
                    }
                default: { break; }
            }

            try
            {
                await Context.Message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            ConstVariables.logger.Info($"is group 'Automatic' is command 'off' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level '{level}'");
        }

        [Command("on", RunMode = RunMode.Async)]
        public async Task OnBotAsync(byte level = 2)
        {
            if (!(await Access("on")))
                return;

            await ConstVariables._Client.SetStatusAsync(UserStatus.Online);

            switch (level)
            {
                case 0:
                    {

                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle("Ура").WithDescription("Я снова в строю").WithColor(Discord.Color.Magenta);

                        foreach (var server in ConstVariables.CServer)
                        {
                            ConstVariables.CServer[server.Key].IsOn = true;

                            ConstVariables.CDiscord guild = server.Value;

                            var result = await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                            IEmote em = new Emoji("💗");

                            await result.AddReactionAsync(em);
                        }

                        break;
                    }

                case 1:
                    {
                        ConstVariables.CServer[Context.Guild.Id].IsOn = true;
                        break;
                    }
                case 2:
                    {
                        foreach (var key in Module.ConstVariables.CServer)
                        {
                            ConstVariables.CServer[key.Key].IsOn = true;
                        }
                        break;
                    }
                case 3:
                    {
                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle("Ура").WithDescription("Я снова в строю").WithColor(Discord.Color.Magenta);

                        foreach (var key in Module.ConstVariables.CServer)
                        {
                            ConstVariables.CServer[key.Key].IsOn = true;
                        }

                        var result = await ConstVariables.CServer[Context.Guild.Id].GetDefaultChannel().SendMessageAsync("", false, builder.Build());
                        IEmote em = new Emoji("💗");
                        await result.AddReactionAsync(em);

                        break;
                    }
                default: { break; }
            }

            try
            {
                await Context.Message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            ConstVariables.logger.Info($"is group 'Automatic' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
        }

        [Command("news", RunMode = RunMode.Async)]
        public async Task NewsAsync([Remainder]string mess)
        {
            if (!(await Access("news")))
                return;

            string URL = "Base/news26052017.jpg";
            bool isfalg = false;
            string text = "";
            int index = 0;

            if (mess.IndexOf("image:") > -1)
            {
                URL = "";
                isfalg = true;
                index = mess.IndexOf("image:") + 6;
                if (mess[index] == ' ')
                    index++;

                while (mess[index] != ';' && mess[index] != '\n')
                {
                    URL += mess[index];
                    index++;
                    if (index >= mess.Length)
                        break;
                }
            }

            if (mess.IndexOf("text:") > -1)
            {
                index = mess.IndexOf("text:") + 5;
                if (mess[index] == ' ')
                    index++;

                for (; index < mess.Length; index++)
                    text += mess[index];
            }

            var result = await Context.Channel.SendMessageAsync("Начата рассылка!\n Ждите, если это сообщение не проподет, то напишите Sayary.");

            foreach (var server in ConstVariables.CServer)
            {
                if (server.Value.DefaultChannelNewsId == 0)
                {
                    continue;
                }

                try
                {
                    ConstVariables.CDiscord guild = server.Value; 

                    if (ConstVariables.ThisTest)
                    {
                        if (server.Value.GuildId != 435485527156981770)
                            continue;

                        if (isfalg)
                        {
                            await guild.GetDefaultNewsChannel().SendMessageAsync(URL);
                        }
                        else await guild.GetDefaultNewsChannel().SendFileAsync(URL, " ");

                        await guild.GetDefaultNewsChannel().SendMessageAsync(text);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                        break;
                    }
                    else if (Context.User.Id == ConstVariables.CreatorId)
                    {
                        if (isfalg)
                        {
                            await guild.GetDefaultNewsChannel().SendMessageAsync(URL);
                        }
                        else await guild.GetDefaultNewsChannel().SendFileAsync(URL, " ");

                        await guild.GetDefaultNewsChannel().SendMessageAsync(text);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                    }
                    else
                    {
                        if (Context.Guild.Id == server.Value.GuildId)
                        {
                            if (isfalg)
                                await guild.GetDefaultNewsChannel().SendMessageAsync(URL);

                            await guild.GetDefaultNewsChannel().SendFileAsync("Base/LegioNews2.png", " ");
                            await guild.GetDefaultNewsChannel().SendMessageAsync(text);
                            await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is group 'Automatic' is command 'news' is user '{Context.User.Username}#{Context.User.Discriminator}' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is errors '{e.Message}'");
                    throw;
                }
            }

            try
            {
                await Context.Message.DeleteAsync();
                await result.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            ConstVariables.logger.Info($"is group 'Automatic' is command 'news' is user '{Context.User.Username}#{Context.User.Discriminator}' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}'");
        }

        [Command("status")]
        [CategoryChannel(IC: true)]
        public async Task StatusAsync()
        {
            if (!(await Access("status")))
                return;

            try
            {
                await Context.Message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            string SqlRequestRole = $"SELECT [RoleId] FROM [Role] WHERE [GuildId] = {Context.Guild.Id}";
            string SqlRequestChannel = $"SELECT [ChannelId] FROM [Channel] WHERE [GuildId] = {Context.Guild.Id}";

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            EmbedBuilder builder = new EmbedBuilder();

            string role = "";

            string channel = "";

            builder.WithTitle("the status of the bot")
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithAuthor(Context.User.Username, Context.User.GetAvatarUrl())
                .AddField("Guild", Context.Guild.Name, true)
                .AddField("Resurs", guild.CountRes + "/" + guild.Restruction, true)
                .AddField("Is on?", ConstVariables.CServer[Context.Guild.Id].IsOn, true)
                .WithColor(ConstVariables.AdminColor);

            int i = 1;

            using (SqlConnection connect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                connect.Open();
                SqlCommand command = new SqlCommand(SqlRequestRole, connect);
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            role += $"{i++}: **{Context.Guild.GetRole((ulong)reader.GetInt64(0)).Name}**\r\n";
                        }
                    }
                    else throw new Exception($"Ошибка в чтении ролей сервера: {Context.Guild.Id}");
                }                

                i = 1;

                command.CommandText = SqlRequestChannel;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            channel += $"{i++}: **{Context.Guild.GetChannel((ulong)reader.GetInt64(0)).Name}**\r\n";
                        }
                    }
                    else throw new Exception($"Ошибка в чтении каналов сервера: {Context.Guild.Id}");
                }
            }

            builder.AddField("Roles", role, true)
                   .AddField("Channels", channel, true)
                   .AddField("Версия бота", Base.Resource1.VersionBot, true)
                   .AddField("Defaul channel", Context.Guild.GetTextChannel(guild.DefaultChannelId).Mention, true)
                   .AddField("Default channel for news", Context.Guild.GetTextChannel(guild.DefaultChannelNewsId).Mention, true)
                   .AddField("Guild Id", guild.GuildId, true);

            await Context.Channel.SendMessageAsync("", false, builder.Build());

            ConstVariables.logger.Info($"is group 'admin' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}'");
        }

        [Command("debug")]
        [OwnerOnly]
        public async Task DebugAsync()
        {
            try
            {
                await Context.Message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            ConstVariables.CServer[Context.Guild.Id].Debug = !ConstVariables.CServer[Context.Guild.Id].Debug;

            await ReplyAndDeleteAsync($"Режим дебага: {ConstVariables.CServer[Context.Guild.Id].Debug}", timeout: TimeSpan.FromSeconds(5));

            ConstVariables.logger.Info($"is group 'Admin' is command 'debug' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is result '{(ConstVariables.CServer[Context.Guild.Id].Debug ? "on" : "off")}'");
        }

        [Command("banlist")]
        public async Task BanListAsync(IUser User = null)
        {
            if (!(await Access("banlist")))
                return;

            string SqlExpression = "sp_SelectBanList";
            string TextRequest = "";

            EmbedBuilder builder = new EmbedBuilder();

            if (User != null)
                SqlExpression = "sp_SelectBanListAnd";

            builder.WithTitle($"Бан лист{(User != null? "(Выборка)":"")}");

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                try
                {
                    conect.Open();
                    using (SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        if (User != null)
                        {
                            SqlParameter BannedParam = new SqlParameter()
                            {
                                ParameterName = "@BannedId",
                                DbType = System.Data.DbType.Int64,
                                Value = User
                            };

                            command.Parameters.Add(BannedParam);
                        }

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

                                TextRequest += $"**Пользователь:** {(banned == null ? "----" : banned.Mention)}\r\n**Администратор:** {(admin == null ? "----" : admin.Mention)}\r\nЗаметка: {Comment}\r\nДата: {Date}\r\n";
                            }

                            builder.WithDescription(TextRequest)
                                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                                .WithColor(ConstVariables.AdminColor);

                            await ReplyAsync("", embed: builder.Build());
                        }
                        else await ReplyAndDeleteAsync("Данных в базе не обнаружено!", timeout: TimeSpan.FromSeconds(5));

                        ConstVariables.logger.Info($"is group 'admin' is command 'BanList' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'");
                    }
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is group 'admin' is command 'BanList' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                    await ReplyAndDeleteAsync("Произошла ошибка! Обратитесь к администратору.", timeout: TimeSpan.FromSeconds(5));
                }
            }            
        }

        [Command("banlistadmin")]
        public async Task BanListAdmAsync(IUser User)
        {
            if (!(await Access("banlistadmin")))
                return;

            string SqlExpression = "sp_SelectBanListAdmin";
            string TextRequest = "";

            SqlParameter GuildIdParam = new SqlParameter()
            {
                ParameterName = "@GuildId",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            SqlParameter AdminIdParam = new SqlParameter()
            {
                ParameterName = "@AdminId",
                DbType = System.Data.DbType.Int64,
                Value = User.Id
            };

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Бан лист(Выборка)");

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure };

                    command.Parameters.Add(GuildIdParam);
                    command.Parameters.Add(AdminIdParam);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ulong BannedId = (ulong)reader.GetInt64(0);
                            ulong AdminId = (ulong)reader.GetInt64(1);
                            string Comment = reader.GetString(2);
                            DateTime Date = reader.GetDateTime(3);

                            TextRequest += $"**Пользователь:** {Context.Guild.GetUser(BannedId).Mention} **Администратор:** {Context.Guild.GetUser(AdminId).Mention}\r\n Заметка: {Comment}\r\n Дата: {Date}";
                        }

                        builder.WithDescription(TextRequest)
                            .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                            .WithColor(ConstVariables.AdminColor);

                        await ReplyAsync("", embed: builder.Build());
                    }
                    else await ReplyAndDeleteAsync("Данных в базе не обноружено!", timeout: TimeSpan.FromSeconds(5));

                    ConstVariables.logger.Info($"is group 'admin' is command 'BanListAdmin' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is Admin '{User.Username}#{User.Discriminator}'");
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is group 'admin' is command 'BanListAdmin' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                }
            }
        }

        [Command("banlistadd")]
        public async Task BanListAsync(IUser User, [Remainder]string comment = null)
        {
            if (!(await Access("banlistadd")))
                return;

            Stopwatch sw = Stopwatch.StartNew();
            EmbedBuilder builder = new EmbedBuilder();
            string SqlExpression = "sp_InsertUsersBan";

            if(Context.User.Id == User.Id)
            {
                await ReplyAndDeleteAsync("Банить самого себя нельзя! Но я очень хочу на это посмотреть!", timeout: TimeSpan.FromSeconds(5));
                return;
            }

            if(User.IsBot)
            {
                switch (Context.Guild.Id)
                {
                    //шароновский легион
                    case 461284473799966730:
                        {
                            await ReplyAndDeleteAsync($"Ботов банить нельзя! Но если вы удалите Monika, то я буду не против", timeout: TimeSpan.FromSeconds(5));
                            return;
                        }
                    //[Legion Sharon'a]
                    case 423154703354822668:
                        {
                            await ReplyAndDeleteAsync($"Ботов банить нельзя! Но если вы удалите Nadeko, то я буду не против", timeout: TimeSpan.FromSeconds(5));
                            return;
                        }

                    default:
                        {
                            await ReplyAndDeleteAsync($"Ботов банить нельзя!", timeout: TimeSpan.FromSeconds(5));
                            return;
                        }
                }
            }

            builder.WithTitle("Лист бана")
                .WithDescription("Добавлен новый пользователь")
                .AddField("Пользователь", User.Mention, true)
                .AddField("Администратор", Context.User.Mention, true)
                .WithColor(ConstVariables.AdminColor);

            if (comment != null)
            {
                builder.AddField("Заметка", comment);
            }
            else comment = "Отсутствует";

            SqlParameter GuildParam = new SqlParameter
            {
                ParameterName = "@GuildId",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            SqlParameter BanedParam = new SqlParameter
            {
                ParameterName = "@BanedId",
                DbType = System.Data.DbType.Int64,
                Value = User.Id
            };

            SqlParameter AdminParam = new SqlParameter
            {
                ParameterName = "@AdminId",
                DbType = System.Data.DbType.Int64,
                Value = Context.User.Id
            };

            SqlParameter CommentParam = new SqlParameter
            {
                ParameterName = "@Comment",
                DbType = System.Data.DbType.String,
                Value = comment
            };

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure };

                    command.Parameters.Add(GuildParam);
                    command.Parameters.Add(BanedParam);
                    command.Parameters.Add(AdminParam);
                    command.Parameters.Add(CommentParam);

                    int number = command.ExecuteNonQuery();
                    sw.Stop();
                    if(ConstVariables.ThisTest)
                        builder.AddField("time", sw.Elapsed);

                    await ReplyAsync("", embed: builder.Build());

                    ConstVariables.logger.Info($"is group 'admin' is command 'BanListAdd' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is UserBanned '{User.Username}#{User.Discriminator}'");
                }
                catch (Exception e)
                {
                    sw.Stop();
                    ConstVariables.logger.Error($"is group 'admin' is command 'BanListAdd' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                    await ReplyAndDeleteAsync("Ошибка вставки! Обратитесь к администратору!", timeout: TimeSpan.FromSeconds(5));
                }
            }
        }

        [Command("banlistdelete")]
        public async Task DeleteUSersListAsync(IUser User)
        {
            if (!(await Access("banlistdelete")))
                return;

            string SqlExpression = "sp_DeletUsersBan";

            SqlParameter GuildIdParam = new SqlParameter
            {
                ParameterName = "@GuildId",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            SqlParameter BanedIdParam = new SqlParameter
            {
                ParameterName = "@BanedId",
                DbType = System.Data.DbType.Int64,
                Value = User.Id
            };

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure};

                    command.Parameters.Add(GuildIdParam);
                    command.Parameters.Add(BanedIdParam);

                    int number = command.ExecuteNonQuery();
                    await ReplyAsync($"Удалено записей: {number}");

                    ConstVariables.logger.Info($"is group 'admin' is command 'BanListDelete' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is userdelete '{User.Username}#{User.Discriminator}'");
                }
                catch (Exception e)
                {
                    await ReplyAndDeleteAsync("Ошибка вставки! Обратитесь к администратору!", timeout: TimeSpan.FromSeconds(5));
                    ConstVariables.logger.Error($"is group 'admin' is command 'BanListDelete' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                }
            }
        }
        
        [Command("flowcontrol")]
        [CategoryChannel(IC: true)]
        public async Task FlowControlAsync(int name = 0)
        {
            if (!(await Access("flowcontrol")))
                return;

            switch (name)
            {
                case 0:
                    {
                        await ReplyAndDeleteAsync($"Статус функции: {ConstVariables.ControlFlow}", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }

                case 1:
                    {
                        ConstVariables.ControlFlow = true;
                        await ReplyAndDeleteAsync($"Установлен на true", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }

                case 2:
                    {
                        ConstVariables.ControlFlow = false;
                        await ReplyAndDeleteAsync($"Установлен на false", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }

                default:
                    {
                        await ReplyAndDeleteAsync($"не установленно значение", timeout: TimeSpan.FromSeconds(5));
                        break;
                    }
            }
            bool flag = false;
            string error = "";

            try
            {
                await Context.Message.DeleteAsync();
            }
            catch(Exception e)
            {
                flag = true;
                error = e.Message;
            }

            ConstVariables.logger.Info($"is group 'admin' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}' is status '{ConstVariables.ControlFlow}' {(flag? $"is error: '{error}'" : "")}");
        }
    }
}
