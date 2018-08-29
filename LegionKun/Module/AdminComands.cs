using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System;
using System.Threading.Tasks;
using LegionKun.Attribute;
using System.Data.SqlClient;
using Discord.WebSocket;

namespace LegionKun.Module
{
    [Admin]
    class AdminComands : InteractiveBase
    {
        [Command("off", RunMode = RunMode.Async)]
        public async Task OffBotAsync(byte level = 0)
        {
            if (!ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                await ReplyAndDeleteAsync("Бот выключен!", timeout: TimeSpan.FromSeconds(5));
                return;
            }

            if (ConstVariables.ThisTest)
            {
                return;
            }

            await ConstVariables._Client.SetStatusAsync(UserStatus.DoNotDisturb);

            switch (level)
            {
                case 0:
                    {
                        EmbedBuilder builder = new EmbedBuilder();

                        builder.WithTitle("!!!Внимание!!!").WithDescription("бот будет выключен на обновление :stuck_out_tongue_winking_eye:").WithColor(Discord.Color.Magenta);

                        foreach (var server in ConstVariables.CServer)
                        {
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

            ConstVariables.CServer[Context.Guild.Id].IsOn = false;
        }

        [Command("on", RunMode = RunMode.Async)]
        public async Task OnBotAsync(byte level = 0)
        {
            if (ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                return;
            }

            if (ConstVariables.ThisTest)
            {
                return;
            }

            await ConstVariables._Client.SetStatusAsync(UserStatus.Online);

            switch (level)
            {
                case 0:
                    {

                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle("Ура").WithDescription("Я снова в строю").WithColor(Discord.Color.Magenta);

                        foreach (var server in ConstVariables.CServer)
                        {
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

            ConstVariables.CServer[Context.Guild.Id].IsOn = true;
        }

        [Command("news", RunMode = RunMode.Async)]
        public async Task NewsAsync([Remainder]string mess)
        {
            if (ConstVariables.ThisTest)
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Новости бота")
                .WithDescription(mess)
                .WithColor(ConstVariables.InfoColor)
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/464447806887690240/news26052017.jpg?width=841&height=474");

            foreach (var server in ConstVariables.CServer)
            {
                if (server.Value.DefaultChannelNewsId == 0)
                {
                    continue;
                }

                if ((Context.User.Id == ConstVariables.CreatorId))
                {
                    ConstVariables.CDiscord guild = server.Value;

                    await guild.GetDefaultNewsChannel().SendMessageAsync("", false, builder.Build());
                }
                else
                {
                    if (Context.Guild.Id == server.Value.GuildId)
                    {
                        ConstVariables.CDiscord guild = server.Value;

                        await guild.GetDefaultNewsChannel().SendMessageAsync("", false, builder.Build());
                        break;
                    }
                }
            }

            try
            {
                await Context.Message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            ConstVariables.logger.Info($"is group 'Automatic' is command 'news' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
        }

        [Command("status")]
        public async Task StatusAsync()
        {
            try
            {
                await Context.Message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine("Ошибка доступа!");
            }

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            EmbedBuilder builder = new EmbedBuilder();

            string role = "";

            string channel = "";

            int i = 1;

            builder.WithTitle("the status of the bot")
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                .WithAuthor(Context.User.Username, Context.User.GetAvatarUrl())
                .AddField("Guild", Context.Guild.Name, true)
                .AddField("Resurs", guild.CountRes + "/" + guild.Restruction, true)
                .AddField("Is on?", ConstVariables.CServer[Context.Guild.Id].IsOn, true)
                .WithColor(ConstVariables.AdminColor);

            foreach (var rol in guild._Role)
            {
                role += $"{i}: {Context.Guild.GetRole(rol.Key)}\r\n";
                i++;
            }

            i = 1;

            foreach (var chan in guild._Channels)
            {
                channel += $"{i}: {Context.Guild.GetTextChannel(chan.Key)}\r\n";
                i++;
            }

            builder.AddField("Roles", role, true)
                .AddField("Channels", channel, true)
                .AddField("Версия бота", Base.Resource1.VersionBot, true);

            if (role != "")
                builder.AddField("Defaul channel", Context.Guild.GetTextChannel(guild.DefaultChannelId).Mention, true);
            if ((channel != "") && (guild.DefaultChannelNewsId != 0))
                builder.AddField("Default channel for news", Context.Guild.GetTextChannel(guild.DefaultChannelNewsId).Mention, true);
            builder.AddField("Guild Id", guild.GuildId, true);

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
            string SqlRequest = "SELECT TOP 20 [BannedId], [AdminId], [Comment] FROM [UserBanned].[dbo].[List_User_Banned]";
            string TextRequest = "";

            EmbedBuilder builder = new EmbedBuilder();

            if (User != null)
                SqlRequest += $" WHERE [BannedId] = {User.Id}";

            builder.WithTitle($"Бан лист{(User != null? "(Выборка)":"")}");

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKey))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlRequest, conect);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ulong BannedId = (ulong)reader.GetInt64(0);
                            ulong AdminId = (ulong)reader.GetInt64(1);
                            string Comment = reader.GetString(2);
                            
                            TextRequest += $"**Пользователь:** {Context.Guild.GetUser(BannedId).Mention} **Администратор:** {Context.Guild.GetUser(AdminId).Mention}\r\n Заметка: {Comment}\r\n";
                        }

                        builder.WithDescription(TextRequest)
                            .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                            .WithColor(ConstVariables.AdminColor);

                        await ReplyAsync("", embed: builder.Build());
                    }
                    else await ReplyAndDeleteAsync("Данных в базе не обноружено!", timeout: TimeSpan.FromSeconds(5));

                    ConstVariables.logger.Info($"is group 'admin' is command 'banlist 0' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is userbanned '{User.Username}#{User.Discriminator}'");
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is group 'admin' is command 'banlist 0' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                }
            }
        }

        [Command("banlistadmin")]
        public async Task BanListAdmAsync(IUser User = null)
        {
            string SqlRequest = "SELECT TOP 20 [BannedId], [AdminId], [Comment] FROM [UserBanned].[dbo].[List_User_Banned]";
            string TextRequest = "";

            EmbedBuilder builder = new EmbedBuilder();

            if (User != null)
                SqlRequest += $" WHERE [adminId] = {User.Id}";

            builder.WithTitle($"Бан лист{(User != null ? "(Выборка)" : "")}");

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKey))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(SqlRequest, conect);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ulong BannedId = (ulong)reader.GetInt64(0);
                            ulong AdminId = (ulong)reader.GetInt64(1);
                            string Comment = reader.GetString(2);

                            TextRequest += $"**Пользователь:** {Context.Guild.GetUser(BannedId).Mention} **Администратор:** {Context.Guild.GetUser(AdminId).Mention}\r\n Заметка: {Comment}\r\n";
                        }

                        builder.WithDescription(TextRequest)
                            .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                            .WithColor(ConstVariables.AdminColor);

                        await ReplyAsync("", embed: builder.Build());
                    }
                    else await ReplyAndDeleteAsync("Данных в базе не обноружено!", timeout: TimeSpan.FromSeconds(5));

                    ConstVariables.logger.Info($"is group 'admin' is command 'banlist 0' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is userbanned '{User.Username}#{User.Discriminator}'");
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is group 'admin' is command 'banlist 0' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                }
            }
        }

        [Command("banlistadd")]
        public async Task BanListAsync(IUser User, [Remainder]string comment = null)
        {
            EmbedBuilder builder = new EmbedBuilder();
            string Ask;

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
                builder.AddField("Комментарий", comment);

            if (comment != null)
                Ask = $"INSERT INTO List_User_Banned (BannedId, AdminId, Comment) VALUES ({User.Id}, {Context.User.Id}, '{comment}')";
            else Ask = $"INSERT INTO List_User_Banned (BannedId, AdminId, Comment) VALUES ({User.Id}, {Context.User.Id}, 'Отсутствует')";

            using (SqlConnection conect = new SqlConnection(Base.Resource1.ConnectionKey))
            {
                try
                {
                    conect.Open();
                    SqlCommand command = new SqlCommand(Ask, conect);
                    int number = command.ExecuteNonQuery();
                    await ReplyAsync("", embed: builder.Build());

                    ConstVariables.logger.Info($"is group 'admin' is command 'banlist 1' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is userbanned '{User.Username}#{User.Discriminator}'");
                }
                catch (Exception e)
                {
                    await ReplyAndDeleteAsync("Ошибка вставки! Обратитесь к администратору!", timeout: TimeSpan.FromSeconds(5));
                    ConstVariables.logger.Error($"is group 'admin' is command 'banlist 1' is guild '{Context.Guild.Name}' is command '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is errors {e}");
                }
            }
        }
        
        [Command("flowcontrol")]
        public async Task FlowControlAsync(int name = 0)
        {
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

            ConstVariables.logger.Info($"is group 'admin' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}' is status '{ConstVariables.ControlFlow}'");

        }
    }
}
