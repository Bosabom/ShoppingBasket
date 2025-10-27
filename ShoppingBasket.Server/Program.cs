using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Server.Data;
using ShoppingBasket.Server.Repositories;
using ShoppingBasket.Server.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DbConnection");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register EF Core + Npgsql
builder.Services.AddDbContext<ShoppingBasketDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("Health Check", () => HealthCheckResult.Healthy("The app is healthy!"))
    .AddNpgSql(connectionString);

// Register repositories and services
builder.Services.AddTransient<IReceiptRepository, ReceiptRepository>();
builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();
app.UseRouting();
app.MapHealthChecks("/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

// Apply EF Core migrations at startup (optional but useful in dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShoppingBasketDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Define API endpoints
RouteGroupBuilder items = app.MapGroup("/items");
items.MapGet("/", GetAllItems).WithName("GetItems");
items.MapGet("/{id}", GetItemById).WithName("GetItemById");

RouteGroupBuilder receipts = app.MapGroup("/receipts");
receipts.MapGet("/", GetAllReceipts).WithName("GetReceipts");
receipts.MapGet("/{id}", GetReceiptById).WithName("GetReceiptById");
receipts.MapGet("/detailed/{id}", GetDeteiledReceiptById).WithName("GetDeteiledReceiptById");
receipts.MapGet("/history", GetReceiptsHistory).WithName("GetReceiptsHistory");
//FIXME: implement CreateReceipt endpoint
//receipts.MapPost("/", () => Results.StatusCode(501)).WithName("CreateReceipt");

static async Task<IResult> GetAllItems(IItemService itemService)
{
    var items = await itemService.GetAllItemsAsync();
    return items is not null ? TypedResults.Ok(items) : TypedResults.NotFound();
}

static async Task<IResult> GetItemById(long id, IItemService itemService)
{
    var item = await itemService.GetItemByIdAsync(id);
    return item is not null ? TypedResults.Ok(item) : TypedResults.NotFound();
}

static async Task<IResult> GetAllReceipts(IReceiptService receiptService)
{
    var receipts = await receiptService.GetAllReceiptsAsync();
    return receipts is not null ? TypedResults.Ok(receipts) : TypedResults.NotFound();
}

static async Task<IResult> GetReceiptById(long id, IReceiptService receiptService)
{
    var receipt = await receiptService.GetReceiptByIdAsync(id);
    return receipt is not null ? TypedResults.Ok(receipt) : TypedResults.NotFound();
}

static async Task<IResult> GetDeteiledReceiptById(long id, IReceiptService receiptService)
{
    var receipt = await receiptService.GetDetailedReceiptByIdAsync(id);
    return receipt is not null ? TypedResults.Ok(receipt) : TypedResults.NotFound();
}

static async Task<IResult> GetReceiptsHistory(IReceiptService receiptService)
{
    var receipts = await receiptService.GetReceiptsHistoryAsync();
    return receipts is not null ? TypedResults.Ok(receipts) : TypedResults.NotFound();
}

app.MapFallbackToFile("/index.html");

app.Run();