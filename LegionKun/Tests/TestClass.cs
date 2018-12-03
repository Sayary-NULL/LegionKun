using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using LegionKun.Attribute;
using System.Data.SqlClient;

//UCDnNz_stjQqcikCvIF2NTAw - Kanade
//UCScLnRAwAT2qyNcvaFSFvYA - Sharon
//UCuF8ghQWaa7K-28llm-K3Zg - Anilibria.TV
namespace LegionKun.Tests
{
    [Group("tests")]
    [Tests, OwnerOnly]
    class TestClass : InteractiveBase
    {
        [Command("test")]/*Произведено исправление[100]*/
        public async Task TestAsync([Remainder]string URL)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithImageUrl(URL);

            //var mess = await ReplyAsync($"{Context.User.Mention}, :grin:");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("connect")]
        [CategoryChannel(IC:true)]
        public async Task ConnectAsync()
        {
            string SqlExpression = "sp_GetUserBaned";
            string TextRequest = "";

            using (SqlConnection connect = new SqlConnection(Base.Resource1.ConnectionKeyTestServer))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand(SqlExpression, connect)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    SqlParameter GuildIdParam = new SqlParameter
                    {
                        ParameterName = "@GuildId",
                        DbType = System.Data.DbType.Int64,
                        Value = Context.Guild.Id
                    };

                    command.Parameters.Add(GuildIdParam);
                    
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ulong BannedId = (ulong)reader.GetInt64(0);
                            ulong AdminId = (ulong)reader.GetInt64(1);
                            string Comment = reader.GetString(2);

                            TextRequest += $"**Пользователь:** {Context.Guild.GetUser(BannedId).Mention} **Администратор:** {Context.Guild.GetUser(AdminId).Mention}\r\n Заметка: {Comment}\r\n";
                        }

                        await ReplyAsync(TextRequest);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Connect: " + e);
                }
                
            }
        }

        [Command("stop")]
        public async Task StopAsync()
        {
            await Module.ConstVariables._Client.StopAsync();
        }
    }
}