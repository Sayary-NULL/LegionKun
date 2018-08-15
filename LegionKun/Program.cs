﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Windows.Forms;
using LegionKun.Module;

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
            Module.ConstVariables.SetDelegate(new Program().Messege);

            Module.ConstVariables.InstallationLists();

            new Program().MainTime();

            new Program().Youtube();

            int i = 0;
           
            do
            {
                ConstVariables.logger.Info(i == 0 ? "Запуск программы":"Повторный запуск программы");

                i = 1;
                try
                {
                    new Program().RunBotAsync().GetAwaiter().GetResult();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }

            } while (MessageBox.Show(null, "Повторный запуск?", "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.No);
        }

        private async Task RunBotAsync()
        { 
            Module.ConstVariables._Client.Log += Log;

            Module.ConstVariables._Client.MessageReceived += MessageRec;

            Module.ConstVariables._Client.UserJoined += Userjoin;

            Module.ConstVariables._Client.UserBanned += UserBaned;

            Module.ConstVariables._Client.UserUnbanned += UserUnbaned;

            Module.ConstVariables._Client.UserUpdated += UserUp;
            
            Module.ConstVariables._Client.UserLeft += UserLeft;

            Module.ConstVariables._Client.ReactionAdded += AddReaction;

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

        private async Task AddReaction(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            if ((reaction.Emote.Name == ConstVariables.DEmoji.EDelete.Name) && (reaction.UserId == ConstVariables.CreatorId))
            {
                var mess = await channel.GetMessageAsync(message.Id);
                await mess.DeleteAsync();
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[user.Guild.Id];

            if (guild.EndUser == user.Id)
                return;

            EmbedBuilder builder = new EmbedBuilder();

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

            ConstVariables.logger.Info($" is func 'UserLeft' is user '{user.Username}#{user.Discriminator}' is guild '{user.Guild.Name}'");

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private async Task UserUp(SocketUser arg1, SocketUser arg2)
        {
            if((arg1.Username == arg2.Username) || (arg1.Id == 356145518444806144) || (arg1.IsBot))
            {
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithDescription($"Пользователь: {arg1.Username}. Сменил имя на: {arg2.Username}");
            builder.WithTitle("Пользователь обновлен");

            foreach(var guild in Module.ConstVariables._Client.Guilds)
            {
                if(guild.Id == 461284473799966730)
                {
                    continue;
                }

                if((guild.GetUser(arg2.Id) != null))
                {
                    var dguild = Module.ConstVariables.CServer[guild.Id];

                    builder.WithFooter(guild.Name, guild.IconUrl);

                    await dguild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
                }
            }

            ConstVariables.logger.Info($" is func 'UserUp' is user  1:'{arg1.Username}#{arg1.Discriminator}'; 2:'{arg2.Username}#{arg2.Discriminator}';");

        }

        private async Task UserUnbaned(SocketUser arg1, SocketGuild arg2)
        {
            var guild = Module.ConstVariables.CServer[arg2.Id];

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithFooter(arg2.Name, arg2.IconUrl);

            builder.WithTitle("Разбан").WithColor(Color.DarkBlue);

            if (arg1.IsBot)
            {
                builder.WithDescription($"Бот: {arg1.Mention} - разбанен");

                ConstVariables.logger.Info($" is func 'UnBanned' is Guild '{arg2.Name}#{arg2.CurrentUser.Discriminator}' is bot '{arg1.Username}#{arg1.Discriminator}'");
            }
            else
            {
                builder.WithDescription($"Пользователь: {arg1.Mention} - разбанен");

                ConstVariables.logger.Info($" is func 'UnBanned' is Guild '{arg2.Name}#{arg2.CurrentUser.Discriminator}' is user '{arg1.Username}#{arg1.Discriminator}'");

                if (guild.EndUser == arg1.Id)
                    guild.EndUser = 0;
            }

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private async Task UserBaned(SocketUser arg1, SocketGuild arg2)
        {
            var guild = Module.ConstVariables.CServer[arg2.Id];

            EmbedBuilder builder = new EmbedBuilder();

            Random ran = new Random();

            string url = "";

            builder.WithFooter(arg2.Name, arg2.IconUrl);

            builder.WithTitle("Бан").WithColor(Color.DarkBlue);

            if (arg1.IsBot)
            {
                builder.WithDescription($"Бот: {arg1.Mention} - изгнан");

                ConstVariables.logger.Info($" is func 'Ban' is Guid '{arg2.Name}' is bot '{arg1.Username}#{arg1.Discriminator}'");
            }
            else
            {
                builder.WithDescription($"Пользователь: {arg1.Mention} - забанен");

                ConstVariables.logger.Info($" is func 'Ban' is Guild '{arg2.Name}' is user '{arg1.Username}#{arg1.Discriminator}'");

                guild.EndUser = arg1.Id;
            }

            switch (ran.Next(0, 2))
            {
                case 0: { url = "https://cdn.discordapp.com/attachments/462236317926031370/462236494459961344/Katyusha_2.gif "; break; }
                case 1: { url = "https://cdn.discordapp.com/attachments/462236317926031370/462236553771483137/KspISzf.gif"; break; }
                default: { url = ""; break; }
            }

            if (url != "")
            {
                builder.WithImageUrl(url);
            }

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private async Task Userjoin(SocketGuildUser user)
        {
            var guild = Module.ConstVariables.CServer[user.Guild.Id];

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
                                await guild.GetDefaultChannel().SendMessageAsync("Такой роли не существет!");
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
                                await guild.GetDefaultChannel().SendMessageAsync("Такой роли не существет!");
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

                ConstVariables.logger.Info($" is func 'UserJoin' is Guild '{user.Guild.Name}' is user '{user.Username}#{user.Discriminator}' " + addrole);
            }

            if (guild.Debug)
            {
                Messege($"end");
            }

            ConstVariables.CServer[user.Guild.Id].NumberNewUser++;

            builder.WithFooter($" Вы {guild.NumberNewUser} который к нам сегодня зашел, сегодня на сервере {user.Guild.MemberCount}", user.Guild.IconUrl);

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            if (arg.Message != "")
                ConstVariables.logger.Info(arg.Message);
            else ConstVariables.logger.Info(arg.Exception);

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

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage Messeg = arg as SocketUserMessage;

            if (Messeg is null || Messeg.Author.IsBot)
                return;
            int argPos = 0;
            
            if (Messeg.HasStringPrefix("sh!", ref argPos) || Messeg.HasStringPrefix("Sh!", ref argPos))
            {
                SocketCommandContext contex = new SocketCommandContext(Module.ConstVariables._Client, Messeg);
                
                IResult result = await Module.ConstVariables._Command.ExecuteAsync(contex, argPos, Module.ConstVariables._UserService);

                if (!result.IsSuccess)
                {
                    switch (result.Error.Value)
                    {
                        case CommandError.BadArgCount: { ConstVariables.logger.Error($" is errors 'BadArgCount' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); contex.Channel.SendMessageAsync($"{contex.User.Mention}, неверные параметры команды").GetAwaiter(); break; }
                        case CommandError.UnknownCommand: { ConstVariables.logger.Error($" is errors 'UnknownCommand' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); contex.Channel.SendMessageAsync($"{contex.User.Mention}, неизвестная команда").GetAwaiter(); Help(contex); break; }
                        case CommandError.ObjectNotFound: { ConstVariables.logger.Error($" is errors 'ObjectNotFound' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.ParseFailed: { ConstVariables.logger.Error($" is errors 'ParseFailed' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.MultipleMatches: { ConstVariables.logger.Error($" is errors 'MultipleMatches' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.UnmetPrecondition: { ConstVariables.logger.Error($" is errors 'UnmetPrecondition' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.Exception: { ConstVariables.logger.Error($" is errors 'Exception' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.Unsuccessful: { ConstVariables.logger.Error($" is errors 'Unsuccessful' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        default: { ConstVariables.logger.Error($" is errors 'Default' is channel '{arg.Channel.Name}' is param {result.ErrorReason} is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                    }
                }
            }
            else if(Messeg.HasStringPrefix("c!", ref argPos) || Messeg.HasStringPrefix("с!", ref argPos))
            {
                SocketCommandContext contex = new SocketCommandContext(Module.ConstVariables._Client, Messeg);

                IResult result = await Module.ConstVariables._GameCommand.ExecuteAsync(contex, argPos, Module.ConstVariables._GameService);

                if (!result.IsSuccess)
                {
                    switch (result.Error.Value)
                    {
                        case CommandError.BadArgCount: { ConstVariables.logger.Error($" is errors 'BadArgCount' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); contex.Channel.SendMessageAsync($"{contex.User.Mention}, неверные параметры команды").GetAwaiter(); break; }
                        case CommandError.UnknownCommand: { ConstVariables.logger.Error($" is errors 'UnknownCommand' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); contex.Channel.SendMessageAsync($"{contex.User.Mention}, неизвестная команда").GetAwaiter(); break; }
                        case CommandError.ObjectNotFound: { ConstVariables.logger.Error($" is errors 'ObjectNotFound' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.ParseFailed: { ConstVariables.logger.Error($" is errors 'ParseFailed' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.MultipleMatches: { ConstVariables.logger.Error($" is errors 'MultipleMatches' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.UnmetPrecondition: { ConstVariables.logger.Error($" is errors 'UnmetPrecondition' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.Exception: { ConstVariables.logger.Error($" is errors 'Exception' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        case CommandError.Unsuccessful: { ConstVariables.logger.Error($" is errors 'Unsuccessful' is param {result.ErrorReason} is channel '{arg.Channel.Name}' is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                        default: { ConstVariables.logger.Error($" is errors 'Default' is channel '{arg.Channel.Name}' is param {result.ErrorReason} is user '{arg.Author.Username}#{arg.Author.Discriminator}' is context '{arg.Content}'"); break; }
                    }
                }
            }
        }

        private void Help(SocketCommandContext Context)
        {
            if (!Module.ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                if (!Module.ConstVariables.ThisTest)
                    return;
            }

            if (!Module.ConstVariables.UserCommand[6].IsOn)
            {
                return;
            }

            SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            if(Context.User.Id == guild.DefaultCommandChannel)
            {
                return;
            }

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

            ConstVariables.logger.Info($" is Guid {Context.Guild.Name} is command 'help' is user '{user.Username}' is channel '{Context.Channel.Name}' is IsRole '{IsRole}' ");

            Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}