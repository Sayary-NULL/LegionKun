/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
USE [TestServer]
/*DELETE FROM [Ban] WHERE [Id] = 3*/
/*UPDATE [Channel] SET [IsCommand] = 'true' WHERE [ChannelId] = 461599477292204032*/
/*DBCC CHECKIDENT ('Ban', RESEED, 0);*/
/*INSERT INTO [Channel] ([GuildId], [ChannelId], [IsDefault], [IsDefaultNews], [IsCommand]) VALUES (423154703354822668, 461599477292204032, 'false', 'false', 'false')*/
/*INSERT INTO [Role] ([GuildId], [RoleId]) VALUES (435485527156981770, 435486880767803413)*/
/*INSERT INTO [Ban] ([GuildId], [BanedId], [AdminId], [Comment], [Date]) VALUES (435485527156981770, 435486880767803413, 435486880767803413, 'Отсутствует', )*/

SELECT [Id]
	  ,[GuildId]
	  ,[BanedId]
	  ,[AdminId]
	  ,[Comment]
	  ,[Date]
  FROM [Ban]

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