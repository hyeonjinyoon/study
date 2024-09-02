using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using WebAPI.Actions;
using WebAPI.Middleware;
using WebAPI.Models;
using WebAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services
    .AddScoped<RequestContext>()
    .AddMemoryCache()
    .AddApiService()
    .AddEndpointsApiExplorer();

builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

builder.Services.AddDbContext<ScoreContext>();

var redisConnection = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton(redisConnection.GetDatabase());

builder.Services.AddControllers();

builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(80));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Map("/map1", HandleMapTest1);
app.Map("/map2", HandleMapTest2);
app.Map("/yes", Yes);

app.Map("/a", level1App =>
{
    level1App.Map("/b", level2AApp =>
    {
        level2AApp.Run(async context =>
        {
            await context.Response.WriteAsync("ab find");
        });
        // "/level1/level2a" processing
    });
});

app.MapWhen(context => context.Request.Query.ContainsKey("branch"), HandleBranch);

// app.Use(async (context, next) =>
// {
//     // Do work that can write to the Response.
//     await next.Invoke();
//     // Do logging or other work that doesn't write to the Response.
// });

app.UseMiddleware<RequestParser>();

app.Run();
// async context =>
// {
//     await context.Response.WriteAsync("Hello from 2nd delegate.");
// }

static void HandleMapTest1(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 1");
    });
}

static void HandleMapTest2(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 2");
    });
}

static void HandleBranch(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        var branchVer = context.Request.Query["branch"];
        await context.Response.WriteAsync($"Branch used = {branchVer}");
    });
}

static void Yes(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        // await context.Response.WriteAsync($"Response used = {context.Response}\n");
        // await context.Response.WriteAsync($"Body CanRead = {context.Request.Body.CanRead}\n");
        // await context.Response.WriteAsync($"Body = {context.Request.Body.ReadByte().ToString()}\n");
        // await context.Response.WriteAsync($"Body = {context.Request.Body.}\n");

        try
        {
            var bodyStr = "";
            var req = context.Request;

            using(var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = reader.ReadToEnd();
            }

            var jdom = JsonDocument.Parse(bodyStr);
            var jRoot = jdom.RootElement;

            var actions = jRoot.GetProperty("actions");

            // var responseObject = new JObject();
            var responseArray = new JArray();

            foreach (var item in actions.EnumerateArray())
            {
                var actionName = item.GetProperty("action").ToString();

                var responseObject2 = new JObject();
                responseObject2.Add("action", actionName);

                switch (actionName)
                {
                    case "echo":
                        responseObject2.Add("text", item.GetProperty("text").ToString());
                        responseObject2.Add("status", "success");
                        break;
                    case "add":
                        responseObject2.Add("sum", $"{(item.GetProperty("a").GetDouble() + item.GetProperty("b").GetDouble()):N0}");
                        responseObject2.Add("status", "success");
                        break;
                    case "multiply":
                        responseObject2.Add("result", $"{(item.GetProperty("a").GetDouble() * item.GetProperty("b").GetDouble()):N0}");
                        responseObject2.Add("status", "success");
                        break;
                    // case "get_player_score":
                    //     responseObject2.Add("result", $"{(item.GetProperty("a").GetDouble() * item.GetProperty("b").GetDouble()):N0}");
                    //     responseObject2.Add("status", "success");
                    //     break;
                    // case "set_player_score":
                    //     responseObject2.Add("result", $"{(item.GetProperty("a").GetDouble() * item.GetProperty("b").GetDouble()):N0}");
                    //     responseObject2.Add("status", "success");
                    //     break;
                    default:
                        responseObject2.Add("status", "INVALID_REQUEST");
                        break;
                }

                responseArray.Add(responseObject2);
                // await context.Response.WriteAsync($"{responseObject2}\n");
                // await context.Response.WriteAsync($"item = {item}\n");
            }

            await context.Response.WriteAsync($"{responseArray}");
        }
        catch (Exception e)
        {
            await context.Response.WriteAsync($"Please request the body in Json format.");
        }
    });
}
