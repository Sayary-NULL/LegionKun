/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
Use [TestServer]
/*DELETE FROM [Ban] WHERE [id] = 4023*/
/*UPDATE [Ban] SET [AdminId] = 274906688996245505 WHERE [id] = 4023*/
/*DBCC CHECKIDENT ('UserBanned.dbo.List_User_Banned', RESEED, 0);*/
/*INSERT INTO [List_User_Banned] ([GuildId], [BannedId], [AdminId], [Comment]) VALUES (423154703354822668, 468089180665151509, 329653972728020994, 'Доставал админа, ругался')*/
SELECT [id]
	  ,[GuildId]
      ,[BanedId]
      ,[AdminId]
      ,[Comment]
  FROM [Ban]
 /* WHERE [GuildId] = 435485527156981770 AND [BannedId] = 329653972728020994*/
  