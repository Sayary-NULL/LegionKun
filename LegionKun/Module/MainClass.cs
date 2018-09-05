using Discord;
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

        public void OneMin(ulong guildId)
        {
            threadclass.OneMinStart(guildId);
        }

        public void MainTime()
        {
            threadclass.MainTimerStart();
        }

        public void Youtube()
        {
            threadclass.YoutubeStart();
        }

        public async Task MessageRec(SocketMessage Messag)
        {
            string mess = Messag.Content.ToLower();

            SocketUserMessage Messege = Messag as SocketUserMessage;

            SocketCommandContext Context = new SocketCommandContext(Module.ConstVariables._Client, Messege);
            
            ISocketMessageChannel channel = Context.Channel;

            if (!ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                if (!ConstVariables.ThisTest)
                    return;
            }           

            bool IsTrigger = false;

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            if(MentionUser(Context, 178916136404910082, 178916011167318017))
            {
                if (!ConstVariables.Sharon)
                {
                    Module.ConstVariables.Sharon = true;
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    builder.WithDescription("Генерал, мы тебя ждали!");
                    await channel.SendMessageAsync("", false, builder.Build());
                    ConstVariables.logger.Info($"Написал генерал! is Guid '{guild.GetGuild().Name}' is channel '{channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'");
                }
            }

            if (((guild.IsEntryOrСategoryChannel(channel.Id)) && (!Context.User.IsBot)) || ConstVariables.ThisTest)
            {
                if ((mess == "спокойной ночи") || (mess == "я спать"))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, Cпокойной ночи!");
                    IsTrigger = true;
                }
                else if ((mess == "я пошел") || (mess == "я пошёл"))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, удачи!:grin:");
                    IsTrigger = true;
                }
                else if ((mess == "доброе утро"))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, Доброе утро!");
                    IsTrigger = true;
                }
                else if ((mess == "здравствуйте") || (mess == "здравствуйте!"))
                {
                    TimeSpan current_time = DateTime.Now.TimeOfDay;
                    int h = current_time.Hours;

                    string good = $"{Context.User.Mention}, ";

                    if (h < 6)
                        good += "Доброй ночи!";
                    else if (h < 12)
                        good += "Доброе утро!";
                    else if (h < 18)
                        good += "Добрый день!";
                    else if (h < 24)
                        good += "Доброй ночи!";

                    IsTrigger = true;

                    await channel.SendMessageAsync(good);
                }
                else if ((mess == "хидери"))
                {
                    Random ran = new Random();

                    switch (ran.Next(4))
                    {
                        case 1:
                            {
                                await channel.SendMessageAsync($"{Context.User.Mention}, я за него!");
                                await channel.SendMessageAsync("Спрашивай");
                                break;
                            }
                        case 2:
                            {
                                await channel.SendMessageAsync($"Хидери, тебя ищет {Context.User.Mention}!");
                                break;
                            }
                        case 3:
                            {
                                await channel.SendMessageAsync($"Хидери, ау, ты где?");
                                break;
                            }
                        default:
                            {
                                await channel.SendMessageAsync($"Хидери, ау, ты где?");
                                break;
                            }
                    }
                    IsTrigger = true;
                }
                else if (mess == "канти")
                {
                    Random ran = new Random();
                    if (ran.Next(0, 2) == 1)
                    {
                        await channel.SendMessageAsync("Канти - наше солнышко! :kissing_heart:");
                    }
                    else
                    {
                        await channel.SendMessageAsync("раз, два, три, четыре, пять, вышла Канти погулять :stuck_out_tongue_closed_eyes:");
                    }
                    IsTrigger = true;
                }
                else if ((mess == "мур") && (guild.CountRes < guild.Restruction) && (!ConstVariables.CServer[Context.Guild.Id].Trigger))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, Мяу :heartpulse:");
                    guild.CountRes++;
                    IsTrigger = true;
                    ConstVariables.CServer[Context.Guild.Id].Trigger = true;
                    new Program().OneMin(Context.Guild.Id);
                }

                if ((mess.IndexOf("с возвращением") > -1))
                {
                    await channel.SendMessageAsync($"Мы тебя ждали!");
                    IsTrigger = true;
                }
                else if (((mess.IndexOf("хидери") > -1) || MentionUser(Context, guild.OwnerId, ConstVariables.CreatorId)) && (mess.IndexOf("ты не прав") > -1))
                {
                    await channel.SendMessageAsync($"Внимание!!! {Context.User.Mention} устроил бунт на корабле!");
                    IsTrigger = true;
                }
                else if ((mess.IndexOf("уруру") > -1) && (mess.Length <= "уруру))))".Length) && (guild.CountRes < guild.Restruction) && (!ConstVariables.CServer[Context.Guild.Id].Trigger))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, ~Уруру");

                    guild.CountRes++;
                    IsTrigger = true;
                    ConstVariables.CServer[Context.Guild.Id].Trigger = true;
                    new Program().OneMin(Context.Guild.Id);
                }
                else if ((mess.IndexOf("ахах") > -1) && (guild.CountRes < guild.Restruction) && (!ConstVariables.CServer[Context.Guild.Id].Trigger))
                {
                    await channel.SendMessageAsync("ахахаха. Чего смеёмся? Я тоже хочу!");

                    guild.CountRes++;
                    IsTrigger = true;
                    ConstVariables.CServer[Context.Guild.Id].Trigger = true;
                    new Program().OneMin(Context.Guild.Id);
                }
                else if ((mess.IndexOf("в чем смысл жизни?") > -1) && MentionUser(Context, ConstVariables._Client.CurrentUser.Id))
                {
                   await Context.Channel.SendMessageAsync("Смысл жизни: 42");
                    IsTrigger = true;
                }

                if (IsTrigger)
                {
                    ConstVariables.logger.Info($" Сработал тригер: {mess}! is Guid '{guild.GetGuild().Name}' is channel '{channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'");
                }
            }
        }

        public bool MentionUser(ICommandContext Context, params ulong[] Id)
        {
            foreach(var key in Context.Message.MentionedUserIds)
            {
                for(int i=0; i < Id.Length; i++)
                {
                    if(Id[i] == key)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void Messege(string str)
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            Console.WriteLine($"[{time.Hours}:{time.Minutes}:{time.Seconds}.{time.Milliseconds}] {str}");
        }
    }
}
