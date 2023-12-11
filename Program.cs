using System.Text;
using DatingApp.Data;
using DatingApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);
var datingAppFe = "_DatingAppFe";
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: datingAppFe,
        policy  =>
        {
            policy.WithOrigins("http://localhost:5000");
        })
        ;
});
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration.GetSection("Appsettings:Token").Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(datingAppFe);
app.UseHttpsRedirection();
app.MapControllers();
app.Run();