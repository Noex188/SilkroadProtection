USE [SRO_R_Accountdb]
GO
/****** Object:  Table [dbo].[_AnticheatArenaStatus]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_AnticheatArenaStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[char_name] [varchar](50) NULL,
	[status] [int] NULL,
 CONSTRAINT [PK__AnticheatArenaStatusNotify] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_AnticheatAccount]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_AnticheatAccount](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[userName] [varchar](50) NOT NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK__AnticheatAccount] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatBicycle]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[_AnticheatBicycle]   
AS
select 'OK';
GO
/****** Object:  Table [dbo].[_AnticheatGmAccessObjID]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_AnticheatGmAccessObjID](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](128) NOT NULL,
	[obj_id] [int] NOT NULL,
	[amount] [int] NOT NULL,
	[service] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_AnticheatGmAccessControl]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_AnticheatGmAccessControl](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](max) NULL,
	[cmd_id] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_AnticheatUniqueDeath]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_AnticheatUniqueDeath](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](128) NULL,
	[mob_id] [int] NULL,
	[date] [datetime] NOT NULL,
 CONSTRAINT [PK__AnticheatUniqueDath] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatUniqueDathNotify]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create procedure [dbo].[_AnticheatUniqueDathNotify]
	@name	varchar(128),
    @id	int
as
	INSERT INTO _AnticheatUniqueDath([name],[mob_id],[date]) VALUES(@name,@id,GETDATE());
GO
/****** Object:  Table [dbo].[_AnticheatIPLockDown]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_AnticheatIPLockDown](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](128) NOT NULL,
	[IP] [varchar](128) NOT NULL,
 CONSTRAINT [PK__AnticheatIPLockDown] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sroprot_login_log]    Script Date: 07/06/2015 22:52:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sroprot_login_log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StrUserID] [varchar](128) NOT NULL,
	[IP] [varchar](128) NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_sroprot_login_log] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[_GetSilk]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[_GetSilk]
/*@JID int*/
@username varchar(max)
as
DECLARE @JID int
DECLARE @silk_own int
DECLARE @silk_gift int 
DECLARE @silk_point int

if(NOT EXISTS(select * from TB_User where StrUserID = @username))
begin
set  @silk_own   = 0;
set  @silk_gift  = 0;
set  @silk_point = 0;
end

select @JID = JID from TB_User  where StrUserID = @username


if(NOT EXISTS(select * from sk_silk where JID = @JID))
begin
set  @silk_own   = 0;
set  @silk_gift  = 0;
set  @silk_point = 0;
end


select @silk_own = silk_own,@silk_gift = silk_gift,@silk_point = silk_point from sk_silk where JID = @JID


select @silk_own,@silk_gift,@silk_point
GO
/****** Object:  StoredProcedure [dbo].[_web_mall_token]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[_web_mall_token]
@StrUserID varchar(max)
as
DECLARE @JID int
DECLARE @TOKEN varchar(max)

if(EXISTS (select * from TB_User where StrUserID = @StrUserID))
begin
select @JID = JID from TB_User where StrUserID = @StrUserID
end
else
begin
set @JID = 0;
end
if(EXISTS(select * from WEB_ITEM_CERTIFYKEY where UserJID = @JID))
begin
delete from WEB_ITEM_CERTIFYKEY where UserJID = @JID
end

set @TOKEN = CONVERT(VARCHAR(max), HashBytes('MD5', CONVERT(varchar,rand()+GETDATE()+@JID)), 2)

INSERT INTO WEB_ITEM_CERTIFYKEY VALUES (@JID,@TOKEN,64,GETDATE(),0)


select @JID,@TOKEN;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatUniqueDeathNotify]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE procedure [dbo].[_AnticheatUniqueDeathNotify]
	@name	varchar(128),
    @id	int
as
	INSERT INTO _AnticheatUniqueDeath([name],[mob_id],[date]) VALUES(@name,@id,GETDATE());
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatGuild]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE procedure [dbo].[_AnticheatGuild]
	@name	varchar(128),
	@type int,
	@max int
	
as
    DECLARE @result int = 0
    DECLARE @GuildID int
    DECLARE @UnionID int
    DECLARE @Uion_1 int
    DECLARE @Uion_2 int
    DECLARE @Uion_3 int
    DECLARE @Uion_4 int
    DECLARE @Uion_5 int
    DECLARE @Uion_6 int
    DECLARE @Uion_7 int
    DECLARE @Uion_8 int
    DECLARE @Count int = 0
	IF EXISTS (SELECT * FROM SRO_VT_SHARD.dbo._Char WHERE CharName16 = @name and GuildID > 0)
	BEGIN
	SELECT @GuildID = GuildID FROM SRO_VT_SHARD.dbo._Char WHERE CharName16 = @name
	IF(@type = 1)
	BEGIN
	IF((SELECT COUNT(*) FROM SRO_VT_SHARD.dbo._GuildMember WHERE GuildID = @GuildID) < @max)
	BEGIN
	SET @result =1;
	END
	END	
	IF(@type = 2)
	BEGIN
	IF EXISTS(SELECT * FROM SRO_VT_SHARD.dbo._Guild WHERE ID = @GuildID and Alliance > 0)
	BEGIN
	SELECT @UnionID = Alliance FROM SRO_VT_SHARD.dbo._Guild WHERE ID = @GuildID
	print @UnionID;
	SELECT @Uion_1 = Ally1,@Uion_2 = Ally2,@Uion_3 = Ally3,@Uion_4 = Ally4,@Uion_5 = Ally5,@Uion_6 = Ally6,@Uion_7 = Ally7,@Uion_8 = Ally8 FROM SRO_VT_SHARD.dbo._AlliedClans where ID = @UnionID
	
	IF(@Uion_1 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_2 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_3 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_4 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_5 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_6 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_7 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF(@Uion_8 = 0)
	BEGIN
	SET @Count = @Count + 1
	END
	
	IF((8 - @Count) < @max)
	BEGIN
	SET @result = 1;
	END
	END
	END
	END
	SELECT @result;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatGetCharSata]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create procedure [dbo].[_AnticheatGetCharSata]
	@name	varchar(128)
as
    DECLARE @level int = 0;
	IF EXISTS (SELECT * FROM SRO_VT_SHARD.dbo._Char where CharName16 = @name)
	BEGIN
       SELECT @level = CurLevel FROM SRO_VT_SHARD.dbo._Char where CharName16 = @name
	END
	SELECT @level
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatChekItemOpt]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE procedure [dbo].[_AnticheatChekItemOpt]
	@name	varchar(128),
	@slot int,
	@max int
	
as
    DECLARE @result int = 0;
	IF EXISTS (SELECT * FROM SRO_VT_SHARD.dbo._Items WHERE OptLevel < @max and ID64 IN (SELECT ItemID FROM SRO_VT_SHARD.dbo._Inventory WHERE Slot = @slot and CharID IN (SELECT CharID FROM SRO_VT_SHARD.dbo._Char WHERE CharName16 = @name)))
	BEGIN
	SET @result = 1;
	END
	SELECT @result;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatCheckTeleportAccess]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[_AnticheatCheckTeleportAccess]
    @charname    VARCHAR(16),
    @teleport int
    
AS
DECLARE @RETURN int = 0

IF(@teleport = 166)
BEGIN
IF EXISTS (SELECT * FROM SR_R_Shard1.dbo._CharQuest where QuestID = 544 and Status = 4 and CharID in (select CharID from SR_R_Shard1.dbo._Char WHERE CharName16 = @charname))
BEGIN
SET @RETURN = 1;
END
END

IF(@teleport = 167)
BEGIN
IF EXISTS (SELECT * FROM SR_R_Shard1.dbo._CharQuest where QuestID = 650 and Status = 4 and CharID in (select CharID from SR_R_Shard1.dbo._Char WHERE CharName16 = @charname))
BEGIN
SET @RETURN = 1;
END
END
select @RETURN;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatCheckGmAccessObjID]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[_AnticheatCheckGmAccessObjID]
    @username     VARCHAR(128),
    @objID int,
    @Amount int
    
AS
DECLARE @RETURN int = 0

IF(EXISTS(SELECT * FROM _AnticheatGmAccessObjID WHERE username = @username and obj_id = @objID and amount >= @Amount and service = 1))
begin
set @RETURN = 1;
end

IF(EXISTS(SELECT * FROM _AnticheatGmAccessObjID WHERE username = @username and obj_id = 0 and amount = 0 and service = 1))
begin
set @RETURN = 1;
end

select @RETURN;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatCheckGmAccessControl]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[_AnticheatCheckGmAccessControl]
    @username     VARCHAR(128),
    @cmd int
    
AS
DECLARE @RETURN int = 0

IF(EXISTS(SELECT * FROM _AnticheatGmAccessControl WHERE username = @username and cmd_id = @cmd))
begin
set @RETURN = 1;
end
--SUPER GM
IF(EXISTS(SELECT * FROM _AnticheatGmAccessControl WHERE username = @username and cmd_id = 0))
BEGIN
set @RETURN = 1;
END



select @RETURN;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatAutoRegistration]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create procedure [dbo].[_AnticheatAutoRegistration]
	@username	varchar(128),
	@password	varchar(128),
    @IP	varchar(128)
as
	IF NOT EXISTS(SELECT * FROM TB_User where StrUserID = @username)
	BEGIN
	INSERT INTO TB_User(StrUserID,password,GMrank,regtime,reg_ip,sec_primary,sec_content) VALUES (@username,@password,0,GETDATE(),@IP,3,3)
	END
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatAuthLog]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE procedure [dbo].[_AnticheatAuthLog]
	@username	varchar(128),
    @IP	varchar(128)
as
	insert into sroprot_login_log values (@username, @IP, GETDATE())
	
	select 1;
	return;
GO
/****** Object:  StoredProcedure [dbo].[_AnticheatArenaStatusNotify]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[_AnticheatArenaStatusNotify]
    @charname     VARCHAR(16),
    @status int
    
AS
DECLARE @camp_id int = 0;
DEClARE @honor_point int;
DECLARE @graduate_count int;

IF(EXISTS(SELECT * FROM SR_R_Shard1.dbo._TrainingCampMember where MemberClass = 0 and CharID in (select CharID from SR_R_Shard1.dbo._Char where CharName16 = @charname)))
BEGIN
SELECT @camp_id = CampID FROM SR_R_Shard1.dbo._TrainingCampMember where MemberClass = 0 and CharID in (select CharID from SR_R_Shard1.dbo._Char where CharName16 = @charname)
END
IF(@camp_id > 0)
BEGIN
IF(EXISTS (SELECT * FROM SR_R_Shard1.dbo._TrainingCamp where ID = @camp_id))
BEGIN
SELECT @honor_point = EvaluationPoint,@graduate_count = GraduateCount FROM SR_R_Shard1.dbo._TrainingCamp where ID = @camp_id
IF(@status = 1)
BEGIN
UPDATE SR_R_Shard1.dbo._TrainingCamp set GraduateCount =  GraduateCount + 1,EvaluationPoint = EvaluationPoint + 1 WHERE ID = @camp_id
END
ELSE
BEGIN
if(@honor_point > 0 and @graduate_count > 0)
BEGIN
UPDATE SR_R_Shard1.dbo._TrainingCamp set GraduateCount =  GraduateCount - 1,EvaluationPoint = EvaluationPoint - 1 WHERE ID = @camp_id
END
END
END
END



INSERT INTO _AnticheatArenaStatus ([char_name],[status]) VALUES (@charname,@status)
GO
/****** Object:  StoredProcedure [dbo].[__IPLockDown]    Script Date: 07/06/2015 22:52:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[__IPLockDown]
@username VARCHAR (128),
@ip VARCHAR(128)
AS
DECLARE @JID INT
DECLARE @RETURN INT = 1
IF(@username like '"\?/()%#--')
begin
select 0;
return;
end
IF(EXISTS(SELECT * FROM _AnticheatIPLockDown WHERE username = @username))
BEGIN
IF(NOT EXISTS(SELECT * FROM _AnticheatIPLockDown WHERE username = @username and IP = @ip))
BEGIN
SET @RETURN = 0;
end
END

SELECT @RETURN;
GO
