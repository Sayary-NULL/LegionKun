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

        public void OneMinStart()
        {
            OneMinTimer.Start();
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
            Module.ConstVariables.Trigger = false;
        }

        private static void MainFunc(object obj)
        {

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
                }//else Messege($"Time: {time.Hours}");

                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
                Thread.Sleep(900000);
            }
        }

        private static async void Youtube(object obj)
        {
            Thread.Sleep(60000);
            Console.WriteLine("Start thread");

            Module.ConstVariables.CDiscord guild = Module.ConstVariables.CServer[423154703354822668];
            SocketTextChannel channel = null;

            if (Module.ConstVariables.ThisTest)
            {
                /*Console.WriteLine("Это тестовый бот! Поток не запущен!");
                return;*/
                guild = Module.ConstVariables.CServer[435485527156981770];
                channel = guild.GetDefaultChannel();
            }
            else channel = guild.GetDefaultNewsChannel();

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithFooter(guild.Name, guild.GetGuild().IconUrl);
            builder.AddField("Новости", $"@here, у Генерала найден стрим!");
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
                    if ((url != "") || (url2 != ""))
                    {
                        await channel.SendMessageAsync("", false, builder.Build());
                    }

                    if (url != "")
                    {
                        await channel.SendMessageAsync("https://www.youtube.com/video/" + url);
                        Module.ConstVariables.Video1Id = url;
                         url = "";
                    }

                    if (url2 != "")
                    {
                        await channel.SendMessageAsync("https://www.youtube.com/video/" + url2);
                        Module.ConstVariables.Video2Id = url2;
                        url2 = "";
                    }
                }
                Thread.Sleep(60000);
            } while (true);
        }
    }
}
