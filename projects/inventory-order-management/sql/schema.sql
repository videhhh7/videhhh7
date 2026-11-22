/*
  Inventory & Order Management — SQL Server starter schema
  Run only on a local or dedicated development SQL Server instance.
*/
IF DB_ID(N'InventoryOrderDemo') IS NULL
    CREATE DATABASE InventoryOrderDemo;
GO

USE InventoryOrderDemo;
GO

CREATE TABLE dbo.Customers
(
    CustomerId   INT IDENTITY(1,1) CONSTRAINT PK_Customers PRIMARY KEY,
    FullName     NVARCHAR(150) NOT NULL,
    Email        NVARCHAR(254) NOT NULL,
    CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Customers_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Customers_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.Products
(
    ProductId       INT IDENTITY(1,1) CONSTRAINT PK_Products PRIMARY KEY,
    Sku             NVARCHAR(40) NOT NULL,
    ProductName     NVARCHAR(160) NOT NULL,
    UnitPrice       DECIMAL(12,2) NOT NULL,
    QuantityOnHand  INT NOT NULL CONSTRAINT DF_Products_Quantity DEFAULT 0,
    ReorderLevel    INT NOT NULL CONSTRAINT DF_Products_Reorder DEFAULT 5,
    IsActive        BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT 1,
    CONSTRAINT UQ_Products_Sku UNIQUE (Sku),
    CONSTRAINT CK_Products_UnitPrice CHECK (UnitPrice >= 0),
    CONSTRAINT CK_Products_Quantity CHECK (QuantityOnHand >= 0),
    CONSTRAINT CK_Products_Reorder CHECK (ReorderLevel >= 0)
);
GO

CREATE TABLE dbo.Orders
(
    OrderId       BIGINT IDENTITY(1,1) CONSTRAINT PK_Orders PRIMARY KEY,
    CustomerId    INT NOT NULL,
    OrderDateUtc  DATETIME2(0) NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT SYSUTCDATETIME(),
    Status        NVARCHAR(20) NOT NULL CONSTRAINT DF_Orders_Status DEFAULT N'Pending',
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT CK_Orders_Status CHECK (Status IN (N'Pending', N'Paid', N'Shipped', N'Cancelled'))
);
GO

CREATE TABLE dbo.OrderItems
(
    OrderItemId BIGINT IDENTITY(1,1) CONSTRAINT PK_OrderItems PRIMARY KEY,
    OrderId     BIGINT NOT NULL,
    ProductId   INT NOT NULL,
    Quantity    INT NOT NULL,
    UnitPrice   DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT UQ_OrderItems_Order_Product UNIQUE (OrderId, ProductId),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice >= 0)
);
GO

CREATE INDEX IX_Orders_Customer_OrderDate
    ON dbo.Orders(CustomerId, OrderDateUtc DESC);
GO

CREATE INDEX IX_OrderItems_Product
    ON dbo.OrderItems(ProductId);
GO

CREATE VIEW dbo.vw_OrderTotals
AS
    SELECT
        o.OrderId,
        o.CustomerId,
        c.FullName AS CustomerName,
        o.OrderDateUtc,
        o.Status,
        SUM(oi.Quantity * oi.UnitPrice) AS OrderTotal
    FROM dbo.Orders AS o
    INNER JOIN dbo.Customers AS c ON c.CustomerId = o.CustomerId
    INNER JOIN dbo.OrderItems AS oi ON oi.OrderId = o.OrderId
    GROUP BY o.OrderId, o.CustomerId, c.FullName, o.OrderDateUtc, o.Status;
GO
