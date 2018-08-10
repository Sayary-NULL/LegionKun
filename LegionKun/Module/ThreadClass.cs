﻿using Discord;
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
            ConstVariables.Log?.Invoke(" Запуск потока: MainFunc;");
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

                    Module.ConstVariables.Mess?.Invoke($"[{time.Hours}:{time.Minutes}:{time.Seconds}] произведен сброс!");
                    Module.ConstVariables.Log?.Invoke(" произведен сброс!");
                }

                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
            }
        }

        private static async void Youtube(object obj)
        {
            if(ConstVariables.ThisTest)
            {
                return;
            }

            while(ConstVariables._Client.ConnectionState != ConnectionState.Connected)
            {
            }

            Thread.Sleep(2000);

            ConstVariables.Mess?.Invoke(" Запуск потока: YouTubeStream;");
            ConstVariables.Log?.Invoke(" Запуск потока: YouTubeStream;");

            EmbedBuilder Live = new EmbedBuilder();
            Live.AddField("Новости", "у Генерала найден стрим!")
                .WithColor(ConstVariables.InfoColor);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Base.Resource1.ApiKeyToken,
                ApplicationName = "Legion-kun"
            });            

            var SharonRequest = youtubeService.Search.List("snippet");
            SharonRequest.Type = "video";
            SharonRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            SharonRequest.ChannelId = "UCScLnRAwAT2qyNcvaFSFvYA";
            SharonRequest.MaxResults = 1;

            var DejzRequest = youtubeService.Search.List("snippet");
            DejzRequest.Type = "video";
            DejzRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            DejzRequest.ChannelId = "UCbGMUOX-6gsC2q75x3YS0gw";
            DejzRequest.MaxResults = 1;

            do
            {
                SearchListResponse SharonResponse = await SharonRequest.ExecuteAsync();
                SearchListResponse DejzResponse = await DejzRequest.ExecuteAsync();

                SearchResult SharoHH = null;
                SearchResult Dejz = null;

                if ((SharonResponse.Items.Count != 0) || (DejzResponse.Items.Count != 0))
                {
                    if (SharonResponse.Items.Count != 0)
                    {
                        SharoHH = SharonResponse.Items[0];
                    }

                    if (DejzResponse.Items.Count != 0)
                    {
                        Dejz = DejzResponse.Items[0];
                    }

                    if (ConstVariables.ThisTest)
                    {
                        ConstVariables.CDiscord guild = ConstVariables.CServer[435485527156981770];
                        SocketTextChannel channel = guild.GetDefaultChannel();

                        await channel.SendMessageAsync("", embed: Live.Build());

                        if (SharonResponse.Items.Count != 0)
                        {
                            await channel.SendMessageAsync("https://www.youtube.com/video/" + SharoHH.Id.VideoId);
                        }

                        if (DejzResponse.Items.Count != 0)
                        {
                            await channel.SendMessageAsync("https://www.youtube.com/video/" + Dejz.Id.VideoId);
                        }
                    }
                    else
                    {
                        foreach (var key in ConstVariables.CServer)
                        {
                            SocketTextChannel channel = null;
                            if (key.Value.DefaultChannelNewsId == 0)
                            {
                                channel = key.Value.GetDefaultChannel();
                            }
                            else channel = key.Value.GetDefaultNewsChannel();

                            if ((SharonResponse.Items.Count != 0) && (ConstVariables.Video1Id != SharoHH.Id.VideoId))
                            {
                                await key.Value.GetDefaultNewsChannel().SendMessageAsync("", embed: Live.Build());
                                await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + SharoHH.Id.VideoId);
                                ConstVariables.Video1Id = SharoHH.Id.VideoId;
                            }

                            if ((DejzResponse.Items.Count != 0) && (ConstVariables.Video2Id != Dejz.Id.VideoId))
                            {
                                await key.Value.GetDefaultNewsChannel().SendMessageAsync("", embed: Live.Build());
                                await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + Dejz.Id.VideoId);
                                ConstVariables.Video2Id = Dejz.Id.VideoId;
                            }
                        }
                    }
                }

                Thread.Sleep(60000);
            } while (true);
        }
    }
}
