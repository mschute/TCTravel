using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TCTravel;
using TCTravel.Controllers;
using TCTravel.Models;
using TCTravel.Service;
using TCTravel.Services;

// Page includes various dependency injections to support this web application
var builder = WebApplication.CreateBuilder(args);

// Configuring the logging with log level and logging output type
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
    .ClearProviders()
    .AddConsole()
    .AddFile($"Logs/TCTravel-{typeof(JSType.Date)}.txt");

builder.Services.AddControllersWithViews();

// Configure the database 
builder.Services.AddDbContext<TCTravelContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));

// Configure identity framework for user registration, login etc.
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TCTravelContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<RolesController>();

builder.Services.AddScoped<AppJwtBearerEvents>();

// Configure Jwt authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Override default events type for JwtBearerEvents in order to use logging for Jwt events
        options.EventsType = typeof(AppJwtBearerEvents);
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();