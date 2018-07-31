using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;
using Discord.Rest;
namespace LegionChan
{
    class PLAYER
    {
        public IUser _user;
        public bool isMafia;
        public bool isDead;
        public bool canVote;
        public int votes;
    };


    class Program : LegionChan.Module.ImageGenerator
    {
        //public IUserMessage Message;
        //public ISocketMessageChannel spamChannel;
        public IUser userHost; 
        List<PLAYER> players = new List<PLAYER>();
        List<string> roles = new List<string>();
        ISocketMessageChannel gameChannel;
        public IUserMessage voteMessage;
        public int voted = 0, alive = 0;
        public EmbedBuilder voteBuilder = new EmbedBuilder();

        public DiscordSocketClient _client;
        public CommandService _command;
        public IServiceProvider _service;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
           
            _client = new DiscordSocketClient();
            _command = new CommandService();

            _service = new ServiceCollection().AddSingleton(_client).AddSingleton(_command).BuildServiceProvider();

            string botToken = "NDU4MjgzOTMzNTQ5NzIzNjUw.DhZDBA.D1bgTxGhIdOTrnov-dZ7c8UQ1N8";

            _client.Log += Log;
            _client.MessageReceived += MessegRec;
           

            await RegistredCommandsAsync();

            await _client.LoginAsync(Discord.TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task MessegRec(SocketMessage user)
        {
            var channel = user.Channel;
            if ( user.Content.StartsWith("test"))
            {
                await JailIMage(user.Author);
                await channel.SendFileAsync("jail.jpg");
            }

            if (user.Content == "mafia!start" && (userHost == null ? true: user.Author == userHost) )
            {
                players = new List<PLAYER>();
                roles = new List<string>();
                voted = 0;
                alive = 0;
                voteBuilder = new EmbedBuilder();

                gameChannel = user.Channel;
                userHost = user.Author;
                await gameChannel.SendMessageAsync("Здесь будет игра в мафию.");
                await user.DeleteAsync();
            }

            if(user.Content.StartsWith("mafia!addme") && user.Channel == gameChannel)
            {
                players.Add(new PLAYER() { _user = user.Author, isMafia = false, canVote = false, votes = 0 });

                await gameChannel.SendMessageAsync(user.Author.Mention + " добавлен в игру.");

                await user.DeleteAsync();
            }
            if ( user.Content.StartsWith("mafia!kick") && user.Channel == gameChannel && userHost == user.Author)
            {
                players.RemoveAt(Int32.Parse(user.Content.Substring(11) ) - 1 );

                await user.DeleteAsync();
            }
            if (user.Content.StartsWith("addroles ") && userHost == user.Author)
            {
                string text = user.Content.Substring(9);
                string[] words = text.Split(new char[] { ' ' });

                foreach ( string s in words)
                {
                    roles.Add(s);
                }

            }
            if (user.Content == "rlist" && userHost == user.Author)
            {
                var playersList = new EmbedBuilder();
                playersList.WithTitle("Список всех ролей");
                playersList.WithDescription("");
                foreach (string s in roles)
                {
                    playersList.WithDescription(playersList.Description + s + "\n");
                }

                await gameChannel.SendMessageAsync("", false, playersList.Build());
                await user.DeleteAsync();
            }
            if (user.Content == "plist" && userHost == user.Author)
            {
                var playersList = new EmbedBuilder();
                int i = 1;
                string aliveText = "";
                string playerList = "";

                foreach (PLAYER p in players)
                {
                    playerList = playerList + i++ + "." + p._user.Username + "\n";
                    //playersList.WithDescription($"{playersList.Description}{i++}.{p._user.Username}\n");
                    if (p.isDead) aliveText = aliveText + "Мертв\n" ;
                    else aliveText = aliveText + "Жив\n";
                }
                playersList.AddInlineField("Игроки", playerList).AddInlineField("Состояние игрока", aliveText);


                await gameChannel.SendMessageAsync("", false, playersList.Build());
                await user.DeleteAsync();
            }
            if( user.Content == "sendroles" && userHost == user.Author)
            {
                Random ran = new Random();
                foreach (PLAYER p in players)
                {
                    int index = ran.Next(roles.Count);
                    await UserExtensions.SendMessageAsync(p._user, "Твоя роль: " + roles[index]);
                    await UserExtensions.SendMessageAsync(userHost, p._user.Username + " - " + roles[index]);
                    if (roles[index] == "Мафия" || roles[index] == "Дон")
                    {
                        p.isMafia = true;
                    }
                    roles.RemoveAt(index);
                }

                List<PLAYER> mafias = new List<PLAYER>();
                foreach(PLAYER p in players)
                {
                    if (p.isMafia)
                        mafias.Add(p);
                }

                string mafiasList = "Твоя команда мафиози:";
                foreach(PLAYER p in mafias)
                {
                    mafiasList = mafiasList + p._user.Username + ", ";
                }
                foreach(PLAYER p in mafias)
                {
                    await UserExtensions.SendMessageAsync(p._user, mafiasList);
                }
                
            }

            if (user.Content.StartsWith("mafia!kill") && userHost == user.Author)
            {
                string number = user.Content.Substring(11);
                Console.WriteLine(number);
                players[Int32.Parse(number) - 1].isDead = true;
            }
            //if( user.Content.StartsWith("send!"))
            //{
            //    await UserExtensions.SendMessageAsync(User, user.Content.Substring(5));
            //}

            if (user.Content == "mafia!votestart" && userHost == user.Author)
            {
                voteBuilder = new EmbedBuilder();
                foreach (PLAYER p in players)
                {
                    p.votes = 0;

                    if (!p.isDead)
                    {
                        alive++;
                        p.canVote = true;
                    }
                }
                    
                voted = 0;

                int i = 1;
                string aliveText = "";
                string playerList = "";
                string votesList = "";

                foreach (PLAYER p in players)
                {
                    playerList = playerList + i++ + "." + p._user.Username + "\n";
                    //playersList.WithDescription($"{playersList.Description}{i++}.{p._user.Username}\n");
                    if (p.isDead) aliveText = aliveText + "Мертв\n";
                    else aliveText = aliveText + "Жив\n";
                    votesList = votesList + p.votes + "\n";
                }
                voteBuilder.AddInlineField("Игроки", playerList).AddInlineField("Состояние игрока", aliveText).AddInlineField("Голоса", votesList);

                voteMessage = await gameChannel.SendMessageAsync("", false, voteBuilder.Build());

                await user.DeleteAsync();
            }
            if (user.Content.StartsWith( "mafia!vote" ) && voted != alive )
            {
                voteBuilder = new EmbedBuilder();
                foreach ( PLAYER p in players)
                {
                    if (user.Author == p._user)
                    {
                        if(p.canVote)
                        {
                            players[Int32.Parse(user.Content.Substring(11)) - 1 ].votes++;
                            p.canVote = false;
                            voted++;
                        }
                    }
                }

                int i = 1;
                string aliveText = "";
                string playerList = "";
                string votesList = "";

                foreach (PLAYER p in players)
                {
                    playerList = playerList + i++ + "." + p._user.Username + "\n";
                    //playersList.WithDescription($"{playersList.Description}{i++}.{p._user.Username}\n");
                    if (p.isDead) aliveText = aliveText + "Мертв\n";
                    else aliveText = aliveText + "Жив\n";
                    votesList = votesList + p.votes + "\n";
                }
                voteBuilder.AddInlineField("Игроки", playerList).AddInlineField("Состояние игрока", aliveText).AddInlineField("Голоса", votesList);

                await voteMessage.ModifyAsync(msg => msg.Embed = voteBuilder.Build());

                await user.DeleteAsync();
            }
            // vote result
            if ( voted == alive && voted != 0)
            {
                int minpos = 0;
                bool isDouble = false;

                for ( int i = 1; i < players.Count; i++)
                {
                    if (players[i].votes > players[minpos].votes)
                        minpos = i;
                }

                for ( int i = 0; i < players.Count; i++)
                {
                    if (i != minpos && players[i].votes == players[minpos].votes)
                    {
                        isDouble = true;
                        await gameChannel.SendMessageAsync(players[i]._user.Username + " и " + players[minpos]._user.Username + " имеют одинаковое кол-во голосов!");
                    }
                }

                if(!isDouble)
                {
                    players[minpos].isDead = true;
                    await JailIMage(players[minpos]._user);
                    await gameChannel.SendFileAsync("jail.jpg", players[minpos]._user.Username + " был отправлен за решетку!");
                    
                }
                voted = 0;
                alive = 0;
            }

            
        }

        private async Task Userjoin(SocketGuildUser user)
        {
            var guid = user.Guild;
            var channel = guid.DefaultChannel;
            await channel.SendMessageAsync($"Welcom, {user.Mention}!");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegistredCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _command.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var Messeg = arg as SocketUserMessage;

            if (Messeg is null /*|| Messege.Author.IsBot */ )
                return;
            int argPos = 0;

            if (Messeg.HasStringPrefix("=", ref argPos) || Messeg.HasStringPrefix("-", ref argPos) || Messeg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var contex = new SocketCommandContext(_client, Messeg);

                var result = await _command.ExecuteAsync(contex, argPos, _service);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
