using BlazorApp1.Actions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorApp1.Data;
using BlazorApp1.Middleware;
using BlazorApp1.Models;
using BlazorApp1.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var redisConnection = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton(redisConnection.GetDatabase());

builder.Services
    .AddScoped<RequestContext>()
    .AddMemoryCache()
    .AddApiService()
    .AddEndpointsApiExplorer();

builder.Services.AddDbContext<ScoreContext>();

builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(80));

builder.Services.AddControllers();

var app = builder.Build();

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

app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseMiddleware<RequestParser>();

app.Run();
