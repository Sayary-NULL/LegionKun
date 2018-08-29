using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Discord.Rest;
using NLog;

namespace LegionKun.Module
{
    public static class ConstVariables
    {
        public static DiscordSocketClient _Client { get; set; }
        public static CommandService _Command { get; set; }
        public static CommandService _GameCommand { get; set; }
        public static IServiceProvider _UserService { get; set; }
        public static IServiceProvider _GameService { get; set; }

        /// <summary>
        /// Класс серверов где работает бот
        /// </summary>
        public class CDiscord
        {
            //массив ролей и каналов для реакции бота
            public Dictionary<ulong, string> _Channels = new Dictionary<ulong, string>();
            public Dictionary<ulong, string> _Role = new Dictionary<ulong, string>();
            //id
            public ulong OwnerId;
            public ulong DefaultChannelId;
            public ulong DefaultChannelNewsId;
            public ulong DefaultCommandChannel;
            public ulong GuildId;
            public ulong EndUser = 0;
            //дополнительные данные
            public int Restruction = 50;
            public int CountRes = 0;
            public string Name;
            public bool Debug = false;
            public bool IsOn = false;
            public bool Trigger = false;
            public int NumberNewUser = 0;
            //мини-класс для random
            public class Rmessages
            {
                public int MinValue = 0;
                public int MaxValue = 1;
                public EmbedBuilder Embed = null;
                public RestUserMessage RestUser = null;
                public ulong UserId = 0;
            }
            public Rmessages RMessages = new Rmessages();

            ///<summary>возвращает class сервера</summary>
            public SocketGuild GetGuild()
            {
                return _Client.GetGuild(GuildId);
            }
            ///<summary>возвращает class канала по умолчанию</summary>
            public SocketTextChannel GetDefaultChannel()
            {
                if (DefaultChannelId != 0)
                    return GetGuild().GetTextChannel(DefaultChannelId);
                else return null;
            }
            ///<summary>возвращает class канала новостей</summary>
            public SocketTextChannel GetDefaultNewsChannel()
            {
                if (DefaultChannelNewsId != 0)
                    return GetGuild().GetTextChannel(DefaultChannelNewsId);
                else return null;
            }
            ///<summary>возвращает class канал с командами бота</summary>
            public SocketTextChannel GetDefaultCommandChannel()
            {
                if (DefaultChannelId != 0)
                    return GetGuild().GetTextChannel(DefaultCommandChannel);
                else return null;
            }
        }
        
        public static Dictionary<ulong, CDiscord> CServer = new Dictionary<ulong, CDiscord>();

        public static Dictionary<ulong, ulong> DMessage = new Dictionary<ulong, ulong>();

        public static List<Commands> UserCommand { get; private set; } = new List<Commands>()
        {
            //UserCommands
            new Commands( "hello" , "hello <User Mention>", true),
            new Commands( "say" , "say [text]", true),
            new Commands( "warn" , "warn [User Mention] <comment>", true),
            new Commands( "roleinfo" , "RoleInfo <Role>", true),
            new Commands( "time" , "time", true),
            new Commands( "coin" , "coin [number]", true),
            new Commands( "search" , "search [Seearch text]", true),
            new Commands( "userinfo" , "userinfo <User Mention>", true),
            new Commands( "serverinfo" , "serverinfo", true),
            new Commands( "ctinfo" , "ctinfo", true),
            new Commands( "cvinfo" , "cvinfo", true),
            new Commands( "ping" , "ping", true),
            new Commands( "ban" , "ban [User Mention]", false),
            new Commands( "report" , "report [Name Command] [report text]", true),
            new Commands( "help" , "help", true),
        };

        public static List<Commands> AdminCommand { get; private set; } = new List<Commands>()
        {
            //AdminCommands
            new Commands( "test" , "test", ThisTest),
            new Commands( "news" , "news [news]", true),
            new Commands( "status" , "status", true),
            new Commands( "debug" , "debug", true),
            new Commands( "flowcontrol" , "flowcontrol <number level>", true),
            new Commands( "banlist" , "banlist <User Mention>", true),
            new Commands( "banlistadmin" , "banlistadmin <Admin Mention>", true),
            new Commands( "banlistadd" , "banlistadd [User Mention] <Comment>", true)
        };

        public struct Commands
        {
            public string Name;
            public string CommandName;
            public bool IsOn;
            
            public Commands(string name, string comname, bool ison)
            {
                Name = name;
                IsOn = ison;
                CommandName = comname;
            }

            public bool ContainerName(string name)
            {
                if (Name == name)
                    return true;
                return false;
            }
        }

        public static class DEmoji
        {
            public static Emoji EReturn = new Emoji("🔄");

            public static Emoji EDelete = new Emoji("❎");
        }

        public static ulong CreatorId { get; private set; } = 329653972728020994;

        public static string WiteListGuild { get; private set; } = @"";

        public static string Filed { get; private set; } = @"";

        public static string Cross { get; private set; } = @"";

        public static string Zero { get; private set; } = @"";

        public static string UTHelp { get; private set; } = "";

        public static string ATHelp { get; private set; } = "";

        public static string Video1Id { get; set; } = "";

        public static string Video2Id { get; set; } = "";
#if DEBUG
        public static bool ThisTest { get; private set; } = true;
#else 
        public static bool ThisTest { get; private set; } = false;
#endif 
        public static bool Sharon { get; set; } = false;

        public static bool Perevorot { get; set; } = false;

        public static bool ControlFlow { get; set; } = true;

        public delegate void DMessege(string str);

        public static DMessege Mess = null;

        public static Discord.Color InfoColor { get; private set; } = Color.DarkTeal;

        public static Discord.Color UserColor { get; private set; } = Color.Blue;

        public static Discord.Color AdminColor { get; private set; } = Color.Red;

        public static Logger logger { get; private set; } = LogManager.GetCurrentClassLogger(); 

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel, string text, bool isTTS = false, Embed embed = null, uint deleteAfter = 0, RequestOptions options = null)
        {
            var message = await channel.SendMessageAsync(text, isTTS, embed, options);
            if (deleteAfter > 0)
            {
                var _ = Task.Run(() => DeleteAfterAsync(message, deleteAfter));
            }
            return message;
        }

        private static async Task DeleteAfterAsync(IUserMessage message, uint deleteAfter)
        {
            await Task.Delay(TimeSpan.FromSeconds(deleteAfter));
            await message.DeleteAsync();
        }

        public static void InstallationLists()
        {
            if(ThisTest)
            {
                WiteListGuild = @"C:\Users\shlia\source\repos\LegionKun\LegionKun\Base\WiteListGuild.txt";
                Filed = @"C:\Users\shlia\source\repos\LegionKun\LegionKun\Base\filed.jpg";
                Cross = @"C:\Users\shlia\source\repos\LegionKun\LegionKun\Base\cross.png";
                Zero = @"C:\Users\shlia\source\repos\LegionKun\LegionKun\Base\zero.png";
            }
            else
            {
                WiteListGuild = @"Base\WiteListGuild.txt";
                Filed = @"Base\filed.jpg";
                Cross = @"Base\cross.png";
                Zero = @"Base\zero.png";
            }

            _Client = new DiscordSocketClient();

            _Command = new CommandService();

            _GameCommand = new CommandService();

            _UserService = new ServiceCollection().AddSingleton(_Client).AddSingleton(_Command).AddSingleton<InteractiveService>().BuildServiceProvider();

            _GameService = new ServiceCollection().AddSingleton(_Client).AddSingleton(_GameCommand).AddSingleton<InteractiveService>().BuildServiceProvider();

            int i = 1;
            foreach(var help in UserCommand)
            {
                if (help.IsOn)
                    UTHelp += $"{i++}: {help.CommandName}\r\n";
            }

            i = 1;
            foreach(var help in AdminCommand)
            {
                if (help.IsOn)
                    ATHelp += $"{i++}: {help.CommandName}\r\n";
            }           

            bool result = GuildDowload();
            Mess($"GuildDowload: {result}");
        }

        public static void SetDelegate(DMessege fun)
        {
            Mess = fun;
        }
         
        private static bool GuildDowload()
        {
            bool result = false;
            using (StreamReader read = File.OpenText(WiteListGuild))
            {
                string strock = "";
                CDiscord discord = new CDiscord();

                while ((strock = read.ReadLine()) != null)
                {
                    strock.Trim();
                    if (strock.IndexOf("Name: ") > -1)
                    {
                        Mess?.Invoke($"Name: {strock.Substring("Name: ".Length, strock.Length - "Name: ".Length)}");
                        discord.Name = strock.Substring("Name: ".Length, strock.Length - "Name: ".Length);
                    }
                    else if (strock.IndexOf("Id: ") == 0)
                    {
                        //Mess?.Invoke($"{strock.Substring("Id: ".Length, strock.Length - "Id: ".Length)}");
                        discord.GuildId = Convert.ToUInt64(strock.Substring("Id: ".Length, strock.Length - "Id: ".Length));
                    }
                    else if(strock.IndexOf("OwnerId: ") == 0)
                    {
                        discord.OwnerId = Convert.ToUInt64(strock.Substring("OwnerId: ".Length, strock.Length - "OwnerId: ".Length));
                    }
                    else if (strock.IndexOf("Defaultchannelid: ") > -1)
                    {
                        //Mess?.Invoke($"{guild.DefaultChannelId}");
                        discord.DefaultChannelId = Convert.ToUInt64(strock.Substring("Defaultchannelid: ".Length, strock.Length - "Defaultchannelid: ".Length));
                    }
                    else if (strock.IndexOf("Restruction: ") > -1)
                    {
                        //Mess?.Invoke($"{guild.Restruction}");
                        discord.Restruction = Convert.ToInt16(strock.Substring("Restruction: ".Length, strock.Length - "Restruction: ".Length));
                    }
                    else if (strock.IndexOf("ChannelNewsId: ") > -1)
                    {
                        //Mess?.Invoke($"{guild.DefaultChannelNewsId}");
                        if (strock != "ChannelNewsId: -")
                            discord.DefaultChannelNewsId = Convert.ToUInt64(strock.Substring("ChannelNewsId: ".Length, strock.Length - "ChannelNewsId: ".Length));
                    }
                    else if(strock.IndexOf("ChannelCommand: ") > -1)
                    {
                        discord.DefaultCommandChannel = Convert.ToUInt64(strock.Substring("ChannelCommand: ".Length, strock.Length - "ChannelCommand: ".Length));
                    }
                    else if (strock.IndexOf("RolesId: ") > -1)
                    {
                        while ((strock = read.ReadLine()) != null)
                        {
                            strock.Trim();
                            string[] Role = strock.Split(new string[] { " - " }, StringSplitOptions.None);

                            if (strock.IndexOf(';') > -1)
                            {
                                Role[1].Trim(';');
                            }
                            
                            discord._Role.Add(Convert.ToUInt64(Role[0]), Role[1]);
                            //Mess?.Invoke($"id: {Convert.ToUInt64(Role[0])}, Name: {Role[1]}");

                            if (strock.IndexOf(';') > -1)
                            {
                                break;
                            }
                        }
                    }
                    else if (strock.IndexOf("ChannelsId: ") > -1)
                    {
                        while ((strock = read.ReadLine()) != null)
                        {
                            strock.Trim();
                            string[] Role = strock.Split(new string[] { " - " }, StringSplitOptions.None);

                            if (strock.IndexOf(';') > -1)
                            {
                                Role[1].Trim(';');
                            }
                            
                            discord._Channels.Add(Convert.ToUInt64(Role[0]), Role[1]);
                            //Mess?.Invoke($"id: {Convert.ToUInt64(Role[0])}, Name: {Role[1]}");

                            if (strock.IndexOf(';') > -1)
                            {
                                break;
                            }
                        }
                    }
                    else if(strock == "------------------------------------------")
                    {
                        CServer.Add(discord.GuildId, discord);
                        Mess?.Invoke($"Guild id: {discord.GuildId}\r\n" +
                                     $"Default Channel Id: {discord.DefaultChannelId}\r\n" +
                                     $"Channel News Id: {discord.DefaultChannelNewsId}\r\n" +
                                     $"Channel Command Id: {discord.DefaultCommandChannel}\r\n" +
                                     $"Restruction: {discord.Restruction}\r\n" +
                                     $"RolesId.Count: {discord._Role.Count}\r\n" +
                                     $"ChannelsId.Count: {discord._Channels.Count}\r\n");
                        result = true;

                        discord = new CDiscord
                        {
                            _Role = new Dictionary<ulong, string>(),
                            _Channels = new Dictionary<ulong, string>()
                        };
                    }
                }
            }

            return result;
        }
    }
} 