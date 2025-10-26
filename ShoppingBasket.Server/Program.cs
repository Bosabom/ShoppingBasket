using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ShoppingBasket.Server.Data;

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

//TODO: Implement creating receipts with applying discounts and calculating total cost (CalculationHelper class?)
//Add mapping here (AutoMapperProfile)

//receipts.MapPost("/", async (ReceiptDto receiptDto, ShoppingBasketDbContext db) =>
//{
//    var receipt = new Receipt
//    {
//        TotalDiscount = receiptDto.TotalDiscount,
//        TotalCost = receiptDto.TotalCost
//    };
//    db.Receipts.Add(receipt);
//    await db.SaveChangesAsync();
//    return TypedResults.CreatedAtRoute("CreateReceipt",receipt);
//}).WithName("CreateReceipt");

//FIXME: Move this methods to separate files (like some service classes and also data retrieval classes)
static async Task<IResult> GetAllItems(ShoppingBasketDbContext db)
{
    return TypedResults.Ok(await db.Items.AsNoTracking().ToArrayAsync());
}

static async Task<IResult> GetItemById(long id, ShoppingBasketDbContext db)
{
    var item = await db.Items.FindAsync(id);
    return item is not null ? TypedResults.Ok(item) : TypedResults.NotFound();
}

static async Task<IResult> GetAllReceipts(ShoppingBasketDbContext db)
{
    return TypedResults.Ok(await db.Receipts.AsNoTracking().ToArrayAsync());
}

static async Task<IResult> GetReceiptById(long id, ShoppingBasketDbContext db)
{
    var item = await db.Receipts.FindAsync(id);
    return item is not null ? TypedResults.Ok(item) : TypedResults.NotFound();
}

app.MapFallbackToFile("/index.html");

app.Run();