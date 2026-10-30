# WarehouseHub API 📦

A robust and scalable backend Web API built with **.NET Core**, following **Clean Architecture** principles, designed for efficient warehouse management, inventory tracking, and secure access control.

---

## 🚀 Live Demo
You can test and explore all the available endpoints directly via Swagger UI:
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

## ⚙️ Getting Started Locally
To run this project on your local machine for development or testing purposes, follow these steps:

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/omarfares577-hue/WarehouseHub.git](https://github.com/omarfares577-hue/WarehouseHub.git)
