using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Windows.Forms;
using LegionKun.Module;
using Discord.Rest;
using System.Threading;

/*461284473799966730 - шароновский легион
 *423154703354822668 - [Legion Sharon'a]
 *435485527156981770 - Disboard*/

/*?) Доделать игру крестики-нолики
 *1) Переработать вывод бота
 *2) идея о общем классе наследнике для всех Command*/

namespace LegionKun
{
    public class Program : Module.MainClass
    {
        static void Main(string[] args)
        {
            ConstVariables.SetDelegate(new Program().Messege);

            ConstVariables.InstallationLists();

            new Program().MainTime();

            new Program().Youtube();

            try
            {
                new Program().RunBotAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error(e.Message);
            }

            Console.ReadKey();
        }

        private async Task RunBotAsync()
        {
            Module.ConstVariables._Client.Log += Log;

            Module.ConstVariables._Client.MessageReceived += MessageRec;

            Module.ConstVariables._Client.UserJoined += Userjoin;

            Module.ConstVariables._Client.UserBanned += UserBaned;

            Module.ConstVariables._Client.UserUnbanned += UserUnbaned;

            Module.ConstVariables._Client.UserLeft += UserLeft;

            Module.ConstVariables._Client.ReactionAdded += AddReaction;

            Module.ConstVariables._Client.MessageUpdated += MessageUpdate;

            Module.ConstVariables._Client.Ready += BotReady;

            Module.ConstVariables._Client.Disconnected += BotDisconnected;

            Module.ConstVariables._Client.Connected += BotConnected;

            await Module.ConstVariables._Client.SetGameAsync("sh!help");

            await Module.ConstVariables._Client.SetStatusAsync(UserStatus.Idle);

            await RegistredCommandsAsync();

            if (!Module.ConstVariables.ThisTest)
            {
                Console.WriteLine("------------!Рабочий бот!--------------");

                Console.WriteLine($"Версия: {Base.Resource1.VersionBot}");

                await Module.ConstVariables._Client.LoginAsync(TokenType.Bot, Base.Resource1.TokenBot);
            }
            else
            {
                Console.WriteLine("------------!Тестовый бот!--------------");

                Console.WriteLine($"Версия: {Base.Resource1.VersionBot}");

                await Module.ConstVariables._Client.LoginAsync(TokenType.Bot, Base.Resource1.TestBotToken);
            }

            await Module.ConstVariables._Client.StartAsync();

            await Task.Delay(-1);
        }

        private Task BotConnected()
        {
            Messege("Bot: Connected!");

            ConstVariables.logger.Info("Bot: Connected!");

            throw new NotImplementedException();
        }

        private Task BotDisconnected(Exception arg)
        {
            Messege(" Bot: Disconnected!");

            ConstVariables.logger.Error("Bot: Disconnected!");

            return Task.CompletedTask;
        }

        private Task BotReady()
        {
            Messege("Bot: ready!");

            ConstVariables.logger.Info("Bot: ready!");

            return Task.CompletedTask;
        }

        private async Task AddReaction(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var mess = await channel.GetMessageAsync(message.Id);
            IUserMessage Mess1 = mess as IUserMessage;

            if ((reaction.Emote.Name == ConstVariables.DEmoji.EDelete.Name) && (reaction.UserId == ConstVariables.CreatorId))
            {
                await mess.DeleteAsync();

                ConstVariables.logger.Info($"is grout 'automatic' is func 'AddReaction' is punkt 'delete message' is guild '--' is channel '{channel.Name}' is user '{reaction.User.Value.Username}#{reaction.User.Value.Discriminator}'");
            }
            else if (reaction.Emote.Name == ConstVariables.DEmoji.ERemuv.Name)
            {
                foreach (var react in Mess1.Reactions)
                {
                    if (react.Key.Name == ConstVariables.DEmoji.ERemuv.Name)
                        if (react.Value.ReactionCount > 1)
                            return;
                        else break;
                }

                String str = mess.Content,
                    copy = "";
                int index = 0;

                while (str[index] == '<' && (str[index + 1] == '!' || str[index + 1] == '@'))
                {
                    bool isflag = false;
                    for (; index < str.Length; index++)
                        if (str[index] == '>')
                            break;
                    index++;
                    for (; index < str.Length; index++)
                    {
                        if (str[index] == ' ' || str[index] == '?')
                            continue;
                        if (str[index] == '<' && (str[index + 1] == '!' || str[index + 1] == '@'))
                        {
                            break;
                        }
                        else
                        {
                            isflag = true;
                            break;
                        }
                    }

                    if (isflag)
                        break;
                }

                for (int i = 0; i < index; i++)
                    if (str[i] != '?')
                        copy += str[i];
                    else copy += ',';

                try
                {
                    for (; index < str.Length; index++)
                    {
                        if (str[index] == '<' && str[index + 1] == ':')
                        {
                            while (str[index] != '>')
                            {
                                copy += str[index];
                                index++;
                                if (index == str.Length)
                                    break;
                            }
                            copy += str[index];
                            index++;
                        }
                        if (index == str.Length)
                            break;
                        if (ConstVariables.Code.ContainsKey(str[index]))
                            copy += ConstVariables.Code[str[index]];
                        else copy += str[index];
                    }

                    EmbedBuilder builder = new EmbedBuilder();

                    builder.WithColor(ConstVariables.UserColor);
                    builder.WithDescription(copy);
                    builder.WithAuthor(mess.Author);
                    builder.WithTimestamp(mess.CreatedAt);

                    await channel.SendMessageAsync("", false, builder.Build());

                    ConstVariables.logger.Info($"is grout 'automatic' is func 'AddReaction' is punkt 'remuv' is guild '--' is channel '{channel.Name}' is user '{reaction.User.Value.Username}#{reaction.User.Value.Discriminator}'");
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"ошибка в переводе: {e}");
                }
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            ConstVariables.CDiscord guild = ConstVariables.CServer[user.Guild.Id];

            if (guild.EndUser == user.Id)
                return;

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithColor(ConstVariables.InfoColor);

            switch (user.Guild.Id)
            {
                //disboard
                case 435485527156981770:
                    {
                        builder.WithDescription($"Пользователь '{user.Username}#{user.Discriminator}' - покинул нас! Он не знает от чего отказывается!");
                        builder.WithTitle("Прощание");
                        break;
                    }
                //шароновский легион
                case 461284473799966730:
                    {
                        builder.WithDescription($"'{user.Username}#{user.Discriminator}' - ушёл в последний бой");
                        builder.WithTitle("Мемориал памяти воинского трибунала");
                        break;
                    }
                //[Legion Sharon'a]
                case 423154703354822668:
                    {
                        var Guild = ConstVariables.CServer[461284473799966730];
                        await Guild.GetDefaultChannel().SendMessageAsync($"С сервера [Legion Sharon'a] ушел 1 человек: '{user.Username}#{user.Discriminator}{(user.Nickname != null ? $"({user.Nickname})": "")}'");

                        builder.WithDescription($"'{user.Username}#{user.Discriminator}' - ушёл в последний бой");
                        builder.WithTitle("Мемориал памяти воинского трибунала");
                        break;
                    }
                default:
                    {
                        builder.WithDescription($"Пользователь '{user.Username}#{user.Discriminator}' - покинул нас");
                        builder.WithTitle("Прощание");
                        break;
                    }
            }

            ConstVariables.logger.Info($"is func 'UserLeft' is guild '{user.Guild.Name}' is user '{user.Username}#{user.Discriminator}'");

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private async Task UserUnbaned(SocketUser arg1, SocketGuild arg2)
        {
            var guild = Module.ConstVariables.CServer[arg2.Id];

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithFooter(arg2.Name, arg2.IconUrl)
                .WithTitle("Разбан").WithColor(Color.DarkBlue);

            if (arg1.IsBot)
            {
                builder.WithDescription($"Бот: {arg1.Mention} - разбанен");

                ConstVariables.logger.Info($"is func 'UnBanned' is Guild '{arg2.Name}#{arg2.CurrentUser.Discriminator}' is bot '{arg1.Username}#{arg1.Discriminator}'");
            }
            else
            {
                builder.WithDescription($"Пользователь: {arg1.Mention} - разбанен");

                if (guild.EndUser == arg1.Id)
                    guild.EndUser = 0;

                ConstVariables.logger.Info($"is func 'UnBanned' is Guild '{arg2.Name}' is user '{arg1.Username}#{arg1.Discriminator}'");
            }

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private async Task UserBaned(SocketUser arg1, SocketGuild arg2)
        {
            try
            {
                var guild = ConstVariables.CServer[arg2.Id];

                RestBan banned = await arg2.GetBanAsync(arg1);

                EmbedBuilder builder = new EmbedBuilder();

                Random ran = new Random();

                builder.WithFooter(arg2.Name, arg2.IconUrl)
                    .WithTitle("Бан")
                    .WithColor(ConstVariables.InfoColor);

                switch (arg2.Id)
                {    
                    //[Legion Sharon'a]
                    case 423154703354822668:
                        {
                            ConstVariables.CDiscord Guild = ConstVariables.CServer[461284473799966730];

                            SocketGuildUser user = arg2.GetUser(arg1.Id);

                            await Guild.GetDefaultChannel().SendMessageAsync($"На сервере [Legion Sharon'a] был забанен 1 человек: '{arg1.Username}#{arg1.Discriminator}{(user.Nickname != null ? $"({user.Nickname})" : "")}'");
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

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

                if (arg1.IsBot)
                {
                    builder.WithDescription($"Бот: {arg1.Mention} - изгнан")
                        .AddField("Администратор", banned.User);

                    ConstVariables.logger.Info($"is func 'Ban' is Guid '{arg2.Name}' is bot '{arg1.Username}#{arg1.Discriminator}' is Admin '{banned.User}'");
                }
                else
                {
                    builder.WithDescription($"Пользователь: {arg1.Mention} - забанен")
                        .AddField($"Причина", banned.Reason, true);

                    ConstVariables.logger.Info($"is func 'Ban' is Guild '{arg2.Name}' is user '{arg1.Username}#{arg1.Discriminator}' is Admin '{banned.User}' is Reason '{banned.Reason}'");

                    ConstVariables.CServer[arg2.Id].EndUser = arg1.Id;
                }                

                await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
            }
            catch(Exception e)
            {
                Console.WriteLine("UserBaned" + e);
            }            
        }

        private async Task Userjoin(SocketGuildUser user)
        {
            var guild = ConstVariables.CServer[user.Guild.Id];
            
            if(guild.Debug)
            {
                Messege($"Запуск");
            }

            EmbedBuilder builder = new EmbedBuilder();

            string addrole = "";

            builder.WithTitle("Добро пожаловать!").WithColor(Color.DarkBlue);

            if (guild.Debug)
            {
                Messege($"if");
            }

            if (user.IsBot)
            {
                builder.WithDescription($"{user.Mention} - это бот!");

                ConstVariables.logger.Info($" is Guild {user.Guild.Name} is bot {user.Username}#{user.Discriminator}");
            }
            else
            {
                if (guild.Debug)
                {
                    Messege($"switch");
                }

                switch (user.Guild.Id)
                {
                    case 435485527156981770:
                        {
                            //Тестер
                            var role = user.Guild.GetRole(435486930885672970);
                            if (role == null)
                            {
                                ConstVariables.logger.Error("Роль не найдена! 435486930885672970");
                                break;
                            }
                            await user.AddRoleAsync(role);
                            addrole = $" add role {role.Name}";
                            break;
                        }
                    case 461284473799966730:
                        {
                            //Житель легиона
                            var role = user.Guild.GetRole(463829025169604630);
                            if(role == null)
                            {
                                ConstVariables.logger.Error("Роль не найдена! 463829025169604630");
                                break;
                            }
                            await user.AddRoleAsync(role);
                            addrole = $" add role {role.Name}";
                            break;
                        }
                    default: { addrole = " default id:" + user.Guild.Id + " Name:" + user.Guild.Name; break; }
                }

                if (guild.Debug)
                {
                    Messege($"write");
                }

                builder.WithDescription($"{user.Mention}, добро пожаловать к нам! На сервер: {user.Guild.Name}");

                ConstVariables.logger.Info($"is func 'UserJoin' is Guild '{user.Guild.Name}' is user '{user.Username}#{user.Discriminator}'" + addrole);
            }

            if (guild.Debug)
            {
                Messege($"end");
            }

            ConstVariables.CServer[user.Guild.Id].NumberNewUser++;

            builder.WithFooter($"Вы {guild.NumberNewUser} который к нам сегодня зашел. Сегодня на сервере {user.Guild.MemberCount} пользователей", user.Guild.IconUrl);

            builder.WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/517355054341292032/hi.gif");

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private Task Log(LogMessage arg)
        {            
            ConstVariables.logger.Error(arg);

            return Task.CompletedTask;
        }

        private async Task RegistredCommandsAsync()
        {
            Module.ConstVariables._Client.MessageReceived += HandleCommandAsync;

            await Module.ConstVariables._Command.AddModuleAsync<Module.UserCommands>(Module.ConstVariables._UserService);

            await Module.ConstVariables._Command.AddModuleAsync<Module.AdminComands>(Module.ConstVariables._UserService);

            await Module.ConstVariables._Command.AddModuleAsync<Tests.TestClass>(Module.ConstVariables._UserService);

            await Module.ConstVariables._GameCommand.AddModuleAsync<Game.CrossZero.CrossZeroModule>(Module.ConstVariables._GameService);
        }
    }
}