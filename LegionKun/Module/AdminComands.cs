using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegionKun.Module
{
    [Group("admin")]
    class AdminComands : ModuleBase<SocketCommandContext>
    {
        [Command("RoleInfo")]/*Произведено исправление[2]*/
        public async Task IhfoAsync()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                EmbedBuilder builder = new EmbedBuilder();

                int CountRole = Context.Guild.Roles.Count - 1;

                string roleinfo = "";

                List<string> Inforole = new List<string>(CountRole + 1);

                for (int z = 0; z < CountRole; z++)
                {
                    Inforole.Add("");
                }

                foreach (var role in Context.Guild.Roles)
                    if (role.Name != "@everyone")
                    {
                        //Console.WriteLine($"{role.Name}, {role.Position}"); /*для отладки*/
                        Inforole.RemoveAt(role.Position - 1);
                        Inforole.Insert(role.Position - 1, role.Name);
                    }

                for (int i = CountRole; i > 0; i--)
                {
                    try
                    {
                        roleinfo += $"{CountRole - i + 1}: {Inforole[i - 1]}\r\n";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{CountRole}, {i},  {e.Message}");
                    }
                }

                builder.WithTitle($"Количество ролей на сервере: {CountRole}");
                builder.WithDescription(roleinfo).WithColor(Discord.Color.Red);
                builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

                Module.ConstVariables.Log?.Invoke($" is group 'Admin' is command 'RoleInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("CTInfo")]/*Произведено исправление[1]*/
        public async Task CTIhfoAsync()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                EmbedBuilder builder = new EmbedBuilder();

                int CountChannels = Context.Guild.TextChannels.Count;

                string chanel = "";

                List<string> Channels = new List<string>(CountChannels);

                try
                {
                    for (int z = 0; z < CountChannels; z++)
                    {
                        Channels.Add("");
                    }

                    foreach (var chan in Context.Guild.TextChannels)
                    {
                        //Console.WriteLine($"{chan.Position}: {chan.Name}");/*для отладки*/
                        Channels.RemoveAt(chan.Position);
                        Channels.Insert(chan.Position, chan.Name);
                    }

                    for (int i = 0; i < CountChannels; i++)
                    {
                        chanel += $"{ i + 1 }: {Channels[i]} \r\n";
                    }

                    builder.WithTitle($"Количество текстовых каналов на сервере: {CountChannels}");
                    builder.WithDescription(chanel).WithColor(Discord.Color.Blue);
                    builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

                    Module.ConstVariables.Log?.Invoke($" is group 'Admin' is command 'CTInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                    await Context.Channel.SendMessageAsync("", false, builder.Build());
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{CountChannels}, {Channels.Capacity}, {e.Message}");
                    await Context.Channel.SendMessageAsync("Ошибка получения информации");
                }
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("CVInfo")]/*Произведено исправление[2]*/
        public async Task CVIhfoAsync()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                EmbedBuilder builder = new EmbedBuilder();

                int CountChannel = Context.Guild.VoiceChannels.Count;

                List<string> channels = new List<string>(CountChannel);

                string chanel = "";

                for (int i = 0; i < CountChannel; i++)
                {
                    channels.Add("");
                }

                foreach (var chan in Context.Guild.VoiceChannels)
                {
                    // Console.WriteLine($"{chan.Position}: {chan.Name}, {channels.Count}");/*для отладки*/
                    channels.RemoveAt(chan.Position);
                    channels.Insert(chan.Position, chan.Name);
                }
                //Console.WriteLine("Запись закончена");
                for (int i = 0; i < CountChannel; i++)
                {
                    chanel += $"{i + 1}: {channels[i]} \r\n";
                }

                builder.WithTitle($"Количество голосовых каналов на сервере: {CountChannel}");
                builder.WithDescription(chanel).WithColor(Discord.Color.Teal);
                builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);

                Module.ConstVariables.Log?.Invoke($" is group 'Admin' is command 'CVInfo' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }
        
        [Command("off", RunMode = RunMode.Async)]/*Произведено исправление[3]*/
        public async Task OffBotAsync(byte level = 0)
        {
            if(!Module.ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                return;
            }

            if (Module.ConstVariables.ThisTest)
            {
                return;
            }

            await Module.ConstVariables._Client.SetStatusAsync(UserStatus.DoNotDisturb);

            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild1 = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild1._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                switch (level)
                {
                    case 1:
                        {
                            Module.ConstVariables.CServer[Context.Guild.Id].IsOn = false;
                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    case 2:
                        {
                            foreach (var key in Module.ConstVariables.CServer)
                            {
                                Module.ConstVariables.CServer[key.Key].IsOn = false;
                            }

                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    default: { break; }
                }

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle("!!!Внимание!!!").WithDescription("бот будет выключен на обновление :stuck_out_tongue_winking_eye:").WithColor(Discord.Color.Magenta);

                foreach (var server in Module.ConstVariables.CServer)
                {
                    ConstVariables.CDiscord guild = server.Value;

                    var result = await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                    IEmote em = new Emoji("😜");

                    await result.AddReactionAsync(em);
                }

                Module.ConstVariables.Log?.Invoke($" is group 'Automatic' is command 'off' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                Module.ConstVariables.CServer[Context.Guild.Id].IsOn = false;
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("on", RunMode = RunMode.Async)]/*Произведено исправление[1]*/
        public async Task OnBotAsync(byte level = 0)
        {
            if (Module.ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                return;
            }

            if (Module.ConstVariables.ThisTest)
            {
                return;
            }

            await Module.ConstVariables._Client.SetStatusAsync(UserStatus.Online);

            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild1 = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild1._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                switch (level)
                {
                    case 1:
                        {
                            Module.ConstVariables.CServer[Context.Guild.Id].IsOn = true;
                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    case 2:
                        {
                            foreach (var key in Module.ConstVariables.CServer)
                            {
                                Module.ConstVariables.CServer[key.Key].IsOn = true;
                            }

                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    default: { break; }
                }


                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Ура").WithDescription("Я снова в строю").WithColor(Discord.Color.Magenta);

                foreach(var server in Module.ConstVariables.CServer)
                {
                    Module.ConstVariables.CDiscord guild = server.Value;

                    var result = await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                    IEmote em = new Emoji("💗");

                    await result.AddReactionAsync(em);
                }

                ConstVariables.Log?.Invoke($" is group 'Automatic' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                Module.ConstVariables.CServer[Context.Guild.Id].IsOn = true;
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("news", RunMode = RunMode.Async)]/*Произведено исправление[1]*/
        public async Task NewsAsync([Remainder]string mess)
        {
            if (Module.ConstVariables.ThisTest)
            {
                return;
            }

            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild1 = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild1._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"Новости бота").WithDescription(mess).WithColor(Discord.Color.Magenta);
                builder.WithFooter(Context.Guild.Name, Context.Guild.IconUrl);
                builder.WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/464447806887690240/news26052017.jpg?width=841&height=474");

                foreach(var server in Module.ConstVariables.CServer)
                {
                    if(server.Value.DefaultChannelNewsId == 0)
                    {
                        continue;
                    }

                    if((Context.User.Id == 329653972728020994) && (!Module.ConstVariables.ThisTest))
                    {
                        Module.ConstVariables.CDiscord guild = server.Value;

                        await guild.GetDefaultNewsChannel().SendMessageAsync("", false, builder.Build());
                    }
                    else
                    {
                        if (Context.Guild.Id == server.Value.GuildId)
                        {
                            Module.ConstVariables.CDiscord guild = server.Value;

                            await guild.GetDefaultNewsChannel().SendMessageAsync("", false, builder.Build());
                            break;
                        }
                    }
                }

                ConstVariables.Log?.Invoke($" is group 'Automatic' is command 'news' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("status")]
        public async Task StatusAsync()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                EmbedBuilder builder = new EmbedBuilder();

                string role = "";

                string channel = "";

                int i = 1;

                builder.WithTitle("the status of the bot").WithFooter(Context.Guild.Name, Context.Guild.IconUrl);
                builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                builder.AddField("Guild", Context.Guild.Name, true).AddField("Resurs", guild.CountRes + "/" + guild.Restruction, true);
                builder.AddField("Is on?", Module.ConstVariables.CServer[Context.Guild.Id].IsOn, true);

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
                
                builder.AddField("Roles", role, true).AddField("Channels", channel, true);

                if (role != "")
                    builder.AddField("Defaul channel", Context.Guild.GetTextChannel(guild.DefaultChannelId).Mention, true);
                if ((channel != "") && (guild.DefaultChannelNewsId != 0))
                    builder.AddField("Default channel for news", Context.Guild.GetTextChannel(guild.DefaultChannelNewsId).Mention, true);
                builder.AddField("Guild Id", guild.GuildId, true);

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("debug")]
        public async Task DebugAsync()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                ConstVariables.CServer[Context.Guild.Id].Debug = !Module.ConstVariables.CServer[Context.Guild.Id].Debug;

                ConstVariables.Log?.Invoke($" is group 'Admin' is command 'debug' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is result '{(guild.Debug ? "on":"off")}'");
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }

        [Command("logout")]
        public async Task LogOutAsync()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild = ConstVariables.CServer[Context.Guild.Id];

            bool IsRole = false;

            foreach (var role in user.Roles)
                if (guild._Role.ContainsKey(role.Id))
                {
                    IsRole = true;
                    break;
                }

            if (IsRole)
            {
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    Console.WriteLine("Ошибка доступа!");
                }

                ConstVariables.Log?.Invoke($" is group 'Admin' is command 'logout' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                await ConstVariables._Client.StopAsync();
                await ConstVariables._Client.LogoutAsync();
            }
            else await Module.ConstVariables.SendMessageAsync(Context.Channel, "Нет прав!", deleteAfter: 5);
        }
    }
}
