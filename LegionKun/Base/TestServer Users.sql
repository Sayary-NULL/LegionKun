/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
USE [TestServer]
/*DELETE FROM [Ban] WHERE [GuildId] = 461284473799966730 AND [BanedId] = 479024685317750796 AND NOT [Id] = 1*/
/*UPDATE [Channel] SET [IsCommand] = 1 WHERE [ChannelId] = 459322800990322689*/
/*DBCC CHECKIDENT ('Ban', RESEED, 0);*/
/*INSERT INTO [Channel] ([GuildId], [ChannelId], [IsDefault], [IsDefaultNews], [IsCommand]) VALUES (423154703354822668, 461599477292204032, 'false', 'false', 'false')*/
/*INSERT INTO [Role] ([GuildId], [RoleId]) VALUES (435485527156981770, 435486880767803413)*/
/*INSERT INTO [Ban] ([GuildId], [BanedId], [AdminId], [Comment], [Date]) VALUES (461284473799966730, 252459542057713665, 397084009185935362, 'Гл. II п. 1 и п. 2', '2018-09-09 00:41:10.563')*/

/*SELECT [Id]
	  ,[GuildId]
	  ,[BanedId]
	  ,[AdminId]
	  ,[Comment]
	  ,[Date]
  FROM [Ban]
  WHERE [GuildId] = 461284473799966730*/

  SELECT * FROM [Channel] WHERE [ChannelId] = 459322800990322689
/*SELECT [Id]
	  ,[GuildId]
	  ,[ChannelId]
	  ,[IsDefault]
	  ,[IsDefaultNews]
	  ,[IsCommand]
  FROM [Channel]*/


/*SELECT [Id]
	  ,[GuildId]
	  ,[RoleId]
	  FROM [Role]*/
	  
/*SELECT [Id]
	  ,[GuildId]
	FROM [GuildId]*/

/*SELECT [Id]
	  ,[GuildId]
	  ,[OwnerId]
	FROM [OwnerId]*/