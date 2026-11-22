using Microsoft.Data.SqlClient;

namespace Inventory.Api.Data;

public sealed record LowStockProduct(
    int ProductId, string Sku, string ProductName, int QuantityOnHand, int ReorderLevel);

public sealed class InventoryRepository(IConfiguration configuration)
{
    public async Task<IReadOnlyList<LowStockProduct>> GetLowStockAsync(
        CancellationToken cancellationToken)
    {
        var connectionString = configuration.GetConnectionString("Inventory")
            ?? throw new InvalidOperationException("Connection string 'Inventory' is not configured.");

        const string sql = """
            SELECT ProductId, Sku, ProductName, QuantityOnHand, ReorderLevel
            FROM dbo.Products
            WHERE IsActive = 1 AND QuantityOnHand <= ReorderLevel
            ORDER BY QuantityOnHand, ProductName;
            """;

        var products = new List<LowStockProduct>();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(new LowStockProduct(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.GetInt32(4)));
        }

        return products;
    }
}
