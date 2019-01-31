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

        private static async void Youtube(object obj)
        {
            if (ConstVariables.ThisTest)
                return;

            while (ConstVariables._Client.ConnectionState != ConnectionState.Connected);

            Thread.Sleep(2000);

            ConstVariables.Mess?.Invoke(" Запуск потока: YouTubeStream;");
            ConstVariables.logger.Info("Запуск потока: YouTubeStream;");

            EmbedBuilder Live = new EmbedBuilder();
            Live.AddField("Новости", "у Генерала найден стрим!")
                .WithColor(ConstVariables.InfoColor);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Base.Resource1.ApiKeyToken,
                ApplicationName = "Legion-kun"
            });

            SearchResource.ListRequest SharonRequest = youtubeService.Search.List("snippet");
            SharonRequest.Type = "video";
            SharonRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            SharonRequest.ChannelId = "UCScLnRAwAT2qyNcvaFSFvYA";
            SharonRequest.MaxResults = 1;

            SearchResource.ListRequest DejzRequest = youtubeService.Search.List("snippet");
            DejzRequest.Type = "video";
            DejzRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            DejzRequest.ChannelId = "UCbGMUOX-6gsC2q75x3YS0gw";
            DejzRequest.MaxResults = 1;

            do
            {
                if(ConstVariables.ControlFlow)
                {
                    Thread.Sleep(60000);
                    continue;
                }

                string SSharoHH = "-", SDejz = "-";

                try
                {
                    SSharoHH = ConstVariables.GetVideo1(1);
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error(e.Message);
                    Thread.Sleep(60000);
                    continue;
                }

                try
                {
                    SDejz = ConstVariables.GetVideo1(2);
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error(e.Message);
                    Thread.Sleep(60000);
                    continue;
                }

                SearchListResponse SharonResponse = null;
                SearchListResponse DejzResponse = null;

                try
                {
                    SharonResponse = await SharonRequest.ExecuteAsync();
                    DejzResponse = await DejzRequest.ExecuteAsync();
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error(e.Message);
                    Thread.Sleep(60000);
                    continue;
                }

                SearchResult SharoHH = null;
                SearchResult Dejz = null;

                if ((SharonResponse.Items.Count != 0) || (DejzResponse.Items.Count != 0))
                {
                    if (SharonResponse.Items.Count != 0)
                    {
                        SharoHH = SharonResponse.Items[0];

                        if(SSharoHH != SharoHH.Id.VideoId)
                            ConstVariables.logger.Info("Стрим найден! Sharon: https://www.youtube.com/video/" + $"{SharoHH.Id.VideoId}");
                    }

                    if (DejzResponse.Items.Count != 0)
                    {
                        Dejz = DejzResponse.Items[0];

                        if (SDejz != Dejz.Id.VideoId)
                            ConstVariables.logger.Info("Стрим найден! Dejz: https://www.youtube.com/video/" + $"{Dejz.Id.VideoId}");
                    }

                    if (ConstVariables.ThisTest)
                    {
                        ConstVariables.CDiscord guild = ConstVariables.CServer[435485527156981770];
                        SocketTextChannel channel = guild.GetGuild().GetTextChannel(444152623319482378);

                        await channel.SendMessageAsync("", embed: Live.Build());

                        if ((SharonResponse.Items.Count != 0) && (SSharoHH != SharoHH.Id.VideoId))
                        {
                            await channel.SendMessageAsync("https://www.youtube.com/video/" + SharoHH.Id.VideoId);
                            ConstVariables.UpdVideo(1, SharoHH.Id.VideoId);
                        }

                        if ((DejzResponse.Items.Count != 0) && (SDejz != Dejz.Id.VideoId))
                        {
                            await channel.SendMessageAsync("https://www.youtube.com/video/" + Dejz.Id.VideoId);
                            ConstVariables.UpdVideo(2, Dejz.Id.VideoId);
                        }
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

                                if ((SharonResponse.Items.Count != 0) && (SSharoHH != SharoHH.Id.VideoId))
                                {
                                    await key.Value.GetDefaultNewsChannel().SendMessageAsync("@here", embed: Live.Build());
                                    await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + SharoHH.Id.VideoId);
                                }

                                if ((DejzResponse.Items.Count != 0) && (SDejz != Dejz.Id.VideoId))
                                {
                                    await key.Value.GetDefaultNewsChannel().SendMessageAsync("@here", embed: Live.Build());
                                    await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + Dejz.Id.VideoId);
                                }

                                //ConstVariables.logger.Info($"is guild {key.Value.Name} is channel {channel.Name}");
                            }
                            catch(Exception e)
                            {
                                ConstVariables.logger.Error($"is guild {key.Value.Name} is error {e}");
                                ConstVariables.Mess($"Youtube: is guild: {key.Key} {e}");
                            }
                        }

                        if((SharonResponse.Items.Count != 0) && (SSharoHH != SharoHH.Id.VideoId))
                        {
                            ConstVariables.UpdVideo(1, SharoHH.Id.VideoId);
                        }

                        if ((DejzResponse.Items.Count != 0) && (SDejz != Dejz.Id.VideoId))
                        {
                            ConstVariables.UpdVideo(2, Dejz.Id.VideoId);
                        }
                    }
                }

                Thread.Sleep(30000);
            } while (true);
        }
    }
}