using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using NLog;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using LegionKun.BotAPI;
using System.IO;

namespace LegionKun.Module
{
    [DataContract]
    public class DateBaseJSON
    {
        [DataMember]
        public ulong OwnerID = 0;
        [DataMember]
        public string ConnectionStringKey = "";
    }

    public static class ConstVariables
    {
        public delegate void DMessege(string str);

        public static DMessege Mess = null;

        public static DiscordAPI LegionDiscord = new DiscordAPI();

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
            public ulong DefaultChannelSendMessageForInfoUsers;
            public ulong GuildId;
            public ulong EndUser = 0;
            //дополнительные данные
            public int Restruction = 50;
            public int CountRes = 0;
            public string Name;
            public bool Debug = false;
            public bool Trigger = false;
            public int NumberNewUser = 0;

            ///<summary>возвращает class сервера</summary>
            public SocketGuild GetGuild()
            {
                return DiscordAPI._Client.GetGuild(GuildId);
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

            public SocketTextChannel GetChannelSendMessageForInfoUsers()
            {
                if (DefaultChannelSendMessageForInfoUsers != 0)
                    return GetGuild().GetTextChannel(DefaultChannelSendMessageForInfoUsers);
                else return null;
            }

            public bool EntryRole(ulong RoleId)
            {
                string SqlRequest = $"SELECT [RoleId] FROM Role WHERE [GuildId] = {GuildId} AND [RoleId] = {RoleId}";

                using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
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
                        Logger.Error($"is func 'EntryRole' is errors {e}");
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

                using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
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
                        Logger.Error($"is func 'IsPrefixChannel' is id cannel: '{ChannelId}' is default: '{IsDefault}' is News Channel: '{IsNewsChannel}' is command: '{IsCommand}' is errors {e}");
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

        public static Dictionary<char, char> Code = new Dictionary<char, char>();
#if DEBUG
        public static bool ThisTest { get; private set; } = true;
#else 
        public static bool ThisTest { get; private set; } = false;
#endif 
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
            new Commands( "ping" , "ping", true),
            new Commands( "ban" , "ban [User Mention]", false),
            new Commands( "report" , "report [Name Command] [report text]", true),
            new Commands( "banlistuser" , "banlistuser", false),
            new Commands( "banlist" , "banlist <User Mention>", true),
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
            new Commands( "banlistadmin" , "banlistadmin <Admin Mention>", true),
            new Commands( "banlistadd" , "banlistadd [User Mention] <Comment>", true),
            new Commands( "addtrigger" , "addtrigger [\"Text Search\"] [\"Text Otvet\"]", true),
            new Commands( "selecttrigger" , "selecttrigger", true),
            new Commands( "selecttriggerdefault" , "selecttriggerdefault", false),
            new Commands( "deletetrigger" , "deletetrigger [id trigger]", true),
            new Commands( "updatetrigger" , "updatetrigger [id trigger] [\"Re Text Otvet\"]", true)
        };

        public static DateBaseJSON DateBase;

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

            public static Emoji ERemuv = new Emoji("💢");
        }

        public static string UTHelp { get; private set; } = "";

        public static string Patch = "";

        public static string ATHelp { get; private set; } = "";

        public static bool Perevorot { get; set; } = false;

        public static bool ControlFlow { get; set; } = false;

        public static bool IsDownloadGuild { get; private set; } = false;

        public static Discord.Color InfoColor { get; private set; } = Color.DarkTeal;

        public static Discord.Color UserColor { get; private set; } = Color.Blue;

        public static Discord.Color AdminColor { get; private set; } = Color.Red;

        private static Thread DownloadetThread = new Thread(DownlodeGuildParams);

        public static Thread LegionDiscordThread = new Thread(LegionDiscord.RunBotAsync);

        public static Logger Logger { get; private set; } = LogManager.GetCurrentClassLogger();

        public struct ResultIndexOfText
        {
            public ulong GuildID;
            public string TextRequest;
            public string TextAnswer;
            public string Condition;
            public bool isSearch;
        }
        
        ///<summary>NCR = Not a Correct Result</summary>
        public const string NCR = "NCR";

        public static bool ConnectionBase()
        {
            
            try
            {
                using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand("sp_GetCountGuild", connect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        int Count = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ResultIndexOfText IndexOfText(string text)
        {
            string SqlExpression = "sp_GetIndexOfText";
            ResultIndexOfText result = new ResultIndexOfText
            {
                isSearch = false
            };

            try
            {
                using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand(SqlExpression, connect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.GuildID = (ulong)reader.GetInt64(1);
                                result.TextRequest = reader.GetString(2);
                                result.TextAnswer = reader.GetString(3);
                                result.Condition = reader.GetString(4);
                                result.isSearch = false;

                                if (text.IndexOf(result.TextRequest) > -1)
                                {
                                    result.isSearch = true;
                                    break;
                                }
                            }

                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"IndexOfText: is errors {e.Message}");
            }

            return result;
        }

        public static string GetVideo1(int id)
        {
            string str = "_NO_";

            if ((id != 1) && (id != 2))
                return str;

            using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                connect.Open();

                using (SqlCommand command = new SqlCommand("sp_GetVideoId", connect) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    command.CommandText = "sp_GetVideoId";
                    SqlParameter IdParam = new SqlParameter()
                    {
                        ParameterName = "@Id",
                        DbType = System.Data.DbType.Int16,
                        Value = id
                    };

                    command.Parameters.Add(IdParam);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        str =  reader.GetString(0);
                        reader.Close();
                    }
                }
            }
            return str;
        }

        public static bool UpdVideo(int id, string strock)
        {
            int result = 0;

            using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                connect.Open();

                using (SqlCommand command = new SqlCommand("sp_UpdateVideoId", connect) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    SqlParameter IdParam = new SqlParameter()
                    {
                        ParameterName = "@Id",
                        DbType = System.Data.DbType.Int16,
                        Value = id
                    };

                    SqlParameter Vparam = new SqlParameter()
                    {
                        ParameterName = "@VId",
                        DbType = System.Data.DbType.String,
                        Value = strock
                    };

                    command.Parameters.Add(IdParam);
                    command.Parameters.Add(Vparam);

                    result = command.ExecuteNonQuery();
                }
            }
            return result != 0;
        }

        public static string TextRequst(string text, ulong serverid)
        {
            ulong server = 0;
            List<string> list = new List<string>();
            int count = 0;
            Random ran = new Random();            

            using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                connect.Open();

                using (SqlCommand command = new SqlCommand("sp_CountTextTrigger", connect) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    SqlParameter Vparam = new SqlParameter()
                    {
                        ParameterName = "@TextRequest",
                        DbType = System.Data.DbType.String,
                        Value = text
                    };

                    command.Parameters.Add(Vparam);
                    count = Convert.ToInt32(command.ExecuteScalar());

                    if (count == 0)
                        return NCR;

                    command.CommandText = "sp_TextTrigger";

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            server = (ulong)reader.GetInt64(0);
                            list.Add(reader.GetString(1));
                        }
                        reader.Close();
                    }
                }
            }

            if (server != serverid && server != 0)
                return NCR;
            if (count == 1)
                return list[0];

            return list[ran.Next(0, count)];
        }

        public static int CountTextRequst(string text, ulong serverid)
        {
            int count = 0;
            using(SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                connect.Open();

                using (SqlCommand command = new SqlCommand("sp_CountTextTrigger", connect) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    SqlParameter Vparam = new SqlParameter()
                    {
                        ParameterName = "@TextRequest",
                        DbType = System.Data.DbType.String,
                        Value = text
                    };

                    command.Parameters.Add(Vparam);
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return count;
        }

        public static bool InstallationLists()
        {
            int i = 1;
            foreach (var help in UserCommand)
                if (help.IsOn)
                    UTHelp += $"{i++}: {help.CommandName}\r\n";            

            i = 1;
            foreach (var help in AdminCommand)
                if (help.IsOn)
                    ATHelp += $"{i++}: {help.CommandName}\r\n";            

            {
                //1
                Code.Add('q', 'й');
                Code.Add('w', 'ц');
                Code.Add('e', 'у');
                Code.Add('r', 'к');
                Code.Add('t', 'е');
                Code.Add('y', 'н');
                Code.Add('u', 'г');
                Code.Add('i', 'ш');
                Code.Add('o', 'щ');
                Code.Add('p', 'з');
                Code.Add('[', 'х');
                Code.Add(']', 'ъ');
                //2
                Code.Add('a', 'ф');
                Code.Add('s', 'ы');
                Code.Add('d', 'в');
                Code.Add('f', 'а');
                Code.Add('g', 'п');
                Code.Add('h', 'р');
                Code.Add('j', 'о');
                Code.Add('k', 'л');
                Code.Add('l', 'д');
                Code.Add(';', 'ж');
                Code.Add('\'', 'э');
                //3
                Code.Add('z', 'я');
                Code.Add('x', 'ч');
                Code.Add('c', 'с');
                Code.Add('v', 'м');
                Code.Add('b', 'и');
                Code.Add('n', 'т');
                Code.Add('m', 'ь');
                Code.Add(',', 'б');
                Code.Add('.', 'ю');
                Code.Add('/', '.');
                Code.Add('`', 'ё');
                //+shift
                //1
                Code.Add('Q', 'Й');
                Code.Add('W', 'Ц');
                Code.Add('E', 'У');
                Code.Add('R', 'К');
                Code.Add('T', 'Е');
                Code.Add('Y', 'Н');
                Code.Add('U', 'Г');
                Code.Add('I', 'Ш');
                Code.Add('O', 'Щ');
                Code.Add('P', 'З');
                Code.Add('{', 'Х');
                Code.Add('}', 'Ъ');
                //2
                Code.Add('A', 'Ф');
                Code.Add('S', 'Ы');
                Code.Add('D', 'В');
                Code.Add('F', 'А');
                Code.Add('G', 'П');
                Code.Add('H', 'Р');
                Code.Add('J', 'О');
                Code.Add('K', 'Л');
                Code.Add('L', 'Д');
                Code.Add(':', 'Ж');
                Code.Add('\"', 'Э');
                //3
                Code.Add('Z', 'Я');
                Code.Add('X', 'Ч');
                Code.Add('C', 'С');
                Code.Add('V', 'М');
                Code.Add('B', 'И');
                Code.Add('N', 'Т');
                Code.Add('M', 'Ь');
                Code.Add('<', 'Б');
                Code.Add('>', 'Ю');
                Code.Add('?', ',');
                Code.Add('~', 'Ё');
                //
                Code.Add('&', '?');
                Code.Add(' ', ' ');
            }

            SurrialJSON();

            if (!ConnectionBase())
            {
                Console.WriteLine("Нет потключения к базе!");
                return false;
            }

            DownloadetThread.Start();

            while (!IsDownloadGuild);
            return true;
        }

        public static void SurrialJSON()
        { 
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(DateBaseJSON));

            using (FileStream fs = new FileStream(Patch + "Base//DateBase.json", FileMode.Open))
            {
               DateBase = jsonFormatter.ReadObject(fs) as DateBaseJSON;
            }
        }

        public static void SetDelegate(DMessege fun)
        {
            Mess = fun;
            LegionDiscord.SetDelegate(fun);
        }

        public static void DownlodeGuildParams()
        {
            string SqlExpressionCountGuild = "sp_GetCountGuild", 
                SqlExpressionGuildId = "sp_GetGuildId", 
                SqlExpressionOwnerId = "sp_GetOwnerId", 
                SqlExpressionChannelId = "sp_GetChannelId", 
                SqlExpressionChannelNewsId = "sp_GetChannelNewsId",
                SqlExpressionInfoUser = "sp_GetChannelForInfoUser";

            do
            {
                Stopwatch sw = Stopwatch.StartNew();
                using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
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

                                    command.CommandText = SqlExpressionInfoUser;
                                    reader = command.ExecuteReader();

                                    if(reader.HasRows)
                                    {
                                        reader.Read();
                                        discord.DefaultChannelSendMessageForInfoUsers = (ulong)reader.GetInt64(0);
                                        reader.Close();
                                    }
                                    else throw new Exception($"Ошибка скачивания канала для отправки уведомления об уходе с сервера: {key}");

                                    if (CServer.ContainsKey(discord.GuildId))
                                    {
                                        discord.EndUser = CServer[discord.GuildId].EndUser;

                                        discord.Debug = CServer[discord.GuildId].Debug;

                                        discord.CountRes = CServer[discord.GuildId].CountRes;

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
                                Logger.Error($"is func 'DownlodeGuildParams() -> download param' is guild: '{key}' is error {e}");

                                if (ThisTest)
                                    Console.WriteLine($"is func 'DownlodeGuildParams() -> download param' is guild: '{key}' is error {e}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"is func 'DownlodeGuildParams()' is error {e}");

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
