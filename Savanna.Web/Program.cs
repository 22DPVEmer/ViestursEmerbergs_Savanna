using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Savanna.Infrastructure.Data;
using Savanna.Infrastructure.Models;
using Savanna.Infrastructure.Constants;
using Savanna.Services.Interfaces;
using Savanna.Services.Services;
using Savanna.Common.Plugin;
using Savanna.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(ErrorMessages.ConnectionStringNotFound);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection("Identity"));

builder.Services.AddRazorPages();
builder.Services.AddControllers();


// Add game services in correct order
builder.Services.AddSingleton<AnimalConfigurationService>();
builder.Services.AddSingleton<PluginLoader>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<PluginLoader>>();
    var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginConstants.Paths.PluginDirectory);
    return new PluginLoader(pluginPath, logger);
});
builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

// Initialize services in the correct order
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // First, ensure animal configurations are loaded
        var animalConfig = services.GetRequiredService<AnimalConfigurationService>();
        var configs = animalConfig.GetAnimalConfigurations();
        logger.LogInformation("Successfully initialized animal configurations with {Count} types", configs.Count);

        // Then load plugins
        var pluginLoader = services.GetRequiredService<PluginLoader>();
        pluginLoader.LoadPlugins();
        logger.LogInformation("Successfully loaded {Count} plugins", pluginLoader.GetAllPlugins().Count());

        // Finally, ensure database is ready
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        logger.LogInformation("Successfully initialized database");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during application initialization");
        throw; // Rethrow to prevent application from starting with invalid state
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Enable session middleware
app.UseSession();

app.MapRazorPages();
app.MapControllers();

app.Run();
