# Banking API Solution

This is a simple REST API for a banking application, built with .NET 9, ASP.NET Core, and EF Core. It fulfills the requirements of the C# homework assignment.

## Features

* **Account Management:** Create new accounts, get account details, and list all accounts.
* **Account Transactions:** Deposit, withdraw, and transfer funds between accounts.
* **Data Integrity:** Fund transfers are performed using EF Core's database transactions to ensure atomicity (all or nothing).
* **Robust Validation:** All API inputs are validated using `FluentValidation` to ensure data integrity before hitting the service layer.
* **Global Error Handling:** A central middleware class handles all exceptions (e.g., `NotFound`, `InvalidOperation`) and returns clean, consistent JSON error responses.
* **API Documentation:** A Swagger (OpenAPI) interface is available at `/swagger` to view and test all endpoints.

---

## 1. Technology Stack & Design Choices

* **Framework:** **ASP.NET Core 9** was chosen as the modern, high-performance framework for building the REST API.
* **Language:** **C#** (as required).
* **Database:** **Microsoft SQL Server** with **Entity Framework Core 9** as the ORM.
* **Architecture (Clean Architecture):**
    * `BankingApi.Core`: Contains only the `Account` domain entity. It has no dependencies.
    * `BankingApi.Infrastructure`: Manages data access, containing the `BankingDbContext` and EF Core migrations.
    * `BankingApi.Application`: Contains all business logic, interfaces (`IAccountManagementService`, `ITransactionService`), DTOs, and services.
    * `BankingApi.Api`: The public-facing API layer, containing only the Controllers and `Program.cs`.
* **Design Patterns:**
    * **Interface Segregation:** The service logic was split into `IAccountManagementService` and `ITransactionService`. This creates a cleaner, more secure design where controllers only have access to the methods they truly need.
    * **Middleware:** A `GlobalErrorHandlingMiddleware` intercepts all exceptions, removing repetitive `try/catch` blocks from controllers.
* **Quality & Tooling:**
    * **Input Validation:** **FluentValidation** is used to enforce business rules on all DTOs (e.g., positive amounts, required names).
    * **Object Mapping:** **AutoMapper** is used to automatically map `Account` domain entities to `AccountResponse` DTOs, removing boilerplate code from controllers.
    * **Unit Testing:** **xUnit** was used for unit testing the service layer. The **EF Core In-Memory Database** provides a fast, isolated database for each test, with transaction warnings suppressed to allow for testing the `TransferAsync` logic.

---

## 2. How to Set Up and Run

### Prerequisites

* .NET 9 SDK
* A running instance of SQL Server (Express, Developer, or localdb).

### Step 1: Configure Database

1.  Open the `BankingApi/BankingApi/appsettings.json` file.
2.  Find the `ConnectionStrings` section.
3.  Update the `DefaultConnection` string to point to your local SQL Server instance.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=BankingSolution;Trusted_Connection=True;TrustServerCertificate=True"
    }
    ```

### Step 2: Run Migrations

To create the database and `Accounts` table, run the following command from the solution root folder (the one containing `BankingApi.sln`):

```bash
dotnet ef database update --startup-project BankingApi
