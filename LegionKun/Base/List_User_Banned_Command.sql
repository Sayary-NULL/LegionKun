/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
Use [UserBanned]
/*DELETE FROM [UserBanned].[dbo].[List_User_Banned] WHERE [id] = 24*/
/*UPDATE [UserBanned].[dbo].[List_User_Banned] SET [Comment] = 'Донимался до illan syava' WHERE [id] = 8*/
/*DBCC CHECKIDENT ('UserBanned.dbo.List_User_Banned', RESEED, 0);*/
/*INSERT INTO [List_User_Banned] ([GuildId], [BannedId], [AdminId], [Comment]) VALUES (423154703354822668, 468089180665151509, 329653972728020994, 'Доставал админа, ругался')*/
SELECT [id]
	  ,[GuildId]
      ,[BannedId]
      ,[AdminId]
      ,[Comment]
  FROM [List_User_Banned]
 /* WHERE [GuildId] = 435485527156981770 AND [BannedId] = 329653972728020994*/
  