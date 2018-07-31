﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionKun.Module
{
    public class MainClass
    {
        private readonly ThreadClass threadclass = new ThreadClass();

        public void OneMin()
        {
            threadclass.OneMinStart();
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
            if (!Module.ConstVariables.IsOn)
            {
                if (!Module.ConstVariables.ThisTest)
                    return;
            }

            string mess = Messag.Content.ToLower();

            SocketUserMessage Messege = Messag as SocketUserMessage;

            SocketCommandContext Context = new SocketCommandContext(Module.ConstVariables._Client, Messege);
            
            ISocketMessageChannel channel = Context.Channel;

            bool IsTrigger = false;

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            if (Context.User.Id == 178916136404910082 || Context.User.Id == 178916011167318017)
            {
                if (!Module.ConstVariables.Sharon)
                {
                    Module.ConstVariables.Sharon = true;
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    builder.WithDescription("Генерал, мы тебя ждали!");
                    await channel.SendMessageAsync("", false, builder.Build());
                    Logger($"Написал генерал! is Guid {guild.GetGuild().Name} is channel {channel.Name} is user {Context.User.Username}#{Context.User.Discriminator}");
                }
            }

            if (((guild._Channels.ContainsKey(channel.Id)) || (guild._Channels.Count == 0)) && (!Context.User.IsBot))
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
                    await channel.SendMessageAsync("Канти - наше солнышко! :kissing_heart:");
                    IsTrigger = true;
                }
                else if ((mess == "мур") && (guild.CountRes < guild.Restruction) && (!Module.ConstVariables.Trigger))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, Мяу :heartpulse:");
                    guild.CountRes++;
                    IsTrigger = true;
                    Module.ConstVariables.Trigger = true;
                    new Program().OneMin();
                }

                if ((mess.IndexOf("с возвращением") > -1))
                {
                    await channel.SendMessageAsync($"Мы тебя ждали!");
                    IsTrigger = true;
                }
                else if (((mess.IndexOf("хидери") > -1) || (mess.IndexOf("<@380057037532561429>") > -1)) && (mess.IndexOf("ты не прав") > -1))
                {
                    await channel.SendMessageAsync($"Внимание!!! {Context.User.Mention} устроил бунт на корабле!");
                    IsTrigger = true;
                }
                else if ((mess.IndexOf("уруру") > -1) && (mess.Length <= "уруру))))".Length) && (guild.CountRes < guild.Restruction) && (!Module.ConstVariables.Trigger))
                {
                    await channel.SendMessageAsync($"{Context.User.Mention}, ~Уруру");

                    guild.CountRes++;
                    IsTrigger = true;
                    Module.ConstVariables.Trigger = true;
                    new Program().OneMin();
                }
                else if ((mess.IndexOf("ахах") > -1) && (guild.CountRes < guild.Restruction) && (!Module.ConstVariables.Trigger))
                {
                    await channel.SendMessageAsync("ахахаха. Чего смеёмся? Я тоже хочу!");

                    guild.CountRes++;
                    IsTrigger = true;
                    Module.ConstVariables.Trigger = true;
                    new Program().OneMin();
                }

                if (IsTrigger)
                {
                    Logger($" Сработал тригер: {mess}! is Guid {guild.GetGuild().Name} is channel {channel.Name} is user {Context.User.Username}#{Context.User.Discriminator}");
                }
            }
        }

        public virtual void Messege(string str)
        {
            Console.WriteLine(str);
        }

        public virtual void Logger(string mess)
        {
            using (StreamWriter writer = File.AppendText(Module.ConstVariables.Logger))
            {
                TimeSpan time = DateTime.Now.TimeOfDay;

                DateTime day = DateTime.Now.Date;

                string month = "Не указано";

                switch (day.Month)
                {
                    case 1: { month = "Январь"; break; }
                    case 2: { month = "Февраль"; break; }
                    case 3: { month = "Март"; break; }
                    case 4: { month = "Апрель"; break; }
                    case 5: { month = "Май"; break; }
                    case 6: { month = "Июнь"; break; }
                    case 7: { month = "Июль"; break; }
                    case 8: { month = "Август"; break; }
                    case 9: { month = "Сентябрь"; break; }
                    case 10: { month = "Октябрь"; break; }
                    case 11: { month = "Ноябрь"; break; }
                    case 12: { month = "Декабрь"; break; }
                    default: { month = "Не указано"; break; }
                }

                writer.WriteLine($"[{day.Day} {month} {day.Year}  {time.Hours}:{time.Minutes}:{time.Seconds}]{mess}");

                if (!Module.ConstVariables.ThisTest)
                    Messege($"[{day.Day} {month} {day.Year}  {time.Hours}:{time.Minutes}:{time.Seconds}]{mess}");
            }
        }
    }
}