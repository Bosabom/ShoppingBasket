using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Server.Data;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register EF Core + Npgsql
builder.Services.AddDbContext<ShoppingBasketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();
app.UseRouting();

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

RouteGroupBuilder items = app.MapGroup("/items");

items.MapGet("/", GetAllItems);
items.MapGet("/{id}", GetItemById);
//app.MapGet("/items", GetAllItems).WithName("GetAllItems");


static async Task<IResult> GetAllItems(ShoppingBasketDbContext db)
{
    return TypedResults.Ok(await db.Items.AsNoTracking().ToArrayAsync());
}

static async Task<IResult> GetItemById(long id, ShoppingBasketDbContext db)
{
    var item = await db.Items.FindAsync(id);
    return item is not null ? TypedResults.Ok(item) : TypedResults.NotFound();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapFallbackToFile("/index.html");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
