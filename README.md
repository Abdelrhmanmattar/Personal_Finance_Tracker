# Personal_Finance_Tracker - ASP.NET Core Clean Architecture with Docker
This is a modular ASP.NET Core 8 Web API solution following Clean Architecture principles, built for scalability, maintainability, and easy deployment using Docker and Docker Compose. It provides a robust platform for collaborative budgeting, income/expense tracking, and real-time financial notifications.

# ❓ What Can This API Do?
The Personal_Finance_Tracker API allows users to manage their financial data with a strong emphasis on real-time feedback and collaboration:
## ✨ Features
**User Accounts**  
Securely log in and sign up.

**Budget Management**  
Create, view, update, and delete budgets. Critically, it supports collaborative budgets, where admins can add or remove other users, triggering real-time notifications.

**Category Organization**  
Define and manage financial categories for better organization.

**Income & Expense Tracking**  
Log income and expenses. Expenses are linked to budgets, and real-time notifications are sent when a budget limit is approached or exceeded.

**Financial Reports**  
Generate on-demand summary reports of income, expenses, and budget utilization.

**Persistent Notifications**  
All critical notifications are saved to the database for historical tracking.

##  🧰  Technology Stack

This project leverages a modern and powerful set of technologies:

**Backend:** ASP.NET Core 8 Web API  
**Database ORM:** Entity Framework Core (EF Core) with LINQ  
**Real-time Communication:** SignalR  
**Caching:** Redis  
**Containerization:** Docker & Docker Compose


## 📁 Project Structure

Personal_Finance_Tracker/

├── Personal_Finance_Tracker.sln

├── Dockerfile

├── docker-compose.yml

├── Core/

├── Personal_Finance_Tracker.API/

├── Repository/

├── Services/

└── README.md

## 🛠️ Prerequisites

- [.NET 8 SDK]
- [Docker]
- [MSSQL]

## 🚀 Getting Started

### 1. Clone the Repository
Clone
```
git clone https://github.com/Abdelrhmanmattar/Personal_Finance_Tracker.git
cd Personal_Finance_Tracker
```
ASP.NET
```
dotnet clean Personal_Finance_Tracker.sln
dotnet restore Personal_Finance_Tracker.sln
dotnet build Personal_Finance_Tracker.sln
```
DOCKER
```
docker build -t myproject-api -f Dockerfile .
docker run -d -p 5000:80 myproject-api
docker-compose up --build
```
Test&Run
```
dotnet run --project .\Personal_Finance_Tracker.API\Personal_Finance_Tracker.API.csproj

```
