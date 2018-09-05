using System;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Discord.Rest;
using NLog;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;

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
            /*public SocketTextChannel GetDefaultCommandChannel()
            {
                if (DefaultChannelId != 0)
                    return GetGuild().GetTextChannel(DefaultCommandChannel);
                else return null;
            }*/

            public bool EntryRole(ulong RoleId)
            {
                string SqlRequest = $"SELECT [RoleId] FROM Role WHERE [GuildId] = {GuildId} AND [RoleId] = {RoleId}";

                using (SqlConnection conect = new SqlConnection(@"Data Source=KIRILL\SQL_LEGIONKUN;Initial Catalog=TestServer;Integrated Security=True; User Id = LegeonKun; Password = Kun73$Kanti//"))
                {
                    try
                    {
                        conect.Open();
                        SqlCommand command = new SqlCommand(SqlRequest, conect);
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else return false;
                    }
                    catch (Exception e)
                    {
                        logger.Error($"is func 'EntryRole' is errors {e}");
                        return false;
                    }
                }
            }
            
            public bool IsEntryOrСategoryChannel(ulong ChannelId, bool IsDefault = false, bool IsNewsChannel = false, bool IsCommand = false)
            {
                string SqlRequest = $"SELECT [ChannelId] FROM [Channel] WHERE [GuildId] = {GuildId} AND [ChannelId] = {ChannelId}";

                if (IsDefault)
                    SqlRequest += " AND [IsDefault] = 'true'";
                if (IsNewsChannel)
                    SqlRequest += " AND [IsNewsChannel] = 'true'";
                if (IsCommand)
                    SqlRequest += " AND [IsCommand] = 'true'";

                using (SqlConnection conect = new SqlConnection(@"Data Source=KIRILL\SQL_LEGIONKUN;Initial Catalog=TestServer;Integrated Security=True; User Id = LegeonKun; Password = Kun73$Kanti//"))
                {
                    try
                    {
                        conect.Open();
                        SqlCommand command = new SqlCommand(SqlRequest, conect);
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else return false;
                    }
                    catch (Exception e)
                    {
                        logger.Error($"is func 'IsPrefiчChannel' is id cannel: '{ChannelId}' is default: '{IsDefault}' is News Channel: '{IsNewsChannel}' is command: '{IsCommand}' is errors {e}");
                        return false;
                    }
                }
            }

            public void ServerInfo(ISocketMessageChannel channel = null)
            {
                string TextInfo = "";

                TextInfo += $"Guild Id: {GuildId}\r\n";
                TextInfo += $"Owner Id: {OwnerId}\r\n";
                TextInfo += $"Default Channel Id: {DefaultChannelId}\r\n";
                TextInfo += $"Default Channel News Id: {DefaultChannelNewsId}\r\n";

                if (channel == null)
                {
                    Console.WriteLine(TextInfo);
                }
                else channel.SendMessageAsync(TextInfo);
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

        public static bool IsDownloadGuild { get; private set; } = false;

        public delegate void DMessege(string str);

        public static DMessege Mess = null;

        public static Discord.Color InfoColor { get; private set; } = Color.DarkTeal;

        public static Discord.Color UserColor { get; private set; } = Color.Blue;

        public static Discord.Color AdminColor { get; private set; } = Color.Red;

        private static Thread DownloadetThread = new Thread(DownlodeGuildParams);

        public static Logger logger { get; private set; } = LogManager.GetCurrentClassLogger();

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

            DownloadetThread.Start();

            while (!IsDownloadGuild)
            {

            }
        }

        public static void SetDelegate(DMessege fun)
        {
            Mess = fun;
        }

        public static void DownlodeGuildParams()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string SqlExpressionCountGuild = "sp_GetCountGuild", 
                SqlExpressionGuildId = "sp_GetGuildId", 
                SqlExpressionOwnerId = "sp_GetOwnerId", 
                SqlExpressionChannelId = "sp_GetChannelId", 
                SqlExpressionChannelNewsId = "sp_GetChannelNewsId";

            do
            {
                using (SqlConnection connect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
                {
                    try
                    {
                        connect.Open();
                        List<ulong> ListGuildId = null;

                        using (SqlCommand command = new SqlCommand(SqlExpressionCountGuild, connect){ CommandType = System.Data.CommandType.StoredProcedure })
                        {
                            int Count = Convert.ToInt32(command.ExecuteScalar());
                             
                            ListGuildId = new List<ulong>(Count);

                            command.CommandText = SqlExpressionGuildId;
                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    ListGuildId.Add((ulong)reader.GetInt64(0));
                                }
                                reader.Close();
                            }
                            else throw new Exception($"Ошибка чтения GuildId");
                        }

                        foreach (ulong key in ListGuildId)
                        {
                            try
                            {
                                using (SqlCommand command = new SqlCommand(SqlExpressionOwnerId, connect) { CommandType = System.Data.CommandType.StoredProcedure })
                                {
                                    CDiscord discord = new CDiscord
                                    {
                                        GuildId = key
                                    };

                                    SqlParameter GuildidParameter = new SqlParameter
                                    {
                                        ParameterName = "@GuildId",
                                        DbType = System.Data.DbType.Int64,
                                        Value = key
                                    };

                                    command.Parameters.Add(GuildidParameter);
                                    SqlDataReader reader = command.ExecuteReader();

                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        discord.OwnerId = (ulong)reader.GetInt64(0);
                                        reader.Close();
                                    }
                                    else throw new Exception($"Ошибка скачивания id владельца сервера: {key}");

                                    command.CommandText = SqlExpressionChannelId;
                                    reader = command.ExecuteReader();

                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        discord.DefaultChannelId = (ulong)reader.GetInt64(0);
                                        reader.Close();
                                    }
                                    else throw new Exception($"Ошибка скачивания канала по умолчанию сервера: {key}");

                                    command.CommandText = SqlExpressionChannelNewsId;
                                    reader = command.ExecuteReader();

                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        discord.DefaultChannelNewsId = (ulong)reader.GetInt64(0);
                                        reader.Close();
                                    }
                                    else throw new Exception($"Ошибка скачивания канала для новостей по умолчанию сервера: {key}");

                                    if (CServer.ContainsKey(discord.GuildId))
                                    {
                                        discord.EndUser = CServer[discord.GuildId].EndUser;

                                        discord.Debug = CServer[discord.GuildId].Debug;

                                        discord.CountRes = CServer[discord.GuildId].CountRes;

                                        discord.IsOn = CServer[discord.GuildId].IsOn;

                                        discord.Trigger = CServer[discord.GuildId].Trigger;

                                        discord.NumberNewUser = CServer[discord.GuildId].NumberNewUser;

                                        CServer.Remove(discord.GuildId);
                                        CServer.Add(discord.GuildId, discord);
                                    }
                                    else CServer.Add(discord.GuildId, discord);
                                }                                
                            }
                            catch (Exception e)
                            {
                                logger.Error($"is func 'DownlodeGuildParams() -> download param' is guild: '{key}' is error {e}");

                                if (ThisTest)
                                    Console.WriteLine($"is func 'DownlodeGuildParams() -> download param' is guild: '{key}' is error {e}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error($"is func 'DownlodeGuildParams()' is error {e}");

                        if (ThisTest)
                            Console.WriteLine($"is func 'DownlodeGuildParams()' is error {e}");
                    }
                }

                sw.Stop();
                Mess($"DownlodeGuildParams; is time: {sw.Elapsed}");
                IsDownloadGuild = true;

                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
            } while (true);
        }
    }    
}