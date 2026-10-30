# WarehouseHub API 📦

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Fly.io](https://img.shields.io/badge/Fly.io-8B5CF6?style=for-the-badge&logo=flydotio&logoColor=white)

A robust and scalable backend Web API built with **.NET Core**, following **Clean Architecture** principles, designed for efficient warehouse management, inventory tracking, and secure access control.

---

## 🚀 Live Demo

Test and explore all the available endpoints directly via Swagger UI:

👉 **[WarehouseHub Live Swagger UI](https://warehousehub.fly.dev/swagger)**

---

## 🛠️ Tech Stack & Architecture

* **Framework & Language:** .NET Core, C#
* **Architecture Pattern:** Clean Architecture (API, Core, Domain, Infrastructure layers)
* **Database & ORM:** SQL Server (Cloud-hosted), Entity Framework Core (EF Core), Migrations
* **Authentication & Security:** ASP.NET Core Identity & JSON Web Token (JWT) Authentication
* **Design Patterns:** Repository Pattern, Specification Pattern
* **Deployment & Hosting:** Hosted live on **Fly.io**

---

## 📂 Project Structure (Clean Architecture)

The solution is organized into distinct layers to ensure separation of concerns and maintainability:

* **`WarehouseHub.Api`**: Presentation layer containing controllers, middleware, and dependency injection configurations.
* **`WarehouseHub.Core`**: Business logic layer holding application services, interfaces, DTOs, and business rules.
* **`WarehouseHub.Domain`**: Core enterprise entities and domain models.
* **`WarehouseHub.Infrastructure`**: Data access layer managing EF Core DbContext, database connections, and external services.
* **`WarehouseHub.UnitTests / IntegrationTests`**: Testing layers dedicated to ensuring code reliability and system behavior.

---

## 🔑 Key Features

* **Secure Authentication:** User registration and login powered by JWT tokens.
* **Inventory Management:** Full CRUD operations for managing warehouse products and stock levels.
* **Robust Data Access:** Optimized database queries and handling using Entity Framework Core.
* **Cloud Ready:** Fully deployed and continuously running on a remote cloud environment with live automated database access.

---

## 📡 API Endpoints (Overview)

> The full, interactive list is available on the [Swagger UI](https://warehousehub.fly.dev/swagger).

| Method | Endpoint                | Description                  | Auth |
|--------|-------------------------|------------------------------|------|
| POST   | `/api/auth/register`    | Register a new user          | ❌   |
| POST   | `/api/auth/login`       | Login and receive a JWT      | ❌   |
| GET    | `/api/products`         | Get all products             | ✅   |
| GET    | `/api/products/{id}`    | Get a product by ID          | ✅   |
| POST   | `/api/products`         | Create a new product         | ✅   |
| PUT    | `/api/products/{id}`    | Update an existing product   | ✅   |
| DELETE | `/api/products/{id}`    | Delete a product             | ✅   |

### Using the JWT token

After logging in, send the token in the `Authorization` header:

```http
Authorization: Bearer <your_token>
```

In Swagger UI, click **Authorize** and paste the token to test protected endpoints.

---

## ⚙️ Getting Started Locally

To run this project on your local machine for development or testing purposes, follow these steps:

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB, Express, or full instance)
* EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Clone the repository

```bash
git clone https://github.com/omarfares577-hue/WarehouseHub.git
cd WarehouseHub
```

### 2. Configure the connection string

Open `appsettings.Development.json` in the API project and update the connection string with your local SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=WarehouseHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

If you use SQL Server Authentication instead of Windows Authentication, use this version:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=WarehouseHubDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

### 3. Configure JWT settings

Add the JWT section to the same `appsettings.Development.json` file:

```json
"Jwt": {
  "Key": "YOUR_LONG_RANDOM_SECRET_KEY_AT_LEAST_32_CHARS",
  "Issuer": "WarehouseHub",
  "Audience": "WarehouseHubUsers",
  "ExpiryMinutes": 60
}
```

> ⚠️ **Never commit real secrets.** For local development, consider using [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):
> ```bash
> dotnet user-secrets set "Jwt:Key" "YOUR_SECRET" --project WarehouseHub.Api
> ```

### 4. Apply database migrations

Run the following command in your terminal to set up the database:

```bash
dotnet ef database update --project WarehouseHub.Infrastructure --startup-project WarehouseHub.Api
```

### 5. Run the application

```bash
dotnet run --project WarehouseHub.Api
```

Then open `https://localhost:<port>/swagger` to explore the endpoints.

---

## 🧪 Running Tests

```bash
dotnet test
```

---

## 🔒 Security Notes

* Secrets (connection strings, JWT keys) are **not** stored in the repository.
* In production, configuration is provided through environment variables / Fly.io secrets.

---

## 👨‍💻 Author

**Omar Sami**

[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/omarfares577-hue)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=flat&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/omar-fares-79b823373/)
