using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System;
using System.Threading.Tasks;
using LegionKun.Attribute;
using System.Data.SqlClient;
using LegionKun.BotAPI;

namespace LegionKun.Module
{
    [Admin]
    class AdminComands : InteractiveBase
    {
        private struct SLog
        {
            string _group;
            string _command;
            SocketCommandContext Context;
            public Exception _exception;
            public string _addcondition;

            public SLog(string command, SocketCommandContext context)
            {
                _group = "Admin";
                _command = command;
                Context = context;
                _exception = null;
                _addcondition = "";
            }

            public void PrintLog()
            {
                if (_exception == null)
                    ConstVariables.Logger.Info($"is group '{_group}' is command '{_command}' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'{_addcondition}");
                else ConstVariables.Logger.Error($"is group '{_group}' is command '{_command}' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'{_addcondition} is errors {_exception}");
            }
        }

        private async Task<bool> Access(string name)
        {
            if ( name == "news" && ConstVariables.ThisTest)
            {
                await ReplyAndDeleteAsync($"{Context.User.Mention}, Эти команды недоступны в тестовом режиме!", timeout: TimeSpan.FromSeconds(5));
                return false;
            }

            if (ConstVariables.ThisTest)
            {
                await ReplyAndDeleteAsync($"{Context.User.Mention}, включен тестовый режим!", timeout: TimeSpan.FromSeconds(5));
                return true;
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

        [Command("news", RunMode = RunMode.Async)]
        public async Task NewsAsync([Remainder]string mess)
        {
            if (!(await Access("news")))
                return;

            SLog logger = new SLog("News", Context);

            string URL = "Base/news26052017.jpg";
            bool islocalfile = true;

            if(mess.IndexOf("image:") == 0)
            {
                mess = mess.Remove(0,6);
                int tchk = mess.IndexOf(';');
                URL = mess.Substring(0, tchk);
                mess = mess.Remove(0, tchk + 2);
                islocalfile = false;
            }

            var result = await Context.Channel.SendMessageAsync("Начата рассылка!\n Ждите, если это сообщение не пропадет, то напишите Sayary.");

            var serv = ConstVariables.CServer[Context.Guild.Id];

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

                        if(islocalfile)
                            await guild.GetDefaultNewsChannel().SendFileAsync(URL, " ");
                        else await guild.GetDefaultNewsChannel().SendMessageAsync(URL);

                        await guild.GetDefaultNewsChannel().SendMessageAsync(mess);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                        break;
                    }
                    else if (Context.User.Id == ConstVariables.DateBase.OwnerID)
                    {
                        if (islocalfile)
                            await guild.GetDefaultNewsChannel().SendFileAsync(URL, " ");
                        else await guild.GetDefaultNewsChannel().SendMessageAsync(URL);

                        await guild.GetDefaultNewsChannel().SendMessageAsync(mess);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                    }
                    else if(Context.Guild.Id == server.Value.GuildId)
                    {
                        if (islocalfile)
                            await guild.GetDefaultNewsChannel().SendFileAsync("Base/LegioNews2.png", " ");
                        else await guild.GetDefaultNewsChannel().SendMessageAsync(URL);

                        await guild.GetDefaultNewsChannel().SendMessageAsync(mess);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                        break;

                    }
                }
                catch (Exception e)
                {
                    logger._exception = e;
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

            logger.PrintLog();
        }

        [Command("status")]
        public async Task StatusAsync()
        {
            if (!(await Access("status")))
                return;

            SLog logger = new SLog("Status", Context);

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
                .WithColor(ConstVariables.AdminColor);

            int i = 1;
            try
            {
                using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand(SqlRequestRole, connect);
                    using (SqlDataReader reader = command.ExecuteReader())
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
                       .AddField("Версия бота", Base.Resource2.VersionBot, true)
                       .AddField("Defaul channel", Context.Guild.GetTextChannel(guild.DefaultChannelId).Mention, true)
                       .AddField("Default channel for news", Context.Guild.GetTextChannel(guild.DefaultChannelNewsId).Mention, true)
                       .AddField("Guild Id", guild.GuildId, true);

                await Context.Channel.SendMessageAsync("", false, builder.Build());

            }
            catch(Exception e)
            {
                await ReplyAsync("Ошибка чтения! Обратитесь к администратору!");
                logger._exception = e;
            }
            logger.PrintLog();
        }

        [Command("banlistadd")]
        public async Task BanListAsync(IUser User, [Remainder]string comment = null)
        {
            if (!(await Access("banlistadd")))
                return;

            SLog logger = new SLog("BanListAdd", Context);

            EmbedBuilder builder = new EmbedBuilder();
            string SqlExpression = "sp_InsertUsersBan";

            if (Context.User.Id == User.Id)
            {
                await ReplyAndDeleteAsync("Банить самого себя нельзя! Но я очень хочу на это посмотреть!", timeout: TimeSpan.FromSeconds(5));
                return;
            }

            if (User.IsBot)
            {
                switch (Context.Guild.Id)
                {
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

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
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

                    await ReplyAsync("", embed: builder.Build());

                    logger._addcondition = $"is UserBanned '{User.Username}#{User.Discriminator}'";
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка вставки! Обратитесь к администратору!");
                    logger._exception = e;
                }
                logger.PrintLog();
            }
        }

        [Command("banlistdelete")]
        public async Task DeleteUSersListAsync(IUser User)
        {
            if (!(await Access("banlistdelete")))
                return;

            SLog logger = new SLog("BanListDelete", Context);

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

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure };

                    command.Parameters.Add(GuildIdParam);
                    command.Parameters.Add(BanedIdParam);

                    int number = command.ExecuteNonQuery();
                    await ReplyAsync($"Удалено записей: {number}");

                    logger._addcondition = $"is userdelete '{User.Username}#{User.Discriminator}'";
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка вставки! Обратитесь к администратору!");
                    logger._exception = e;
                }

                logger.PrintLog();
            }
        }
        
        [Command("addtrigger")]
        [CategoryChannel(IC: true)]
        public async Task TriggerAddAsync(string trigger1, string trigger2, bool allserver = false)
        {
            if (!(await Access("addtrigger")))
                return;

            SLog logger = new SLog("AddTrigger", Context);

            string SqlExpression = "sp_AddTrigger";

            SqlParameter GuildIdParam = new SqlParameter
            {
                ParameterName = "@GuildID",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            if (Context.User.Id == ConstVariables.DateBase.OwnerID && allserver)
                GuildIdParam.Value = 0;

            SqlParameter TextRes = new SqlParameter
            {
                ParameterName = "@TextRec",
                DbType = System.Data.DbType.String,
                Value = trigger1.ToLower()
            };

            SqlParameter TextOtv = new SqlParameter
            {
                ParameterName = "@TextOtv",
                DbType = System.Data.DbType.String,
                Value = trigger2
            };

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure };

                    command.Parameters.Add(GuildIdParam);
                    command.Parameters.Add(TextOtv);
                    command.Parameters.Add(TextRes);

                    int number = command.ExecuteNonQuery();
                    await ReplyAsync((number == 0 ? $"{Context.User.Mention}, возникла ошибка при добавлении!" : "добавлена новая запись!"));
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка вставки! Обратитесь к администратору!");
                    logger._exception = e;
                }

                logger.PrintLog();
            }
        }

        [Command("selecttrigger")]
        [CategoryChannel(IC: true)]
        public async Task SelectTriggerAsync()
        {
            if (!(await Access("selecttrigger")))
                return;

            SLog logger = new SLog("SelectTrigger", Context);

            string SqlExpression = "sp_SelectTriggers";

            string answer = ConstVariables.NCR;
            int count = 1;

            SqlParameter GuildIdParam = new SqlParameter
            {
                ParameterName = "@GuildID",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                try
                {
                    conect.Open();

                    using (SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        command.Parameters.Add(GuildIdParam);

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if(reader.HasRows)
                        {
                            answer = "";
                            while(reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string textsec = reader.GetString(1);
                                string textanswer = reader.GetString(2);

                                answer += $"{count++}) **id trigger**: {id}\r\n{CountInterval(count)}**text search**: '{textsec}'\r\n{CountInterval(count)}**text anwser**: '{textanswer}'\r\n";
                            }
                            reader.Close();
                            await ReplyAsync(answer);
                        }
                        else await ReplyAsync("триггеров не найдено!");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка чтения! Обратитесь к администратору!");
                    logger._exception = e;
                }

                logger.PrintLog();
            }
        }

        [Command("deletetrigger")]
        [CategoryChannel(IC: true)]
        public async Task DeleteTriggerAsync(int id)
        {
            if (!(await Access("deletetrigger")))
                return;

            SLog logger = new SLog("DeleteTrigger", Context);

            string SqlExpression = "sp_DeleteTrigger";

            SqlParameter GuildIdParam = new SqlParameter
            {
                ParameterName = "@GuildID",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            SqlParameter IdTrigger = new SqlParameter
            {
                ParameterName = "@ID",
                DbType = System.Data.DbType.Int32,
                Value = id
            };

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                try
                {
                    conect.Open();

                    using (SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        command.Parameters.Add(GuildIdParam);
                        command.Parameters.Add(IdTrigger);

                        int result = await command.ExecuteNonQueryAsync();
                        if (result == 0)
                        {
                            await ReplyAsync("Не удален не одн триггер!");
                        }
                        else await ReplyAsync("Удален 1 триггер.");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка чтения! Обратитесь к администратору!");
                    logger._exception = e;
                }

                logger.PrintLog();
            }
        }

        [Command("updatetrigger")]
        [CategoryChannel(IC: true)]
        public async Task UpdateTriggerAsync(int id, string reupdate)
        {
            if (!(await Access("updatetrigger")))
                return;

            SLog logger = new SLog("UpdateTrigger", Context);

            string SqlExpression = "sp_UpdateTrigger";

            SqlParameter GuildIdParam = new SqlParameter
            {
                ParameterName = "@GuildID",
                DbType = System.Data.DbType.Int64,
                Value = Context.Guild.Id
            };

            SqlParameter IdTrigger = new SqlParameter
            {
                ParameterName = "@ID",
                DbType = System.Data.DbType.Int32,
                Value = id
            };

            SqlParameter ReUpdate = new SqlParameter
            {
                ParameterName = "@UpTrigger",
                DbType = System.Data.DbType.String,
                Value = reupdate
            };

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                try
                {
                    conect.Open();

                    using (SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        command.Parameters.Add(GuildIdParam);
                        command.Parameters.Add(IdTrigger);
                        command.Parameters.Add(ReUpdate);

                        int result = await command.ExecuteNonQueryAsync();
                        if (result == 0)
                        {
                            await ReplyAsync("Не обновлен не одн триггер!");
                        }
                        else await ReplyAsync("Обновлен 1 триггер.");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка чтения! Обратитесь к администратору!");
                    logger._exception = e;
                }

                logger.PrintLog();
            }
        }

        [Command("ban")]
        public async Task BanAsync(IUser user, [Remainder]string comment = null)
        {
            if(user.Id == ConstVariables.DateBase.OwnerID)
            {
                await ReplyAsync($"{Context.User.Mention}, я не собираюсь идти против своего создателя!");
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            Random ran = new Random();

            builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithTitle("Бан")
                .WithColor(ConstVariables.InfoColor);

            switch (ran.Next(0, 5))
            {
                case 0:
                    {
                        builder.WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/517352024640323584/ban_2.gif");
                        break;
                    }
                case 1:
                    {
                        builder.WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/517352068328194059/ban_3.gif?width=405&height=475");
                        break;
                    }
                case 2:
                    {
                        builder.WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/517352083226230794/ban_1.gif");
                        break;
                    }
                default:
                    {
                        builder.WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/464149984619528193/tumblr_oda2o7m3NR1tydz8to1_500.gif");
                        break;
                    }
            }


            builder.WithDescription($"Пользователь: {user.Mention} - забанен")
                .AddField($"Причина: ", (comment?? "не указанно"), true);

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
        
        [Command("flowcontrol")]
        public async Task FlowControlAsync(int name = 0)
        {
            SLog logger = new SLog("FlowControl", Context);

            switch (name)
            {
                case 0:
                    {
                        await ReplyAsync($"Статус функции: {ConstVariables.ControlFlow}");
                        break;
                    }

                case 1:
                    {
                        ConstVariables.ControlFlow = true;
                        await ReplyAsync($"Установлен на true");
                        break;
                    }

                case 2:
                    {
                        ConstVariables.ControlFlow = false;
                        await ReplyAsync($"Установлен на false");
                        break;
                    }
                default:
                    {
                        await ReplyAsync($"не установленно значение");
                        break;
                    }
            }

            logger.PrintLog();
        }

        private string CountInterval(int number)
        {
            double interv = Math.Log10(number) + 1;
            string rez = "";
            interv = Math.Truncate(interv) * 2;

            for (int i = 0; i < interv + 2; i++)
                rez += " ";

            return rez;
        }
    }
}
