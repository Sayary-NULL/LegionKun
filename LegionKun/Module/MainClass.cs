﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LegionKun.Module
{
    public class MainClass
    {
        private readonly ThreadClass threadclass = new ThreadClass();

        public void OneMin(ulong guildId) => threadclass.OneMinStart(guildId);        

        public void MainTime() => threadclass.MainTimerStart();        

        public void Youtube() => threadclass.YoutubeStart();

        protected async Task MessageUpdate(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            try
            {
                var mess = await arg1.GetOrDownloadAsync();

                if( mess.CreatedAt.Year == mess.EditedTimestamp.Value.Year && mess.CreatedAt.Month == mess.EditedTimestamp.Value.Month && mess.CreatedAt.Day == mess.EditedTimestamp.Value.Day && mess.CreatedAt.Hour == mess.EditedTimestamp.Value.Hour && Math.Abs(mess.CreatedAt.Minute - mess.EditedTimestamp.Value.Minute) < 5)
                {
                    await MessageRec(arg2);
                    await HandleCommandAsync(arg2);
                    ConstVariables.logger.Info($"update message: message '{arg2.Content}' is chanel '{arg2.Channel.Name}' is user '{arg2.Author.Mention}'");
                }
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error($"is chanel '{arg2.Channel.Name}' is user '{arg2.Author.Mention}' is errors '{e.Message}'");
            }            
        }

        protected async Task MessageRec(SocketMessage Messag)
        {
            string mess = Messag.Content.ToLower();

            SocketUserMessage Messege = Messag as SocketUserMessage;

            SocketCommandContext Context = new SocketCommandContext(Module.ConstVariables._Client, Messege);

            if (!ConstVariables.CServer[Context.Guild.Id].IsOn && !ConstVariables.ThisTest || Messag.Author.IsBot)
                    return;

            bool IsTrigger = false;

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            ISocketMessageChannel channel = Context.Channel;

            if ((guild.IsEntryOrСategoryChannel(channel.Id) && !Context.User.IsBot) || ConstVariables.ThisTest)
            {
                int count = ConstVariables.CountTextRequst(mess, Context.Guild.Id);

                if(count > 0)
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

                if(result.isSearch && (result.GuildID == Context.Guild.Id || result.GuildID == 0 || ConstVariables.ThisTest))
                {
                    if(result.Condition == "{0}")
                    {
                        if (result.TextAnswer.IndexOf("{0}") >= 0)
                            result.TextAnswer = String.Format(result.TextAnswer, Context.User.Mention);

                        await channel.SendMessageAsync(result.TextAnswer);
                    }
                    else
                    {
                        bool IsCondition = false;
                        bool IsParam1 = false;
                        if(result.Condition.IndexOf("{1}") > - 1)
                        {
                            if((guild.CountRes < guild.Restruction) && (!ConstVariables.CServer[Context.Guild.Id].Trigger))
                                IsCondition = true;

                            IsParam1 = true;
                        }
                        else IsCondition = true;

                        if (result.Condition.IndexOf("{2}") > -1)
                        {
                            if (mess.IndexOf("бот") == 0 || MentionUser(Context, ConstVariables._Client.CurrentUser.Id))
                                IsCondition &= true;
                            else IsCondition = false;
                        }
                        else IsCondition &= true;

                        if(IsCondition)
                        {
                            if (result.TextAnswer.IndexOf("{0}") >= 0)
                                result.TextAnswer = String.Format(result.TextAnswer, Context.User.Mention);

                            await channel.SendMessageAsync(result.TextAnswer);

                            if(IsParam1)
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

        protected async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage Messeg = arg as SocketUserMessage;

            if (Messeg is null || Messeg.Author.IsBot)
                return;
            int argPos = 0;

            SocketCommandContext contex = new SocketCommandContext(ConstVariables._Client, Messeg);

            SLog logger = new SLog(contex);

            IResult result = null;

            if (Messeg.HasStringPrefix("sh!", ref argPos) || Messeg.HasStringPrefix("Sh!", ref argPos))
            {
                result = await ConstVariables._Command.ExecuteAsync(contex, argPos, ConstVariables._UserService);               
            }
            else if (Messeg.HasStringPrefix("c!", ref argPos) || Messeg.HasStringPrefix("с!", ref argPos))
            {
                result = await ConstVariables._GameCommand.ExecuteAsync(contex, argPos, ConstVariables._GameService);                
            }

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

        public bool MentionUser(ICommandContext Context, params ulong[] Id)
        {
            foreach(var key in Context.Message.MentionedUserIds)
                for(int i=0; i < Id.Length; i++)
                    if(Id[i] == key)
                        return true;

            return false;
        }

        public virtual void Messege(string str)
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            Console.WriteLine($"[{time.Hours}:{time.Minutes}:{time.Seconds}.{time.Milliseconds}] {str}");
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
    }
}
