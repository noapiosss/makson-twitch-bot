using Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppConfiguration"));
builder.Services.Configure<TwitchBotConfiguration>(builder.Configuration.GetSection("TwitchBotConfiguration"));
builder.Services.Configure<TwitchApiConfiguration>(builder.Configuration.GetSection("TwitchApiConfiguration"));

builder.Services.AddDomainServices((sp, options) =>
{
    IOptionsMonitor<AppConfiguration> configuration = sp.GetRequiredService<IOptionsMonitor<AppConfiguration>>();
    ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    _ = options.UseSqlite(configuration.CurrentValue.ConnectionString)
        .UseLoggerFactory(loggerFactory);
});

// builder.Services.AddSingleton<ITwitchAPI, TwitchApiClient>();
// builder.Services.AddSingleton<TwitchBotService>();
// builder.Services.AddHostedService(provider => provider.GetService<TwitchBotService>());

WebApplication app = builder.Build();


//builder.Services.AddHostedService<InitTwitchBot>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
