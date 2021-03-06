USE [EmployeeAttendance]
GO
/****** Object:  Table [dbo].[Employee_Checkin]    Script Date: 11/08/2016 14:11:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee_Checkin](
	[name] [nvarchar](100) NULL,
	[id] [int] NULL,
	[checkindttime] [datetime] NULL,
	[createddate] [datetime] NULL,
	[createdate] [date] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EMPLOYEE]    Script Date: 11/08/2016 14:11:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EMPLOYEE](
	[Name] [varchar](100) NULL,
	[Age] [int] NULL,
	[ID] [int] NULL,
	[IMAGE] [image] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
