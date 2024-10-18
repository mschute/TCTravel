# TCTravel

Version 1 – Tay Country Travel ASP.NET Framework Core Web Application

Developed a RESTful backend API using ASP.NET Core with the Model-View-Controller (MVC) architectural pattern. The Model layer manages business logic and database mapping, while the Controllers handle CRUD operations. No views were implemented in this version.

Key Features:

- Dependency Injection: Used for logging, database configuration, identity framework, email services, user roles, JWT token management, and events.
- PostgreSQL database configuration and JWT Bearer Events logging were implemented.
- MailKit was used for sending registration emails via SMTP.
- Syntactic Sugar: A wrapper class was used to simplify the JWT token handling and logging with methods such as AddJwtBearer() and custom Logging Middleware to provide easy, consistent logging across the application.
- Custom helper classes: one for validating passwords and another for logging uniform messages.
- Identity Framework: Manages user registration and roles, adjusting authorization based on roles.

  
NuGet Packages:

- MailKit
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.Extensions.Logging.Console
- Microsoft.Extensions.Logging.EventLog
- Microsoft.VisualStudio.Web.CodeGeneration.Design
- Serilog.Extensions.Logging.File
- Microsoft.EntityFrameworkCore and Tools
- Microsoft.Extensions.DependencyInjection
- Npgsql.EntityFrameworkCore.PostgreSQL

  
Project Overview:

This application supports Tay Country Travel, a luxury transportation service for golfing and tourism. The booking system allows Admin users to create bookings and manage related entities (drivers, vehicles, locations, customers, and third-party clients). SuperAdmin has full control, while Admins can manage entities they are assigned to. Role-based authorization ensures proper data access.

Deployment:

This project was deployed on Microsoft Azure.
