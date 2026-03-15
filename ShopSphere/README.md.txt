# ShopSphere – Online Shopping Web Application

## Project Overview

ShopSphere is a simple online shopping web application developed using ASP.NET Core MVC.
The application allows users to view products, add items to a cart, and place orders.
An admin panel is available to manage products, product types, and special tags.

## Technologies Used

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server
* HTML
* CSS
* JavaScript
* Bootstrap

## Features

* User registration and login using Identity
* Secure authentication using ASP.NET Core Identity
* Product listing and product details page
* Add to cart functionality using session
* Order history for users
* Order placement
* Admin panel for product management
* Manage Product Types
* Manage Special Tags
* Alert messages for add, update, and delete operations

## Project Structure

* Areas – Admin and Customer functionality
* Controllers – Handles application logic
* Models – Data models for the application
* Views – Razor views for UI
* Data – Database context and configuration
* Utility – Helper classes such as Session extensions
* Services – Contains DbInitializer used for seeding default admin role and user

Data Management
* Product Types and Special Tags are managed dynamically from the Admin panel and do not                 require database seeding.

* Admin role and default admin user are seeded in the project using the DbInitializer    service.

## How to Run the Project

1. Clone the repository
2. Open the project in Visual Studio
3. Update the database connection string in appsettings.json
4. Run database migrations
5. Run the project

Default Admin Login

Email: admin@shopsphere.com
Password: Admin@123

##
