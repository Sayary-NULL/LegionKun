using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegionKun.Module
{
    [Group("admin")]
    class AdminComands : InteractiveBase
    {
        [Command("off", RunMode = RunMode.Async)]/*Произведено исправление[3]*/
        public async Task OffBotAsync(byte level = 0)
        {
            if(!ConstVariables.CServer[Context.Guild.Id].IsOn)
            {
                return;
            }

            if (ConstVariables.ThisTest)
            {
                return;
            }

            await ConstVariables._Client.SetStatusAsync(UserStatus.DoNotDisturb);

            var user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild1 = ConstVariables.CServer[Context.Guild.Id];

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
                            ConstVariables.CServer[Context.Guild.Id].IsOn = false;
                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    case 2:
                        {
                            foreach (var key in ConstVariables.CServer)
                            {
                                ConstVariables.CServer[key.Key].IsOn = false;
                            }

                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    default: { break; }
                }

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle("!!!Внимание!!!").WithDescription("бот будет выключен на обновление :stuck_out_tongue_winking_eye:").WithColor(Discord.Color.Magenta);

                foreach (var server in ConstVariables.CServer)
                {
                    ConstVariables.CDiscord guild = server.Value;

                    var result = await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                    IEmote em = new Emoji("😜");

                    await result.AddReactionAsync(em);
                }

                ConstVariables.Log?.Invoke($" is group 'Automatic' is command 'off' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                ConstVariables.CServer[Context.Guild.Id].IsOn = false;
            }
            else await ReplyAndDeleteAsync("Нет прав!", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("on", RunMode = RunMode.Async)]/*Произведено исправление[1]*/
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

            var user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild1 = ConstVariables.CServer[Context.Guild.Id];

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
                            ConstVariables.CServer[Context.Guild.Id].IsOn = true;
                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    case 2:
                        {
                            foreach (var key in Module.ConstVariables.CServer)
                            {
                                ConstVariables.CServer[key.Key].IsOn = true;
                            }

                            ConstVariables.Log?.Invoke($" is group 'Admin' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is level {level}");
                            return;
                        }
                    default: { break; }
                }


                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Ура").WithDescription("Я снова в строю").WithColor(Discord.Color.Magenta);

                foreach(var server in ConstVariables.CServer)
                {
                    ConstVariables.CDiscord guild = server.Value;

                    var result = await guild.GetDefaultChannel().SendMessageAsync("", false, builder.Build());

                    IEmote em = new Emoji("💗");

                    await result.AddReactionAsync(em);
                }

                ConstVariables.Log?.Invoke($" is group 'Automatic' is command 'on' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");

                ConstVariables.CServer[Context.Guild.Id].IsOn = true;
            }
            else await ReplyAndDeleteAsync("Нет прав!", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("news", RunMode = RunMode.Async)]/*Произведено исправление[1]*/
        public async Task NewsAsync([Remainder]string mess)
        {
            if (ConstVariables.ThisTest)
            {
                return;
            }

            var user = Context.Guild.GetUser(Context.User.Id);

            ConstVariables.CDiscord guild1 = ConstVariables.CServer[Context.Guild.Id];

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

                builder.WithTitle($"Новости бота")
                    .WithDescription(mess)
                    .WithColor(ConstVariables.InfoColor)
                    .WithFooter(Context.Guild.Name, Context.Guild.IconUrl)
                    .WithImageUrl("https://media.discordapp.net/attachments/462236317926031370/464447806887690240/news26052017.jpg?width=841&height=474");

                foreach(var server in ConstVariables.CServer)
                {
                    if(server.Value.DefaultChannelNewsId == 0)
                    {
                        continue;
                    }

                    if((Context.User.Id == 329653972728020994) && (!ConstVariables.ThisTest))
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

                ConstVariables.Log?.Invoke($" is group 'Automatic' is command 'news' is user '{Context.User.Username}' is channel '{Context.Channel.Name}'");
            }
            else await ReplyAndDeleteAsync("Нет прав!", timeout: TimeSpan.FromSeconds(5));
        }

        [Command("status")]
        public async Task StatusAsync()
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
                    .AddField("Channels", channel, true);

                if (role != "")
                    builder.AddField("Defaul channel", Context.Guild.GetTextChannel(guild.DefaultChannelId).Mention, true);
                if ((channel != "") && (guild.DefaultChannelNewsId != 0))
                    builder.AddField("Default channel for news", Context.Guild.GetTextChannel(guild.DefaultChannelNewsId).Mention, true);
                builder.AddField("Guild Id", guild.GuildId, true);

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else await ReplyAndDeleteAsync("Нет прав!", timeout: TimeSpan.FromSeconds(5));
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

                ConstVariables.CServer[Context.Guild.Id].Debug = !ConstVariables.CServer[Context.Guild.Id].Debug;

                await ReplyAndDeleteAsync($"Режим дебага: {ConstVariables.CServer[Context.Guild.Id].Debug}", timeout: TimeSpan.FromSeconds(5));

                ConstVariables.Log?.Invoke($" is group 'Admin' is command 'debug' is user '{Context.User.Username}' is channel '{Context.Channel.Name}' is result '{(guild.Debug ? "on":"off")}'");
            }
            else await ReplyAndDeleteAsync("Нет прав!", timeout: TimeSpan.FromSeconds(5));
        }
    }
}
