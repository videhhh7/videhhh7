using Inventory.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<InventoryRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/api/products/low-stock", async (
    InventoryRepository repository, CancellationToken cancellationToken) =>
{
    var products = await repository.GetLowStockAsync(cancellationToken);
    return Results.Ok(products);
});

app.Run();
