using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LegionKun.Module
{
    public class MainClass
    {
        private readonly ThreadClass threadclass = new ThreadClass();

        public void OneMin(ulong guildId) => threadclass.OneMinStart(guildId);        

        public void MainTime() => threadclass.MainTimerStart();        

        public void Youtube() => threadclass.YoutubeStart();
        
        public virtual void Messege(string str)
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            Console.WriteLine($"[{time.Hours}:{time.Minutes}:{time.Seconds}.{time.Milliseconds}] {str}");
        }
    }
}
