﻿using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using LegionKun.Attribute;
using System.Data.SqlClient;
using System.Collections.Generic;
using LegionKun.Module;

//UCDnNz_stjQqcikCvIF2NTAw - Kanade
//UCScLnRAwAT2qyNcvaFSFvYA - Sharon
//UCuF8ghQWaa7K-28llm-K3Zg - Anilibria.TV
//WIN-FCZUC9ET6YV\LEGIONKUN
//KIRILL02\LEGIONKUN
namespace LegionKun.Tests
{
    [Group("tests")]
    [Tests, OwnerOnly]
    class TestClass : InteractiveBase
    {
        public struct MyStruct
        {
            public ulong GuildID;
            public string TextRequest;
            public string TextAnswer;
            public string Condition;
        }

        [Command]
        public async Task TestAsync()
        {
            string titl = null, body = null;
            ulong ad = 0;

            using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
            {
                connect.Open();

                using (SqlCommand command = new SqlCommand("sp_GetGoodbyeText", connect) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    SqlParameter Vparam = new SqlParameter()
                    {
                        ParameterName = "@GuildId",
                        DbType = System.Data.DbType.Int64,
                        Value = Context.Guild.Id
                    };

                    command.Parameters.Add(Vparam);
                    SqlDataReader reader = command.ExecuteReader();

                    if(reader.HasRows)
                    {
                        reader.Read();
                        titl = reader.GetString(0);
                        body = reader.GetString(1);
                        reader.Close();
                    }
                }
            }
            await ReplyAsync($"{titl} {body}");
        }

        [Command("connect")]
        public async Task ConnectAsync([Remainder]string words)
        {
            string SqlExpression = "sp_GetIndexOfText";
            MyStruct result = new MyStruct();
            bool isflag = false;

            try
            {
                using (SqlConnection connect = new SqlConnection(ConstVariables.DateBase.ConnectionStringKey))
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand(SqlExpression, connect) { CommandType = System.Data.CommandType.StoredProcedure })
                    {                        
                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.GuildID = (ulong)reader.GetInt64(1);
                                result.TextRequest = reader.GetString(2);
                                result.TextAnswer = reader.GetString(3);
                                result.Condition = reader.GetString(4);

                                if(words.IndexOf(result.TextRequest) > -1)
                                {
                                    isflag = true;
                                    break;
                                }
                            }
                            reader.Close();
                        }

                        if (isflag)
                            await Context.Channel.SendMessageAsync($"'{result.GuildID}' '{result.TextRequest}' '{result.TextAnswer}' '{result.Condition}'");
                        else await Context.Channel.SendMessageAsync($"Not result");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connect: " + e);
            }
        }        
    }
}