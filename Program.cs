using Ecom.Components;
using Ecom.Infrastructure;
using Ecom.Infrastructure.Data;
using Ecom.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AppMigration>();

//register services
builder.Services.AddScoped<AuthService>();

//Add components
builder.Services.AddScoped<CartSummaryViewComponent>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Apply custom migrations on startup
using (var scope = app.Services.CreateScope())
{
    var appMigration = scope.ServiceProvider.GetRequiredService<AppMigration>();
    appMigration.ApplyCustomMigrations().GetAwaiter().GetResult();  // Call the custom migration logic to apply changes
}

// Seed the database
DatabaseSeeder.Seed(app);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();