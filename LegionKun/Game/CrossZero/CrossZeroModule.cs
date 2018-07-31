using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace LegionKun.Game.CrossZero
{
    [Group("CrossZero")]
    public sealed class CrossZeroModule : CrossZeroGame
    {
        [Command("new")]
        public async Task NewGameAsync(SocketUser User2)
        {
            if(!Module.ConstVariables.ThisTest)
            {
                await Context.Channel.SendMessageAsync("В разработке!");
                return;
            }

            if (!DataDictionary.ContainsKey(Context.Channel))
            {
                DataType DBase = new DataType
                {
                    User1 = Context.User,
                    User2 = User2,
                    Channelsgame = Context.Channel,
                    Guild = Context.Guild,
                    Message = null,
                    GameStat = StatGame.Create
                };
                DataDictionary.Add(Context.Channel, DBase);
                await Context.Channel.SendMessageAsync("Создано");
            }
            else await Context.Channel.SendMessageAsync("уже создано!");
        }

        [Command("start")]
        public async Task StartGameAsync()
        {
            if (!Module.ConstVariables.ThisTest)
            {
                await Context.Channel.SendMessageAsync("В разработке!");
                return;
            }


            await Context.Channel.SendMessageAsync("start");
        }

        [Command("status")]
        public async Task StatusGameAync()
        {
            if (!Module.ConstVariables.ThisTest)
            {
                await Context.Channel.SendMessageAsync("В разработке!");
                return;
            }

            Embed embed = StatusGame(Context.Channel);
            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
