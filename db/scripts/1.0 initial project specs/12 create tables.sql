
create table idpy.[RequestState](
[RequestStateCode] tinyint not null, [RequestStateName] varchar(50) not null,
  constraint [RequestState_pkey] primary key([RequestStateCode])
)
GO




create table idpy.[Request](
  [RequestId] uniqueidentifier not null,
  [RequestStateCode] tinyint not null,
  [RequestHash] nvarchar(256) not null,
  [ResponseJson] nvarchar(max) not null,
  [StatusCode] int not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  [RowVersion] rowversion not null,
  constraint [Request_pkey] primary key([RequestId])
)
GO




create table idpy.[EventType](
[EventTypeCode] tinyint not null, [EventTypeName] varchar(50) not null,
  constraint [EventType_pkey] primary key([EventTypeCode])
)
GO




create table idpy.[OutboxMessages](
  [OutboxMessageId] uniqueidentifier not null,
  [EventTypeCode] tinyint not null,
  [Payload] nvarchar(max) not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  [ProcessedDate] datetime2,
  constraint [OutboxMessages_pkey] primary key([OutboxMessageId])
)
GO




create table idpy.[InboxMessages](
  [InboxMessageId] uniqueidentifier not null,
  [EventTypeCode] tinyint not null,
  [Payload] nvarchar(max) not null,
  [ProcessedDate] datetime2,
  constraint [InboxMessages_pkey] primary key([InboxMessageId])
)
GO




create table dbo.[CattleStatus](
[CattleStatusCode] tinyint not null, [CattleStatusName] varchar(50) not null,
  constraint [CattleStatus_pkey] primary key([CattleStatusCode])
)
GO




create table dbo.[Cattle](
  [CattleId] int identity(1,1) not null,
  [LocationId] int not null,
  [CattleTypeCode] tinyint not null,
  [Birthdate] datetime not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [Cattle_pkey] primary key([CattleId])
)
GO




create table dbo.[CattleStatusHistory](
  [CattleStatusHistoryId] int identity(1,1) not null,
  [CattleId] int not null,
  [CattleStatusCode] tinyint not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [CattleStatusHistory_pkey] primary key([CattleStatusHistoryId])
)
GO




create table dbo.[ProductStatus](
[ProductStatusCode] tinyint not null, [ProductStatusName] varchar(50) not null,
  constraint [ProductStatus_pkey] primary key([ProductStatusCode])
)
GO




create table dbo.[ProductType](
[ProductTypeCode] tinyint not null, [ProductTypeName] varchar(50) not null,
  constraint [ProductType_pkey] primary key([ProductTypeCode])
)
GO




create table dbo.[Product](
  [ProductId] int identity(1,1) not null,
  [CattleId] int not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [Product_pkey] primary key([ProductId])
)
GO




create table dbo.[ProductStatusHistory](
  [ProductStatusHistoryId] int identity(1,1) not null,
  [ProductId] int not null,
  [ProductStatusCode] tinyint not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [ProductStatusHistory_pkey] primary key([ProductStatusHistoryId])
)
GO




create table dbo.[CattleType](
[CattleTypeCode] tinyint not null, [CattleTypeName] varchar(50) not null,
  constraint [CattleType_pkey] primary key([CattleTypeCode])
)
GO




create table dbo.[Brand](
[BrandCode] tinyint not null, [BrandName] varchar(50) not null,
  constraint [Brand_pkey] primary key([BrandCode])
)
GO




create table dbo.[Stock](
  [StockId] int identity(1,1) not null,
  [ProductId] int not null,
  [BrandCode] tinyint not null,
  [ProductTypeCode] tinyint not null,
  [Quantity] int not null,
  [QualityScore] decimal(5,2) not null,
  [PricePerUnit] decimal(10,2) not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [Stock_pkey] primary key([StockId])
)
GO


create unique nonclustered index
  [Stock_idx_ProductId_BrandCode_ProductTypeCode_QualityScore] on dbo.[Stock](
  [ProductId],
  [BrandCode],
  [ProductTypeCode],
  [QualityScore]
)
GO




create table dbo.[Customer](
  [CustomerId] int identity(1,1) not null,
  [CustomerName] varchar(50) not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [Customer_pkey] primary key([CustomerId])
)
GO




create table dbo.[Order](
  [OrderId] int identity(1,1) not null,
  [CustomerId] int not null,
  [OrderDate] datetime2 not null,
  constraint [Order_pkey] primary key([OrderId])
)
GO




create table dbo.[OrderDetails](
  [OrderId] int not null,
  [StockId] int not null,
  [Quantity] int not null,
  [Price] decimal(10,2) not null,
  constraint [OrderDetails_pkey] primary key([OrderId], [StockId])
)
GO




create table dbo.[Shipping](
  [ShippingId] int identity(1,1) not null,
  [OrderId] int not null,
  [EstimatedDeliveryDate] datetime2 not null,
  [ActualDeliveryDate] datetime2,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [Shipping_pkey] primary key([ShippingId])
)
GO




create table dbo.[OrderStatus](
[OrderStatusCode] tinyint not null, [OrderStatusName] varchar(50) not null,
  constraint [OrderStatus_pkey] primary key([OrderStatusCode])
)
GO




create table dbo.[OrderStatusHistory](
  [OrderStatusHistoryId] int identity(1,1) not null,
  [OrderId] int not null,
  [OrderStatusCode] tinyint not null,
  [SavedDate] datetime2 default sysutcdatetime() not null,
  constraint [OrderStatusHistory_pkey] primary key([OrderStatusHistoryId])
)
GO




create table dbo.[Location](
[LocationId] int identity(1,1) not null, [LocationName] varchar(50) not null,
  constraint [Location_pkey] primary key([LocationId])
)
GO



alter table idpy.[Request]
  add constraint [Request_fkey]
    foreign key ([RequestStateCode])
      references idpy.[RequestState] ([RequestStateCode])
GO


alter table idpy.[OutboxMessages]
  add constraint [OutboxMessages_fkey]
    foreign key ([EventTypeCode]) references idpy.[EventType] ([EventTypeCode])
GO


alter table dbo.[CattleStatusHistory]
  add constraint [CattleStatusHistory_fkey]
    foreign key ([CattleStatusCode])
      references dbo.[CattleStatus] ([CattleStatusCode])
GO


alter table dbo.[CattleStatusHistory]
  add constraint [CattleStatusHistory_fkey1]
    foreign key ([CattleId]) references dbo.[Cattle] ([CattleId])
GO


alter table dbo.[ProductStatusHistory]
  add constraint [ProductStatusHistory_fkey]
    foreign key ([ProductId]) references dbo.[Product] ([ProductId])
GO


alter table dbo.[ProductStatusHistory]
  add constraint [ProductStatusHistory_fkey1]
    foreign key ([ProductStatusCode])
      references dbo.[ProductStatus] ([ProductStatusCode])
GO


alter table dbo.[Product]
  add constraint [Product_fkey1]
    foreign key ([CattleId]) references dbo.[Cattle] ([CattleId])
GO


alter table dbo.[Cattle]
  add constraint [Cattle_fkey]
    foreign key ([CattleTypeCode]) references dbo.[CattleType] ([CattleTypeCode])
GO


alter table dbo.[Stock]
  add constraint [Stock_fkey]
    foreign key ([ProductTypeCode]) references dbo.[ProductType] ([ProductTypeCode])
GO


alter table dbo.[Stock]
  add constraint [Stock_fkey1]
    foreign key ([ProductId]) references dbo.[Product] ([ProductId])
GO


alter table dbo.[Stock]
  add constraint [Stock_fkey2]
    foreign key ([BrandCode]) references dbo.[Brand] ([BrandCode])
GO


alter table dbo.[Order]
  add constraint [Order_fkey]
    foreign key ([CustomerId]) references dbo.[Customer] ([CustomerId])
GO


alter table dbo.[OrderDetails]
  add constraint [OrderDetails_fkey]
    foreign key ([OrderId]) references dbo.[Order] ([OrderId])
GO


alter table dbo.[OrderDetails]
  add constraint [OrderDetails_fkey1]
    foreign key ([StockId]) references dbo.[Stock] ([StockId])
GO


alter table dbo.[Shipping]
  add constraint [Shipping_fkey]
    foreign key ([OrderId]) references dbo.[Order] ([OrderId])
GO


alter table dbo.[OrderStatusHistory]
  add constraint [OrderStatusHistory_fkey]
    foreign key ([OrderId]) references dbo.[Order] ([OrderId])
GO


alter table dbo.[OrderStatusHistory]
  add constraint [OrderStatusHistory_fkey1]
    foreign key ([OrderStatusCode]) references dbo.[OrderStatus] ([OrderStatusCode])
GO


alter table idpy.[InboxMessages]
  add constraint [InboxMessages_fkey]
    foreign key ([EventTypeCode]) references idpy.[EventType] ([EventTypeCode])
GO


alter table dbo.[Cattle]
  add constraint [Cattle_fkey1]
    foreign key ([LocationId]) references dbo.[Location] ([LocationId])
GO
