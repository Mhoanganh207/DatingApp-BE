using System.Text;
using System.Text.Json;
using DatingApp.Data;
using DatingApp.Hubs;
using DatingApp.Models;
using DatingApp.Services;
using Meilisearch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:5173");
                });
        });

builder.Services.AddScoped<IDatabase>(provider =>
{
    var redis = ConnectionMultiplexer.Connect("localhost:6379");
    return redis.GetDatabase();
});
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFavouriteService, FavouriteService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration.GetSection("Appsettings:Token")?.Value ?? string.Empty;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token expired");
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Meiliesearch set up;



app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin"); // Placed before MapControllers and MapHub
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
MeilisearchClient client = new MeilisearchClient("http://localhost:7700", "aSampleMasterKey");
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
var services = app.Services.CreateScope().ServiceProvider;

var context = services.GetRequiredService<AppDbContext>();
var accountList = await context.Accounts.Include(acc => acc.Hobbies).ToListAsync();
var list = accountList.ConvertAll(acc => new UserDto(acc));
var index = client.Index("accounts");
await index.AddDocumentsAsync<UserDto>(list);

app.Run();
