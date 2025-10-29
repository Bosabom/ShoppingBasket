using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ShoppingBasket.Server;
using ShoppingBasket.Server.Data;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Repositories;
using ShoppingBasket.Server.Services;
using ShoppingBasket.Server.Utils;

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

// Register repositories
builder.Services.AddTransient<IReceiptRepository, ReceiptRepository>();
builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddTransient<IDiscountRepository, DiscountRepository>();

// Register services
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

// Register Mapster configuration
builder.Services.RegisterMapsterConfiguration();
var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();
app.UseRouting();

// Map health checks
app.MapHealthChecks("/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

// Exception handling middleware (map domain exceptions -> HTTP)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (BadRequestException bre)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";
        var problem = new
        {
            type = "https://httpstatuses.io/400",
            title = "Bad Request",
            status = 400,
            detail = bre.Message
        };
        await context.Response.WriteAsJsonAsync(problem);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        var problem = new
        {
            type = "https://httpstatuses.io/500",
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred."
        };
        await context.Response.WriteAsJsonAsync(problem);
    }
});

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
// Items endpoints
RouteGroupBuilder items = app.MapGroup("/items");
items.MapGet("/", GetAllItemsAsync).WithName("GetItems");
items.MapGet("/{id}", GetItemByIdAsync).WithName("GetItemById");

// Discounts endpoints
RouteGroupBuilder discounts = app.MapGroup("/discounts");
discounts.MapGet("/", GetAllDiscountsAsync).WithName("GetDiscounts");
discounts.MapGet("/{id}", GetDiscountByIdAsync).WithName("GetDiscountById");

// Receipts endpoints
RouteGroupBuilder receipts = app.MapGroup("/receipts");
receipts.MapGet("/", GetAllReceiptsAsync).WithName("GetReceipts");
receipts.MapGet("/{id}", GetReceiptByIdAsync).WithName("GetReceiptById");
receipts.MapGet("/detailed/{id}", GetDeteiledReceiptByIdAsync).WithName("GetDeteiledReceiptById");
receipts.MapGet("/history", GetReceiptsHistoryAsync).WithName("GetReceiptsHistory");
receipts.MapPost("/", CreateReceiptAsync).WithName("CreateReceipt");

// Endpoint handlers
// Items handlers
static async Task<IResult> GetAllItemsAsync(IItemService itemService)
{
    var items = await itemService.GetAllItemsAsync();
    return items is not null ? TypedResults.Ok(items) : TypedResults.NotFound();
}

static async Task<IResult> GetItemByIdAsync(long id, IItemService itemService)
{
    var item = await itemService.GetItemByIdAsync(id);
    return item is not null ? TypedResults.Ok(item) : TypedResults.NotFound();
}

// Discounts handlers
static async Task<IResult> GetAllDiscountsAsync(IDiscountService discountService)
{
    var discounts = await discountService.GetAllDiscountsAsync();
    return discounts is not null ? TypedResults.Ok(discounts) : TypedResults.NotFound();
}

static async Task<IResult> GetDiscountByIdAsync(long id, IDiscountService discountService)
{
    var discount = await discountService.GetDiscountByIdAsync(id);
    return discount is not null ? TypedResults.Ok(discount) : TypedResults.NotFound();
}

// Receipts handlers
static async Task<IResult> GetAllReceiptsAsync(IReceiptService receiptService)
{
    var receipts = await receiptService.GetAllReceiptsAsync();
    return receipts is not null ? TypedResults.Ok(receipts) : TypedResults.NotFound();
}

static async Task<IResult> GetReceiptByIdAsync(long id, IReceiptService receiptService)
{
    var receipt = await receiptService.GetReceiptByIdAsync(id);
    return receipt is not null ? TypedResults.Ok(receipt) : TypedResults.NotFound();
}

static async Task<IResult> GetDeteiledReceiptByIdAsync(long id, IReceiptService receiptService)
{
    var receipt = await receiptService.GetDetailedReceiptByIdAsync(id);
    return receipt is not null ? TypedResults.Ok(receipt) : TypedResults.NotFound();
}

static async Task<IResult> GetReceiptsHistoryAsync(IReceiptService receiptService)
{
    var receipts = await receiptService.GetReceiptsHistoryAsync();
    return receipts is not null ? TypedResults.Ok(receipts) : TypedResults.NotFound();
}
static async Task<IResult> CreateReceiptAsync(ReceiptCreateDto receiptCreatedto, IReceiptService receiptService)
{
    var createdReceiptDto = await receiptService.CreateReceiptAsync(receiptCreatedto);
    return createdReceiptDto is not null ? TypedResults.Created($"/receipts/{createdReceiptDto.ReceiptId}", createdReceiptDto) : TypedResults.BadRequest();
}

app.MapFallbackToFile("/index.html");

app.Run();