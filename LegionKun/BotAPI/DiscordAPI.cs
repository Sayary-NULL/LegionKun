using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionKun.Module;
using Discord.Rest;
using Microsoft.Extensions.DependencyInjection;
using Discord.Addons.Interactive;

namespace LegionKun.BotAPI
{
    public class DiscordAPI
    {
        public static DiscordSocketClient _Client { get; set; }
        public static CommandService _Command { get; set; }
        //public static CommandService _GameCommand { get; set; }
        public static IServiceProvider _UserService { get; set; }
        //public static IServiceProvider _GameService { get; set; }

        public static ConstVariables.DMessege Messege = null;

        public DiscordAPI()
        {
            var _config = new DiscordSocketConfig();
            _config.LogLevel = LogSeverity.Info;

            _Client = new DiscordSocketClient(_config);
            _Command = new CommandService();

            _UserService = new ServiceCollection().AddSingleton(_Client).AddSingleton(_Command).AddSingleton<InteractiveService>().BuildServiceProvider();

            _Client.Log += Log;

            _Client.MessageReceived += MessageRec;

            _Client.UserJoined += Userjoin;

            _Client.UserBanned += UserBaned;

            _Client.UserUnbanned += UserUnbaned;

            _Client.UserLeft += UserLeft;

            _Client.ReactionAdded += AddReaction;

            _Client.MessageUpdated += MessageUpdate;

            _Client.Ready += BotReady;

            _Client.Disconnected += BotDisconnected;

            _Client.Connected += BotConnected;
        }

        private void Help(SocketCommandContext Context)
        {
            if (!ConstVariables.CServer[Context.Guild.Id].IsOn && !ConstVariables.ThisTest)
                return;

            SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            if (!guild.IsEntryOrСategoryChannel(Context.Channel.Id, IsCommand: true))
                return;

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild.EntryRole(role.Id))
                {
                    IsRole = true;
                    break;
                }

            EmbedBuilder builder = new EmbedBuilder();

            string TitlList = "Префикс команд для бота 'sh!' или 'Sh!'";

            builder.AddField("Параметры", "[] - обязательно \r\n<> - не обязательно")
                .WithTitle(TitlList)
                .AddField("Group: Default", ConstVariables.UTHelp, true);

            if (IsRole)
                builder.AddField("Group: Admin", ConstVariables.ATHelp, true);

            builder.WithColor(Color.Orange)
                .WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

            ConstVariables.logger.Info($"is Guid '{Context.Guild.Name}' is command 'help' is user '{user.Username}' is channel '{Context.Channel.Name}' is IsRole '{IsRole}'");

            Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        private struct SLog
        {
            string _error;
            string _errorrreason;
            SocketCommandContext Context;

            public SLog(SocketCommandContext context)
            {
                _error = "";
                _errorrreason = "";
                Context = context;
            }

            public void SetParam(string error, string errorrreason)
            {
                _error = error;
                _errorrreason = errorrreason;
            }

            public void PrintLog()
            {
                ConstVariables.logger.Error($"is errors '{_error}' is param {_errorrreason} is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}' is context '{Context.Message.Content}'");
            }
        };

        public async void RunBotAsync()
        {
            await _Client.SetGameAsync("sh!help");

            await _Client.SetStatusAsync(UserStatus.Idle);

            await RegistredCommandsAsync();

            if (!Module.ConstVariables.ThisTest)
            {
                Console.WriteLine("------------!Рабочий бот!--------------");

                Console.WriteLine($"Версия: {Base.Resource2.VersionBot}");

                await _Client.LoginAsync(TokenType.Bot, Base.Resource2.TokenBot);
            
            }
            else
            {
                Console.WriteLine("------------!Тестовый бот!--------------");

                Console.WriteLine($"Версия: {Base.Resource2.VersionBot}");

                await _Client.LoginAsync(TokenType.Bot, Base.Resource2.TestBotToken);
            }

            await _Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task RegistredCommandsAsync()
        {
            _Client.MessageReceived += HandleCommandAsync;

            await _Command.AddModuleAsync<Module.UserCommands>(_UserService);

            await _Command.AddModuleAsync<Module.AdminComands>(_UserService);

            await _Command.AddModuleAsync<Tests.TestClass>(_UserService);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage Messeg = arg as SocketUserMessage;

            if (Messeg is null || Messeg.Author.IsBot)
                return;
            int argPos = 0;

            SocketCommandContext contex = new SocketCommandContext(_Client, Messeg);

            SLog logger = new SLog(contex);

            IResult result = null;

            if (Messeg.HasStringPrefix("sh!", ref argPos) || Messeg.HasStringPrefix("Sh!", ref argPos))
            {
                result = await _Command.ExecuteAsync(contex, argPos, _UserService);
            }

            if (result == null)
                return;

            if (!result.IsSuccess)
            {
                switch (result.Error.Value)
                {
                    case CommandError.BadArgCount: { logger.SetParam("BadArgCount", result.ErrorReason); contex.Channel.SendMessageAsync($"{contex.User.Mention}, неверные параметры команды").GetAwaiter(); break; }
                    case CommandError.UnknownCommand: { logger.SetParam("UnknownCommand", result.ErrorReason); contex.Channel.SendMessageAsync($"{contex.User.Mention}, неизвестная команда").GetAwaiter(); Help(contex); break; }
                    case CommandError.ObjectNotFound: { logger.SetParam("ObjectNotFound", result.ErrorReason); break; }
                    case CommandError.ParseFailed: { logger.SetParam("ParseFailed", result.ErrorReason); break; }
                    case CommandError.MultipleMatches: { logger.SetParam("MultipleMatches", result.ErrorReason); break; }
                    case CommandError.UnmetPrecondition: { logger.SetParam("UnmetPrecondition", result.ErrorReason); contex.Channel.SendMessageAsync(result.ErrorReason).GetAwaiter(); break; }
                    case CommandError.Exception: { logger.SetParam("Exception", result.ErrorReason); break; }
                    case CommandError.Unsuccessful: { logger.SetParam("Unsuccessful", result.ErrorReason); break; }
                    default: { logger.SetParam("Default", result.ErrorReason); break; }
                }

                logger.PrintLog();
            }
        }

        private Task BotConnected()
        {
            Messege("Bot: Connected!");

            ConstVariables.logger.Info("Bot: Connected!");

            return Task.CompletedTask;
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

            IUserMessage Mess1 = await message.GetOrDownloadAsync();

            if ((reaction.Emote.Name == ConstVariables.DEmoji.EDelete.Name) && (reaction.UserId == ConstVariables.CreatorId))
            {
                await Mess1.DeleteAsync();

                ConstVariables.logger.Info($"is grout 'automatic' is func 'AddReaction' is punkt 'delete message' is guild '{(channel as SocketGuildChannel)?.Guild.Name}' is channel '{channel.Name}' is user '{reaction.User.Value.Username}#{reaction.User.Value.Discriminator}'");
            }
            else if (reaction.Emote.Name == ConstVariables.DEmoji.ERemuv.Name)
            {
                try
                {
                    foreach (var react in Mess1.Reactions)
                    {
                        if (react.Key.Name == ConstVariables.DEmoji.ERemuv.Name)
                            if (react.Value.ReactionCount > 1)
                                return;
                            else break;
                    }

                    String str = Mess1.Content,
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
                    builder.WithAuthor(Mess1.Author);
                    builder.WithTimestamp(Mess1.CreatedAt);

                    await channel.SendMessageAsync("", false, builder.Build());

                    ConstVariables.logger.Info($"is grout 'automatic' is func 'AddReaction' is punkt 'remuv' is guild '{(channel as SocketGuildChannel)?.Guild.Name}' is channel '{channel.Name}' is user '{reaction.User.Value.Username}#{reaction.User.Value.Discriminator}'");
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error($"is grout 'automatic' is func 'AddReaction' is punkt 'remuv' is guild '{(channel as SocketGuildChannel)?.Guild.Name}' is channel '{channel.Name}' is user '{reaction.User.Value.Username}#{reaction.User.Value.Discriminator}' is error '{e}'");
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


            builder.WithDescription($"Пользователь: {arg1.Mention} - разбанен");

            if (guild.EndUser == arg1.Id)
                guild.EndUser = 0;

            ConstVariables.logger.Info($"is func 'UnBanned' is Guild '{arg2.Name}' is user '{arg1.Username}#{arg1.Discriminator}'");


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


                builder.WithDescription($"Пользователь: {arg1.Mention} - забанен")
                    .AddField($"Причина", banned.Reason ?? "неуказанно", true);

                ConstVariables.CServer[arg2.Id].EndUser = arg1.Id;

                await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                ConstVariables.logger.Info($"is func 'Ban' is Guild '{arg2.Name}' is user '{arg1.Username}#{arg1.Discriminator}' is Admin '{banned.User}' is Reason '{banned.Reason}'");
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error($"is func 'Ban' is Guild '{arg2.Name}' is user '{arg1.Username}#{arg1.Discriminator}' is error '{e}'");
            }
        }

        private async Task Userjoin(SocketGuildUser user)
        {
            var guild = ConstVariables.CServer[user.Guild.Id];

            EmbedBuilder builder = new EmbedBuilder();

            string addrole = "";

            builder.WithTitle("Добро пожаловать!").WithColor(Color.DarkBlue);

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
                        if (role == null)
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

            ConstVariables.CServer[user.Guild.Id].NumberNewUser++;

            builder.WithDescription($"{user.Mention}, добро пожаловать к нам! На сервер: {user.Guild.Name}")
                .WithFooter($"Вы {guild.NumberNewUser} который к нам сегодня зашел. Сегодня на сервере {user.Guild.MemberCount} пользователей", user.Guild.IconUrl)
                .WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/517355054341292032/hi.gif");

            await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

            ConstVariables.logger.Info($"is func 'UserJoin' is Guild '{user.Guild.Name}' is user '{user.Username}#{user.Discriminator}'" + addrole);
        }

        private Task Log(LogMessage arg)
        {
            ConstVariables.logger.Error(arg);

            return Task.CompletedTask;
        }
        
        private async Task MessageUpdate(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            var guild = (arg3 as SocketGuildChannel)?.Guild;

            if (arg2.Author.IsBot || arg2.Author.Id == 0)
                return;

            try
            {
                var mess = await arg1.GetOrDownloadAsync();

                if (mess == null)
                    return;

                if (mess.CreatedAt.Year == mess.EditedTimestamp.Value.Year && mess.CreatedAt.Month == mess.EditedTimestamp.Value.Month && mess.CreatedAt.Day == mess.EditedTimestamp.Value.Day && mess.CreatedAt.Hour == mess.EditedTimestamp.Value.Hour && Math.Abs(mess.CreatedAt.Minute - mess.EditedTimestamp.Value.Minute) < 5)
                {
                    await MessageRec(arg2);
                    await HandleCommandAsync(arg2);
                    ConstVariables.logger.Info($"is func 'MessageUpdate' is guild '{(guild == null ? guild.Name : "Not")}' is chanel '{arg2.Channel.Name}' is user '{arg2.Author.Username}#{arg2.Author.Discriminator}' is message '{arg2.Content}'");
                }
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error($"is func 'MessageUpdate' is guild '{(guild == null ? guild.Name : "Not")}' is chanel '{arg2.Channel.Name}' is user '{arg2.Author.Username}#{arg2.Author.Discriminator}' is message '{arg2.Content}' is errors '{e.Message}'");
            }
        }

        private async Task MessageRec(SocketMessage Messag)
        {
            string mess = Messag.Content.ToLower();

            SocketUserMessage Messege = Messag as SocketUserMessage;

            SocketCommandContext Context = new SocketCommandContext(_Client, Messege);

            if (!ConstVariables.CServer[Context.Guild.Id].IsOn && !ConstVariables.ThisTest || Messag.Author.IsBot)
                return;

            bool IsTrigger = false;

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            ISocketMessageChannel channel = Context.Channel;

            if ((guild.IsEntryOrСategoryChannel(channel.Id) && !Context.User.IsBot) || ConstVariables.ThisTest)
            {
                int count = ConstVariables.CountTextRequst(mess, Context.Guild.Id);

                if (count > 0)
                {
                    string str = ConstVariables.TextRequst(mess, Context.Guild.Id);

                    if (str != ConstVariables.NCR)
                    {
                        if (str.IndexOf("{0}") >= 0)
                            str = String.Format(str, Context.User.Mention);

                        await channel.SendMessageAsync(str);

                        IsTrigger = true;
                    }
                }

                ConstVariables.ResultIndexOfText result = ConstVariables.IndexOfText(mess);

                if (result.isSearch && (result.GuildID == Context.Guild.Id || result.GuildID == 0 || ConstVariables.ThisTest))
                {
                    if (result.Condition == "{0}")
                    {
                        if (result.TextAnswer.IndexOf("{0}") >= 0)
                            result.TextAnswer = String.Format(result.TextAnswer, Context.User.Mention);

                        await channel.SendMessageAsync(result.TextAnswer);
                    }
                    else
                    {
                        bool IsCondition = false;
                        bool IsParam1 = false;
                        if (result.Condition.IndexOf("{1}") > -1)
                        {
                            if ((guild.CountRes < guild.Restruction) && (!ConstVariables.CServer[Context.Guild.Id].Trigger))
                                IsCondition = true;

                            IsParam1 = true;
                        }
                        else IsCondition = true;

                        if (result.Condition.IndexOf("{2}") > -1)
                        {
                            if (mess.IndexOf("бот") == 0 || MentionUser(Context, _Client.CurrentUser.Id))
                                IsCondition &= true;
                            else IsCondition = false;
                        }
                        else IsCondition &= true;

                        if (IsCondition)
                        {
                            if (result.TextAnswer.IndexOf("{0}") >= 0)
                                result.TextAnswer = String.Format(result.TextAnswer, Context.User.Mention);

                            await channel.SendMessageAsync(result.TextAnswer);

                            if (IsParam1)
                            {
                                guild.CountRes++;
                                ConstVariables.CServer[Context.Guild.Id].Trigger = true;
                                new Program().OneMin(Context.Guild.Id);
                            }
                        }
                    }

                    IsTrigger = true;
                }

                if (IsTrigger)
                {
                    ConstVariables.logger.Info($" Сработал тригер: '{mess}'! is Guid '{guild.GetGuild().Name}' is channel '{channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'");
                }
            }
        }
        
        private bool MentionUser(ICommandContext Context, params ulong[] Id)
        {
            foreach (var key in Context.Message.MentionedUserIds)
                for (int i = 0; i < Id.Length; i++)
                    if (Id[i] == key)
                        return true;

            return false;
        }
      
        public void SetDelegate(ConstVariables.DMessege dMessege)
        {
            Messege = dMessege;
        }
    }
}
