using MediaWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var postgresUrl = builder.Configuration["DATABASE_URL"];
if (!string.IsNullOrWhiteSpace(postgresUrl))
{
    var uri = new Uri(postgresUrl);
    var credentials = uri.UserInfo.Split(':', 2);
    if (credentials.Length != 2) throw new InvalidOperationException("DATABASE_URL needs a username and password.");
    var postgresConnection = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
        Username = Uri.UnescapeDataString(credentials[0]),
        Password = Uri.UnescapeDataString(credentials[1])
    }.ConnectionString;
    builder.Services.AddDbContext<PostgresApplicationDbContext>(options => options.UseNpgsql(postgresConnection));
    builder.Services.AddScoped<ApplicationDbContext>(services => services.GetRequiredService<PostgresApplicationDbContext>());
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment() || !string.IsNullOrWhiteSpace(postgresUrl))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();
app.MapGet("/health", () => Results.Ok("ok"));

if (args.Contains("--seed-demo") || builder.Configuration.GetValue<bool>("SeedDemoData"))
{
    await DemoDataSeeder.SeedAsync(app.Services);
    if (args.Contains("--seed-demo")) return;
}

app.Run();
