using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using LegionKun.BotAPI;
using LegionKun.Attribute;
using System.Data.SqlClient;

namespace LegionKun.Module
{
    [OwnerOnly]
    class OwnerCommands : InteractiveBase
    {
        private struct SLog
        {
            string _group;
            string _command;
            SocketCommandContext Context;
            public Exception _exception;
            public string _addcondition;

            public SLog(string command, SocketCommandContext context)
            {
                _group = "Admin";
                _command = command;
                Context = context;
                _exception = null;
                _addcondition = "";
            }

            public void PrintLog()
            {
                if (_exception == null)
                    ConstVariables.Logger.Info($"is group '{_group}' is command '{_command}' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'{_addcondition}");
                else ConstVariables.Logger.Error($"is group '{_group}' is command '{_command}' is guild '{Context.Guild.Name}' is channel '{Context.Channel.Name}' is user '{Context.User.Username}#{Context.User.Discriminator}'{_addcondition} is errors {_exception}");
            }
        }
        
        [Command("allnews", RunMode = RunMode.Async)]
        public async Task AllNewsAsync([Remainder]string mess)
        {
            SLog logger = new SLog("AllNews", Context);

            string URL = ConstVariables.Patch + "Base/news26052017.jpg";

            bool islocalfile = true;

            if (mess.IndexOf("image:") == 0)
            {
                mess = mess.Remove(0, 6);
                int tchk = mess.IndexOf(';');
                URL = mess.Substring(0, tchk);
                mess = mess.Remove(0, tchk + 2);
                islocalfile = false;
            }

            var result = await Context.Channel.SendMessageAsync("Sayary, жди. Начата рассылка");

            foreach (var server in ConstVariables.CServer)
            {
                if (server.Value.DefaultChannelNewsId == 0)
                    continue;
                
                try
                {
                    ConstVariables.CDiscord guild = server.Value;

                    if (ConstVariables.ThisTest)
                    {
                        if (server.Value.GuildId != 435485527156981770)
                            continue;

                        if (islocalfile)
                            await guild.GetDefaultNewsChannel().SendFileAsync(URL, " ");
                        else await guild.GetDefaultNewsChannel().SendMessageAsync(URL);

                        await guild.GetDefaultNewsChannel().SendMessageAsync(mess);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                        break;
                    }
                    else
                    {
                        if(islocalfile)
                            await guild.GetDefaultNewsChannel().SendFileAsync(URL, " ");
                        else await guild.GetDefaultNewsChannel().SendMessageAsync(URL);

                        await guild.GetDefaultNewsChannel().SendMessageAsync(mess);
                        await guild.GetDefaultNewsChannel().SendMessageAsync("Автор: " + Context.User.Mention);
                    }
                }
                catch (Exception e)
                {
                    logger._exception = e;
                }
            }

            try
            {
                await Context.Message.DeleteAsync();
                await result.DeleteAsync();
            }
            catch(Exception e)
            {
                logger._exception = e;
            }

            logger.PrintLog();
        }

        [Command("debug")]
        public async Task DebugAsync()
        {
            SLog logger = new SLog("Debug", Context);

            ConstVariables.CServer[Context.Guild.Id].Debug = !ConstVariables.CServer[Context.Guild.Id].Debug;

            await ReplyAsync($"Режим дебага: {ConstVariables.CServer[Context.Guild.Id].Debug}");

            logger._addcondition = $" is result '{(ConstVariables.CServer[Context.Guild.Id].Debug ? "on" : "off")}'";
            logger.PrintLog();
        }

        [Command("selecttriggerdefault")]
        public async Task SelectTriggerDefaultAsync()
        {
            SLog logger = new SLog("SelectTriggerDefault", Context);

            string SqlExpression = "sp_SelectTriggers";

            string answer = ConstVariables.NCR;
            int count = 1;

            SqlParameter GuildIdParam = new SqlParameter
            {
                ParameterName = "@GuildID",
                DbType = System.Data.DbType.Int64,
                Value = 0
            };

            using (SqlConnection conect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                try
                {
                    conect.Open();

                    using (SqlCommand command = new SqlCommand(SqlExpression, conect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        command.Parameters.Add(GuildIdParam);

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            answer = "";
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string textsec = reader.GetString(1);
                                string textanswer = reader.GetString(2);

                                string text = $"{count++}) **id trigger**: {id}\r\n{CountInterval(count)}**text search**: '{textsec}'\r\n{CountInterval(count)}**text anwser**: '{textanswer}'\r\n";

                                if (answer.Length + text.Length >= 2000)
                                {
                                    await ReplyAsync(answer);
                                    answer = text;
                                }
                                else answer += text;
                            }
                            reader.Close();
                            await ReplyAsync(answer);
                        }
                        else await ReplyAsync("триггеров не найдено!");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync("Ошибка чтения! Обратитесь к администратору!");
                    logger._exception = e;
                }

                logger.PrintLog();
            }
        }
            
        private string CountInterval(int number)
        {
            double interv = Math.Log10(number) + 1;
            string rez = "";
            interv = Math.Truncate(interv) * 2;

            for (int i = 0; i < interv + 2; i++)
                rez += " ";

            return rez;
        }
    }
}
