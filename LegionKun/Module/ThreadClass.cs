﻿using Discord;
using Discord.WebSocket;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Data.SqlClient;
using LegionKun.BotAPI;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using TwitchCoreAPI.JsonModule;

namespace LegionKun.Module
{
    class ThreadClass
    {
        private Thread OneMinTimer = new Thread(OneMin);

        private Thread MainTimer = new Thread(MainFunc);

        private Thread YouTubeStream = new Thread(Youtube);

        private Thread TwitchStream = new Thread(Twitch);

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

        public void TwitchStart()
        {
            TwitchStream.Start();
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
            public string Name;
            public string GuildsId;
        }

        private static async void Youtube(object obj)
        {
            while (DiscordAPI._Client.ConnectionState != ConnectionState.Connected);

            Thread.Sleep(2000);

            ConstVariables.Mess?.Invoke(" Запуск потока: YouTubeStream;");
            ConstVariables.logger.Info("Запуск потока: YouTubeStream;");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Base.Resource2.ApiKeyToken,
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

                using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
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
                                    Stream NewStr = new Stream();
                                    NewStr.id = reader.GetInt32(0);
                                    NewStr.channelid = reader.GetString(1);
                                    NewStr.videoid = reader.GetString(2);
                                    NewStr.ison = reader.GetBoolean(3);
                                    NewStr.Name = reader.GetString(4);
                                    NewStr.GuildsId = reader.GetString(5);
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
                    String[] guilds = str.GuildsId.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

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

                        EmbedBuilder Live = new EmbedBuilder();
                        Live.AddField("Новости", $"Найден стрим у {str.Name}!")
                            .WithColor(ConstVariables.InfoColor);

                        await channel.SendMessageAsync("", embed: Live.Build());
                        await channel.SendMessageAsync("https://www.youtube.com/video/" + stream.Id.VideoId);
                    }
                    else
                    {
                        foreach (var key in ConstVariables.CServer)
                        {
                            if(guilds.Length != 0)
                            {
                                foreach (var par in guilds)
                                    if ((ulong)Convert.ToInt64(par) == key.Key)
                                        continue;
                            }

                            try
                            {
                                SocketTextChannel channel = null;
                                if (key.Value.DefaultChannelNewsId == 0)
                                    channel = key.Value.GetDefaultChannel();
                                else channel = key.Value.GetGuild().GetTextChannel(key.Value.DefaultChannelNewsId);

                                EmbedBuilder Live = new EmbedBuilder();
                                Live.AddField("Новости", $"Найден стрим у {str.Name}!")
                                    .WithColor(ConstVariables.InfoColor);

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

                Thread.Sleep(60000);
            } while (true);
        }

        private static async void Twitch(object obj)
        {
            while (DiscordAPI._Client.ConnectionState != ConnectionState.Connected) ;

            Thread.Sleep(2000);
            ConstVariables.Mess?.Invoke(" Запуск потока: TwitchStream;");
            ConstVariables.logger.Info("Запуск потока: TwitchStream;");

            bool isbegin = false;

            try
            {
                do
                {
                    Streams stream = GetRequest<Streams>("streams/95844270");

                    if (stream.stream != null)
                    {
                        if (!isbegin)
                        {
                            EmbedBuilder Live = new EmbedBuilder();
                            Live.AddField("Новости", $"Найден стрим у {stream.stream.channel.name}!")
                                .WithColor(ConstVariables.InfoColor);

                            foreach (var key in ConstVariables.CServer)
                            {
                                try
                                {
                                    SocketTextChannel channel = null;
                                    if (key.Value.DefaultChannelNewsId == 0)
                                        channel = key.Value.GetDefaultChannel();
                                    else channel = key.Value.GetGuild().GetTextChannel(key.Value.DefaultChannelNewsId);

                                    await channel.SendMessageAsync("@here", embed: Live.Build());
                                    await channel.SendMessageAsync($"{stream.stream.channel.status}");
                                    await channel.SendMessageAsync($"Игра: {stream.stream.game} \r\nСсыль: {stream.stream.channel.url}");
                                }
                                catch (Exception e)
                                {
                                    ConstVariables.logger.Error($"is guild {key.Value.Name} is error {e}");
                                    ConstVariables.Mess($"Youtube: is guild: {key.Key} {e}");
                                }
                            }
                            isbegin = true;
                        }
                    }
                    else isbegin = false;

                    Thread.Sleep(60000);
                } while (true);
            }
            catch (Exception e)
            {
                ConstVariables.logger.Error($"Automatic(Twitch): Errorr {e}");
            }
        }

        private static T GetRequest<T>(string url)
        {
            url = "https://api.twitch.tv/kraken/" + url;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "GET";
            req.Accept = "application/vnd.twitchtv.v5+json";
            req.Headers.Add("Client-ID", Base.Resource2.Cliend_ID);
            req.Headers.Add("Authorization", Base.Resource2.Oauth2);

            string result = "";
            try
            {
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                T par = JsonConvert.DeserializeObject<T>(result);
                return par;
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;
                ConstVariables.logger.Error($"Статусный код ошибки: {(int)webResponse.StatusCode} - {webResponse.StatusCode}");
                return default;
            }
        }
    }
}