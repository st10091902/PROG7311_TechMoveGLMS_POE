# 🧠 TechMove GLMS – Global Logistics Management System

TechMove GLMS is an **ASP.NET Core MVC web application** developed for the **PROG7311 POE (Part 2)**.  
The system provides a functional prototype for managing logistics operations, including **clients, contracts, and service requests**, with integrated **currency conversion, file handling, and business rule validation**.

---

## 📌 Features

### ✅ Client Management
- Create, edit, view, and delete clients
- Stores client name, contact details, and region

### ✅ Contract Management
- Contracts linked to clients
- Status tracking: **Draft, Active, Expired, OnHold**
- Upload and download **PDF signed agreements**
- Filter contracts by:
  - Date range
  - Status (LINQ-based filtering)

### ✅ Service Request Management
- Linked to contracts
- Business rule enforcement:
  - ❌ Cannot create service request for **Expired or OnHold** contracts
- Tracks:
  - Description
  - Cost in USD
  - Automatically calculated Cost in ZAR

### ✅ Currency Conversion (API Integration)
- Uses external API via `HttpClient`
- Converts USD → ZAR automatically
- Saves converted values in database

### ✅ File Handling
- Upload PDF agreements
- Server-side storage in `/wwwroot/uploads`
- Download via UI
- Validation:
  - Only `.pdf` files allowed

### ✅ Unit Testing (xUnit)
- Separate test project
- Covers:
  - Business logic (workflow rules)
  - Currency conversion calculations
  - File validation

---

## 🛠 Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server (LocalDB)
- LINQ
- HttpClient (External API)
- xUnit (Unit Testing)
- Bootstrap (UI)
