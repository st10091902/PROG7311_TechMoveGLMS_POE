# 🚚 TechMove GLMS – Global Logistics Management System

TechMove GLMS is an ASP.NET Core solution developed for the **PROG7311 POE**.

The project began as a monolithic ASP.NET Core MVC prototype in Part 2 and was modernised in Part 3 into a service-oriented, containerised application.

The final solution consists of:

* An **ASP.NET Core MVC frontend**
* An **ASP.NET Core Web API backend**
* A **SQL Server database**
* **Docker Compose** orchestration
* **Swagger/OpenAPI** documentation
* **API key authentication**
* **Unit and integration tests**

---

## 📌 Project Overview

TechMove Logistics requires a system to manage logistics-related operations such as clients, contracts, and service requests.

The system supports:

* Client management
* Contract management
* PDF agreement uploads
* Service request processing
* Business rule validation
* USD to ZAR currency conversion
* API-based communication
* Automated testing
* Docker containerisation

---

## 🧱 Solution Architecture

The system is divided into three main layers:

```text
MVC Frontend  →  Web API Backend  →  SQL Server Database
```

### Frontend: `TechMoveGLMS`

The MVC application acts as the presentation layer.

It no longer connects directly to the database. Instead, it uses `HttpClient` to communicate with the backend API.

### Backend: `TechMoveGLMS.Api`

The Web API is responsible for:

* Database access
* Business logic
* Validation rules
* Currency conversion
* Returning JSON data
* Providing Swagger/OpenAPI documentation

### Database: SQL Server

The database stores:

* Clients
* Contracts
* Service Requests

When running through Docker, SQL Server runs in its own container.

---

## 🚀 Features

### ✅ Client Management

* Create clients
* View client list
* View client details
* Edit clients
* Delete clients

### ✅ Contract Management

* Create contracts linked to clients
* Edit contract details
* Delete contracts
* Upload signed agreement PDFs
* Download uploaded agreements
* Filter contracts by:

  * Start date
  * End date
  * Status

### ✅ Service Request Management

* Create service requests linked to contracts
* Edit service requests
* Delete service requests
* View converted ZAR value
* API calculates local cost from USD

### ✅ Workflow Validation

The system prevents service requests from being created for contracts with the following statuses:

* Expired
* OnHold

This business rule is enforced by the backend API.

### ✅ External API Integration

The backend uses `HttpClient` to retrieve the current USD to ZAR exchange rate.

The system:

1. Accepts a USD amount
2. Calls the exchange rate API
3. Calculates the ZAR amount
4. Saves the converted value

### ✅ File Handling

Contracts support PDF agreement uploads.

The system includes:

* PDF-only validation
* Unique file naming using GUIDs
* Server-side storage
* Download links in the UI

### ✅ API Key Authentication

The backend API is protected using an API key.

Requests must include:

```text
X-API-KEY
```

The MVC frontend automatically sends the API key when calling the Web API.

### ✅ Swagger/OpenAPI

Swagger is enabled for the Web API and can be used to inspect and test available endpoints.

---

## 🛠 Tech Stack

* ASP.NET Core MVC
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* LINQ
* HttpClient
* Swagger/OpenAPI
* xUnit
* Docker
* Docker Compose
* Bootstrap

---

## 📁 Project Structure

```text
TechMoveGLMS
│
├── TechMoveGLMS
│   ├── Controllers
│   ├── Models
│   ├── Views
│   ├── ViewModels
│   ├── Services
│   ├── Repositories
│   ├── Data
│   ├── wwwroot
│   └── Dockerfile
│
├── TechMoveGLMS.Api
│   ├── Controllers
│   ├── Middleware
│   ├── appsettings.json
│   └── Dockerfile
│
├── TechMoveGLMS.Tests
│   ├── Services
│   └── IntegrationTests
│
├── docker-compose.yml
├── .dockerignore
└── README.md
```

---

## 🗄 Database

The database is managed using Entity Framework Core.

Main tables:

* Clients
* Contracts
* ServiceRequests

Relationships:

* One Client can have many Contracts
* One Contract can have many Service Requests

The database connection string is stored in the API project configuration.

When running with Docker Compose, the API connects to the SQL Server container using internal Docker networking.

---

## 🌐 Web API Endpoints

The API exposes REST endpoints for the main system resources.

### Clients

```text
GET     /api/clients
GET     /api/clients/{id}
POST    /api/clients
PUT     /api/clients/{id}
DELETE  /api/clients/{id}
```

### Contracts

```text
GET     /api/contracts
GET     /api/contracts/{id}
POST    /api/contracts
PUT     /api/contracts/{id}
PATCH   /api/contracts/{id}/status
DELETE  /api/contracts/{id}
```

### Service Requests

```text
GET     /api/service-requests
GET     /api/service-requests/{id}
POST    /api/service-requests
PUT     /api/service-requests/{id}
DELETE  /api/service-requests/{id}
```

---

## 🔐 API Authentication

The API uses a simple API key authentication approach.

The API expects the following request header:

```text
X-API-KEY: TechMove-Part3-Demo-Key
```

The MVC frontend sends this key automatically through its configured `HttpClient`.

---

## 🐳 Running with Docker Compose

The final system runs using Docker Compose with three containers:

```text
sql-server-db
glms-backend-api
glms-frontend-web
```

### Docker Services

| Service             | Description               | Port         |
| ------------------- | ------------------------- | ------------ |
| `sql-server-db`     | SQL Server database       | `14333:1433` |
| `glms-backend-api`  | ASP.NET Core Web API      | `5001:8080`  |
| `glms-frontend-web` | ASP.NET Core MVC frontend | `5000:8080`  |

### Run the Full System

From the solution root folder, run:

```bash
docker compose up --build
```

### Open the MVC Frontend

```text
http://localhost:5000
```

### Open API Swagger

```text
http://localhost:5001/swagger
```

---

## 🧪 Testing

The solution includes a separate test project:

```text
TechMoveGLMS.Tests
```

The test project includes both:

* Unit tests
* API integration tests

### Unit Tests

Unit tests cover:

* Service request workflow validation
* Currency conversion calculations
* PDF file validation

### Integration Tests

Integration tests call API endpoints and verify responses.

Examples include:

* `GET /api/clients` returns `200 OK`
* `GET /api/contracts` returns `200 OK`
* `GET /api/service-requests` returns `200 OK`
* API rejects requests without the required API key

### Run Tests

```bash
dotnet test
```

All tests should pass.

---

## 🧪 Test Coverage Summary

The test suite verifies:

* Valid service requests can be created for active contracts
* Service requests are blocked for expired or on-hold contracts
* USD to ZAR conversion is calculated correctly
* Currency values are rounded correctly
* Only PDF files are accepted
* Invalid file types are rejected
* Null and empty file names are handled
* API endpoints return successful responses
* API authentication rejects unauthorised calls

---

## 📦 Docker Files

The solution includes:

```text
TechMoveGLMS/Dockerfile
TechMoveGLMS.Api/Dockerfile
docker-compose.yml
.dockerignore
```

These files allow the full system to run in containers.

---

## 🧠 Design Patterns and Architecture

The solution applies:

### MVC Pattern

Used in the frontend to separate:

* Models
* Views
* Controllers

### Repository Pattern

Used to abstract database access logic.

### Service Layer

Used to separate business rules and validation from controllers.

### Service-Oriented Architecture

Part 3 modernises the system by separating the frontend from the backend API.

The MVC frontend now acts as the presentation layer, while the Web API acts as the service layer.

---

## 📚 References

* Microsoft Docs – ASP.NET Core MVC
* Microsoft Docs – ASP.NET Core Web API
* Microsoft Docs – Entity Framework Core
* Microsoft Docs – HttpClient
* Microsoft Docs – xUnit
* Microsoft Docs – Docker with .NET
* Docker Documentation
* Fowler, M. (2003). *Patterns of Enterprise Application Architecture*
* Evans, E. (2003). *Domain-Driven Design*
* Gamma, E., Helm, R., Johnson, R. and Vlissides, J. (1994). *Design Patterns: Elements of Reusable Object-Oriented Software*

---

## ✅ Conclusion

TechMove GLMS demonstrates a modernised enterprise prototype that separates presentation, service, and database responsibilities.

The final solution includes:

* A containerised MVC frontend
* A containerised Web API backend
* A containerised SQL Server database
* API authentication
* Swagger documentation
* Automated unit and integration tests

This provides a strong foundation for a scalable, cloud-native logistics management system.
