# TCTravel

Version 1 – Tay Country Travel ASP.NET Framework Core Web Application

Created a RESTful backend API service for ASP.NET Framework Core using a Model View Controller architectural pattern to aid in organisation. The Model folder contains classes that hold the business logic and aid in mapping the database for data to be stored in. Controllers contain Create Retrieve Update and Delete actions that will operate on the database based on the business logic of the models. Views are not implemented in this version. 

Dependency injection is used for logging, configuring the database, identity framework, email service, defining user roles, and JWT token and events. PostgreSQL database configuration is implemented. JWT Bearer Events methods are overridden in order to add logging functionality for these events. MailKit, a .NET mail-client library created by Jeffrey Stedfast was used for sending registration emails through Simple Mail Transfer Protocol (SMTP). Logs are output to the Console as well as saved to a created new file each day the program is run. Two helper classes were created to aid in the functionality of the program, one to check if a user’s passwords meets the business requirements and a logger extension to aid in printing uniform logs. Identity Framework was used to aid user registration and role functionality. The role assigned to a particular user was used to adjust their authorization for various CRUD actions on the website.

NuGet packages installed for this project are: 

•    MailKit
•    Microsoft.AspNetCore.Authentication.JwtBearer
•    Microsoft.AspNetCore.Identity.EntityFrameworkCore
•    Microsoft.Extensions.Logging.Console
•    Microsoft.Extensions.Logging.EventLog
•    Microsoft.VisualStudio.Web.CodeGeneration.Design
•    Serilog.Extensions.Logging.File, Microsoft.EntityFrameworkCore
•    Microsoft.EntityFrameworkCore.Tools
•    Microsoft.Extensions.DependencyInjection
•    Npgsql.EntityFrameworkCore.PostgreSQL

This project is for Tay Country Travel, a luxury travel company that offers transportation for golfing, heritage landscapes and other tourist destinations. This service aims to create a booking system that will allow Admin to create bookings that will record the associated Driver, Vehicle, Locations, Customers (Passengers), and Third Party Clients and other related information. 

SuperAdmin will have authorisation to do any action in any controller. Admin will have the ability to Create, Edit, Update or Delete information for a Booking, BookingLocation, ClientCompany, Customer, Driver, Location and Vehicle. ClientCompany, Customer and Driver instances will be assigned to specific registered users. Then those users will be able to retrieve and update their specific information.

This project is deployed on Microsoft Azure.
