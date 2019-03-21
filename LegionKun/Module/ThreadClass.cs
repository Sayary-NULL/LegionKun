using Discord;
using Discord.WebSocket;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using LegionKun.BotAPI;

namespace LegionKun.Module
{
    class ThreadClass
    {
        private Thread OneMinTimer = new Thread(OneMin);

        private Thread MainTimer = new Thread(MainFunc);

        private Thread YouTubeStream = new Thread(Youtube);

        public void OneMinStart(ulong guildId)
        {
            OneMinTimer.Start(guildId);
        }

        public void MainTimerStart()
        {
            MainTimer.Start();
        }

        public void YoutubeStart()
        {
            YouTubeStream.Start();
        }

        private static void OneMin(object obj)
        {
            Thread.Sleep(60000);
            ulong guildId = (ulong)obj;
            Module.ConstVariables.CServer[guildId].Trigger = false;
        }

        private static void MainFunc(object obj)
        {
            ConstVariables.Mess?.Invoke(" Запуск потока: MainFunc;");
            ConstVariables.logger.Info("Запуск потока: MainFunc;");
            while (true)
            {
                TimeSpan time = DateTime.Now.TimeOfDay;

                if (time.Hours == 0)
                {
                    foreach (var gui in Module.ConstVariables.CServer)
                    {
                        Module.ConstVariables.CServer[gui.Key].CountRes = 0;
                        Module.ConstVariables.CServer[gui.Key].EndUser = 0;
                        Module.ConstVariables.CServer[gui.Key].NumberNewUser = 0;
                    }

                    ConstVariables.Perevorot = false;

                    Module.ConstVariables.Mess?.Invoke($" произведен сброс!");
                    ConstVariables.logger.Info("произведен сброс!");
                }

                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
            }
        }

        private struct Stream
        {
            public int id;
            public string channelid;
            public string videoid;
            public bool ison;
        }

        private static async void Youtube(object obj)
        {
            if (ConstVariables.ThisTest)
                return;

            while (DiscordAPI._Client.ConnectionState != ConnectionState.Connected);

            Thread.Sleep(2000);

            ConstVariables.Mess?.Invoke(" Запуск потока: YouTubeStream;");
            ConstVariables.logger.Info("Запуск потока: YouTubeStream;");

            EmbedBuilder Live = new EmbedBuilder();
            Live.AddField("Новости", "Найден стрим!")
                .WithColor(ConstVariables.InfoColor);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Base.Resource1.ApiKeyToken,
                ApplicationName = "Legion-kun"
            });

            string SqlExpressionStreams = "sp_GetAccountStreams";

            do
            {
                if(ConstVariables.ControlFlow)
                {
                    Thread.Sleep(60000);
                    continue;
                }

                List<Stream> streams = new List<Stream>();

                using (SqlConnection connect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
                {
                    try
                    {
                        connect.Open();
                        using (SqlCommand command = new SqlCommand(SqlExpressionStreams, connect))
                        {
                            SqlDataReader reader = command.ExecuteReader();

                            if(reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Stream NewStr;
                                    NewStr.id = reader.GetInt32(0);
                                    NewStr.channelid = reader.GetString(1);
                                    NewStr.videoid = reader.GetString(2);
                                    NewStr.ison = reader.GetBoolean(3);
                                    if(NewStr.ison)
                                        streams.Add(NewStr);
                                }
                                reader.Close();
                            }
                            else throw new Exception($"Ошибка чтения");
                        }
                    }
                    catch (Exception e)
                    {
                        ConstVariables.logger.Error($"YutubeSearch, errorrs: '{e}'");
                    }
                }

                foreach(Stream str in streams)
                {
                    SearchResource.ListRequest Account = youtubeService.Search.List("snippet");
                    Account.Type = "video";
                    Account.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
                    Account.ChannelId = str.channelid;
                    Account.MaxResults = 1;

                    SearchListResponse searchList = null;

                    try
                    {
                        searchList = await Account.ExecuteAsync();
                    }
                    catch (Exception e)
                    {
                        ConstVariables.logger.Error($"YoutubeSearch. is errors :{e}");
                        Thread.Sleep(60000);
                        continue;
                    }

                    if (searchList.Items.Count == 0)
                        continue;

                    SearchResult stream = searchList.Items[0];

                    if (stream.Id.VideoId == str.videoid)
                        continue;

                    if (ConstVariables.ThisTest)
                    {
                        ConstVariables.CDiscord guild = ConstVariables.CServer[435485527156981770];
                        SocketTextChannel channel = guild.GetGuild().GetTextChannel(444152623319482378);

                        await channel.SendMessageAsync("", embed: Live.Build());
                        await channel.SendMessageAsync("https://www.youtube.com/video/" + stream.Id.VideoId);
                    }
                    else
                    {
                        foreach (var key in ConstVariables.CServer)
                        {
                            try
                            {
                                SocketTextChannel channel = null;
                                if (key.Value.DefaultChannelNewsId == 0)
                                    channel = key.Value.GetDefaultChannel();
                                else channel = key.Value.GetGuild().GetTextChannel(key.Value.DefaultChannelNewsId);

                                await key.Value.GetDefaultNewsChannel().SendMessageAsync("@here", embed: Live.Build());
                                await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + stream.Id.VideoId);

                                //ConstVariables.logger.Info($"is guild {key.Value.Name} is channel {channel.Name}");
                            }
                            catch (Exception e)
                            {
                                ConstVariables.logger.Error($"is guild {key.Value.Name} is error {e}");
                                ConstVariables.Mess($"Youtube: is guild: {key.Key} {e}");
                            }
                        }
                    }

                    ConstVariables.UpdVideo(str.id, stream.Id.VideoId);
                }

                Thread.Sleep(30000);
            } while (true);
        }
    }
}