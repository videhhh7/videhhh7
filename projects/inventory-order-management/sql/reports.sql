USE InventoryOrderDemo;
GO

-- 1. Products that need replenishment.
SELECT ProductId, Sku, ProductName, QuantityOnHand, ReorderLevel
FROM dbo.Products
WHERE IsActive = 1
  AND QuantityOnHand <= ReorderLevel
ORDER BY QuantityOnHand, ProductName;
GO

-- 2. Sales by product.
SELECT
    p.ProductId,
    p.Sku,
    p.ProductName,
    SUM(oi.Quantity) AS UnitsSold,
    SUM(oi.Quantity * oi.UnitPrice) AS Revenue
FROM dbo.OrderItems AS oi
INNER JOIN dbo.Orders AS o ON o.OrderId = oi.OrderId
INNER JOIN dbo.Products AS p ON p.ProductId = oi.ProductId
WHERE o.Status IN (N'Paid', N'Shipped')
GROUP BY p.ProductId, p.Sku, p.ProductName
ORDER BY Revenue DESC;
GO

-- 3. Customer order activity, including customers with no orders.
SELECT
    c.CustomerId,
    c.FullName,
    COUNT(o.OrderId) AS OrderCount
FROM dbo.Customers AS c
LEFT JOIN dbo.Orders AS o ON o.CustomerId = c.CustomerId
GROUP BY c.CustomerId, c.FullName
ORDER BY OrderCount DESC, c.FullName;
GO

-- 4. Reusable stored procedure for a customer's order history.
CREATE OR ALTER PROCEDURE dbo.GetCustomerOrderHistory
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        o.OrderDateUtc,
        o.Status,
        COALESCE(SUM(oi.Quantity * oi.UnitPrice), 0) AS OrderTotal
    FROM dbo.Orders AS o
    LEFT JOIN dbo.OrderItems AS oi ON oi.OrderId = o.OrderId
    WHERE o.CustomerId = @CustomerId
    GROUP BY o.OrderId, o.OrderDateUtc, o.Status
    ORDER BY o.OrderDateUtc DESC;
END;
GO
