USE [OdataToEntity]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

if exists (select * from sysobjects where id = object_id('dbo.GetOrders') and objectproperty(id, 'IsProcedure') = 1 )
	drop procedure dbo.GetOrders;
go

if exists (select * from sysobjects where id = object_id('dbo.ResetDb') and objectproperty(id, 'IsProcedure') = 1 )
	drop procedure dbo.ResetDb;
go

if exists (select * from sysobjects where id = object_id('dbo.ResetManyColumns') and objectproperty(id, 'IsProcedure') = 1 )
	drop procedure dbo.ResetManyColumns;
go

if exists (select * from sysobjects where id = object_id('dbo.OrderItems') and objectproperty(id, 'IsTable') = 1 )
	drop table dbo.OrderItems;
go

if exists (select * from sysobjects where id = object_id('dbo.Orders') and objectproperty(id, 'IsTable') = 1 )
	drop table dbo.Orders;
go

if exists (select * from sysobjects where id = object_id('dbo.Categories') and objectproperty(id, 'IsTable') = 1 )
	drop table dbo.Categories;
go

if exists (select * from sysobjects where id = object_id('dbo.Customers') and objectproperty(id, 'IsTable') = 1 )
	drop table dbo.Customers;
go

if exists (select * from sysobjects where id = object_id('dbo.ManyColumns') and objectproperty(id, 'IsTable') = 1 )
	drop table dbo.ManyColumns;
go

CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[ParentId] [int] NULL,
	[DateTime] [datetime2]
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Customers](
	[Address] [varchar](256) NULL,
	[Country] [char](2) NOT NULL,
	[Id] [int] NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[Sex] [int] NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Country],[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ManyColumns](
	[Column01] [int] NOT NULL,
	[Column02] [int] NOT NULL,
	[Column03] [int] NOT NULL,
	[Column04] [int] NOT NULL,
	[Column05] [int] NOT NULL,
	[Column06] [int] NOT NULL,
	[Column07] [int] NOT NULL,
	[Column08] [int] NOT NULL,
	[Column09] [int] NOT NULL,
	[Column10] [int] NOT NULL,
	[Column11] [int] NOT NULL,
	[Column12] [int] NOT NULL,
	[Column13] [int] NOT NULL,
	[Column14] [int] NOT NULL,
	[Column15] [int] NOT NULL,
	[Column16] [int] NOT NULL,
	[Column17] [int] NOT NULL,
	[Column18] [int] NOT NULL,
	[Column19] [int] NOT NULL,
	[Column20] [int] NOT NULL,
	[Column21] [int] NOT NULL,
	[Column22] [int] NOT NULL,
	[Column23] [int] NOT NULL,
	[Column24] [int] NOT NULL,
	[Column25] [int] NOT NULL,
	[Column26] [int] NOT NULL,
	[Column27] [int] NOT NULL,
	[Column28] [int] NOT NULL,
	[Column29] [int] NOT NULL,
	[Column30] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderItems](
	[Count] [int] NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[Product] [varchar](256) NOT NULL,
 CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Orders](
	[AltCustomerCountry] [char](2) NULL,
	[AltCustomerId] [int] NULL,
	[CustomerCountry] [char](2) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Date] [datetimeoffset](7) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Categories] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Categories]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItem_Order]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_AltCustomers] FOREIGN KEY([AltCustomerCountry],[AltCustomerId])
REFERENCES [dbo].[Customers] ([Country],[Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_AltCustomers]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Customers] FOREIGN KEY([CustomerCountry],[CustomerId])
REFERENCES [dbo].[Customers] ([Country],[Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Customers]
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetOrders]
  @id int,
  @name varchar(256),
  @status int
as
begin
	set nocount on;

	if @id is null and @name is null and @status is null
	begin
	  select * from dbo.Orders;
	  return;
	end;

	if not @id is null
	begin
	  select * from dbo.Orders where Id = @id;
	  return;
	end;

	if not @name is null
	begin
	  select * from dbo.Orders where Name like '%' + @name + '%';
	  return;
	end;

	if not @status is null
	begin
	  select * from dbo.Orders where Status = @status;
	  return;
	end;
end
go

CREATE procedure [dbo].[ResetDb]
as
begin
	set nocount on;

	delete from dbo.OrderItems;
	delete from dbo.Orders;
	delete from dbo.Customers;
	delete from dbo.Categories;

	dbcc checkident('dbo.OrderItems', reseed, 0);
	dbcc checkident('dbo.Orders', reseed, 0);
	dbcc checkident('dbo.Categories', reseed, 0);
end
go

CREATE procedure [dbo].[ResetManyColumns]
as
begin
	set nocount on;

	delete from dbo.ManyColumns;
end
go
