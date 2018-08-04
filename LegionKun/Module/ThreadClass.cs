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
                    }
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

            Thread.Sleep(60000);
            ConstVariables.Mess?.Invoke(" Запуск потока: YouTubeStream;");
            ConstVariables.Log?.Invoke(" Запуск потока: YouTubeStream;");

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[423154703354822668];
            SocketTextChannel channel = null;

            if (Module.ConstVariables.ThisTest)
            {
                guild = Module.ConstVariables.CServer[435485527156981770];
                channel = guild.GetDefaultChannel();
            }
            else channel = guild.GetDefaultNewsChannel();

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithFooter(guild.Name, guild.GetGuild().IconUrl);
            builder.AddField("Новости", "У Генерала найден стрим!");
            builder.WithColor(Discord.Color.Red);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDIuH33zi6aod6jSHm31V1VIVKYIIGxvEo",
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
                // Call the search.list method to retrieve results matching the specified query term.
                SearchListResponse SharonResponse = await SharonRequest.ExecuteAsync();
                SearchListResponse DejzResponse = await DejzRequest.ExecuteAsync();
                
                string url = "";
                string url2 = "";
                // Add each result to the appropriate list, and then display the lists of.
                foreach (var searchResult in SharonResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        url = searchResult.Id.VideoId;
                        break;
                    }
                }

                foreach (var searchResult in DejzResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        url2 = searchResult.Id.VideoId;
                        break;
                    }
                }

                if ((url != Module.ConstVariables.Video1Id) || (url2 != Module.ConstVariables.Video2Id))
                {
                    foreach(KeyValuePair<ulong, ConstVariables.CDiscord> key in ConstVariables.CServer)
                    {
                        if ((url != "") || (url2 != ""))
                        {
                            await key.Value.GetDefaultNewsChannel().SendMessageAsync("@here", false, builder.Build());
                        }

                        if (url != "")
                        {
                            await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + url);
                            Module.ConstVariables.Video1Id = url;                            
                            url = "";
                        }

                        if (url2 != "")
                        {
                            await key.Value.GetDefaultNewsChannel().SendMessageAsync("https://www.youtube.com/video/" + url2);
                            Module.ConstVariables.Video2Id = url2;                            
                            url2 = "";
                        }
                    }

                    if(url != "")
                    {
                        ConstVariables.Log?.Invoke(" Найдено соответствие с запростом url1: " + "https://www.youtube.com/video/" + url);
                    }

                    if (url2 != "")
                    {
                        ConstVariables.Log?.Invoke(" Найдено соответствие с запростом url1: " + "https://www.youtube.com/video/" + url2);
                    }

                }
                Thread.Sleep(60000);
            } while (true);
        }
    }
}
