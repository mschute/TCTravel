using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using TCTravel;
using TCTravel.Controllers;
using TCTravel.Models;
using TCTravel.Service;
using TCTravel.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuring the logging for this application with minimum level of log messages set to Warning or higher
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
    // Ensuring no previous log providers are set
    .ClearProviders()
    // Output logs to the console
    .AddConsole()
    // Output the logs to a file with the following path
    .AddFile($"Logs/TCTravel-{typeof(JSType.Date)}.txt");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TCTravelContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));

// Configure Identity Framework 
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TCTravelContext>()
    .AddDefaultTokenProviders();

// Included configured service from EmailSettings.cs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Registers the email service with a scoped lifetime of a single request
builder.Services.AddScoped<EmailService>();

// Registers the email service with a scoped lifetime of a single request
builder.Services.AddScoped<RolesController>();

// Registers the AppJwtBearer Events with a scoped lifetime of a single request
builder.Services.AddScoped<AppJwtBearerEvents>();

// Configure Jwt authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Override default events type for JwtBearerEvents in order to use logging
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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