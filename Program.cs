using Microsoft.EntityFrameworkCore;
using BlazorDynamicApp.Data.Context;
using BlazorDynamicApp.Data.Services;
using BlazorDynamicApp.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<IDynamicDataService, DynamicDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<BlazorDynamicApp.Components.App>()
    .AddInteractiveServerRenderMode();

// Initialize database
await InitializeDatabaseAsync(app);

app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Create database if it doesn't exist
        await context.Database.EnsureCreatedAsync();

        // Seed initial data if empty
        if (!await context.DynamicEntities.AnyAsync())
        {
            DynamicEntity[] entities =
            [
                new() { Name = "Laptop", Description = "High-performance gaming laptop", Category = "Electronics", Price = 1200.00m, Quantity = 10, IsActive = true },
                new() { Name = "Office Chair", Description = "Ergonomic office chair", Category = "Furniture", Price = 250.00m, Quantity = 15, IsActive = true },
                new() { Name = "Notebook", Description = "Professional notebook set", Category = "Stationery", Price = 15.99m, Quantity = 100, IsActive = true },
                new() { Name = "Coffee Maker", Description = "Automatic coffee machine", Category = "Appliances", Price = 89.99m, Quantity = 20, IsActive = true },
                new() { Name = "Desk Lamp", Description = "LED desk lamp with adjustable brightness", Category = "Lighting", Price = 35.50m, Quantity = 30, IsActive = true }
            ];

            await context.DynamicEntities.AddRangeAsync(entities);
            await context.SaveChangesAsync();

            Console.WriteLine("Database seeded with initial data.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    }
}