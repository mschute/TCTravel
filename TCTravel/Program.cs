using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TCTravel.Controllers;
using TCTravel.Models;
using TCTravel.Service;
using TCTravel.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TCTravelContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));

// Configure Identity Framework 
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<TCTravelContext>().AddDefaultTokenProviders();

// Included configured service from EmailSettings.cs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Registers the email service with a scoped lifetime of a single request
builder.Services.AddScoped<EmailService>();

// Registers the email service with a scoped lifetime of a single request
builder.Services.AddScoped<RolesController>();

// Configure Jwt authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            // Event is triggered ifJWT authentication fails which could be due to token expiration, token signature verification failure etc. Will log event
            OnAuthenticationFailed = context =>
            {
                // TODO Log Authentication error
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            // Event if triggered when token is valid, will log event Trigger a log event
            OnTokenValidated = context =>
            {
                //TODO Log Token validation success
                Console.WriteLine("Token validated");
                return Task.CompletedTask;
            },
            // Event is triggered when user provides invalid credentials, can be used to redirect, will log event 
            OnChallenge = context =>
            {
                //TODO Log Token validation success
                Console.WriteLine("Authentication challenge " + context.Error);
                return Task.CompletedTask;
            }
        };
        
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