# ⚡ DevPulse — Developer Productivity & Engineering Workspace

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16%2B-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/ef/core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> A modern developer cockpit and engineering workspace built with **ASP.NET Core (.NET 10)**, **Entity Framework Core**, and **PostgreSQL**. Track software projects, manage backlogs with an interactive Kanban board, monitor development hours, and accelerate developer workflows.

---

## 🌟 Key Features

### 📊 1. Engineering Cockpit Dashboard
* **Real-time Velocity & Metrics**: Live metrics calculating active vs. completed projects, total backlog tasks, and overall delivery rate.
* **Hours & Learning Tracker**: Monitor logged development hours against planned estimates with budget warning indicators.
* **Upcoming Deadlines Widget**: Immediate action list of tasks with near deadlines, complete with one-click completion.
* **AI Tools Hub**: Curated directory with launch links for top developer AI assistants (*ChatGPT, GitHub Copilot, Claude, v0*).

### 📋 2. Interactive Kanban Task Board
* **Visual Workflows**: Categorized into **Todo**, **In Progress**, and **Done** columns.
* **One-Click Progression**: Seamlessly transition tasks (`▶ Start`, `✓ Done`, `↺ Reopen`) directly from cards.
* **Global & Project Backlogs**: Access `/Tasks` to inspect all tasks across every project or isolate by specific project.
* **Priority & Project Tags**: Color-coded badges indicating severity (*High, Medium, Low*) and assigned repository/project.

### 📁 3. Project Lifecycle Management
* **Full CRUD Operations**: Create, update, view, and safely delete projects with cascading task cleanups.
* **Auto-Sync Delivery Progress**: One-click **`⟳ Sync with Tasks`** automatically recalculates project completion percentage based on finished tasks.
* **Time & Budget Analysis**: Visual comparison of `HoursSpent` vs. `EstimatedHours` with warning states when hours exceed estimates.
* **Multi-Filter & Search**: Instant filtering by status (*Active, Completed, Paused*), priority, and sorting (*Newest, Oldest, Name, Progress, Deadline*).

### 🛡️ 4. Enterprise-Grade Reliability & UX
* **Protected Operations**: Anti-forgery CSRF tokens on all mutation endpoints and safe deletion prompts.
* **Data Validation**: Strict model data annotations and unobtrusive client-side validation.
* **Alert Notifications**: Dismissible feedback banners for real-time operation confirmation.
* **Dark Mode Theme**: Sleek, eye-friendly developer workspace styling designed with modern CSS gradients and glassmorphism.

---

## 🛠️ Tech Stack

| Component | Technology |
|---|---|
| **Framework** | [ASP.NET Core MVC (.NET 10)](https://dotnet.microsoft.com/) |
| **Language** | C# 13 |
| **Database** | [PostgreSQL](https://www.postgresql.org/) |
| **ORM** | [Entity Framework Core 10](https://docs.microsoft.com/ef/core/) (Npgsql Provider) |
| **Frontend** | Razor Views, Semantic HTML5, CSS3 Glassmorphism, Vanilla JS, jQuery Validation |

---

## 📂 Project Structure

```text
DevPulse/
├── Controllers/
│   ├── DashboardController.cs    # Aggregates analytics, tasks & AI tools
│   ├── HomeController.cs         # Core routing, Error handling & Privacy
│   ├── ProjectsController.cs     # Project CRUD, filtering, sorting & progress sync
│   └── TasksController.cs        # Kanban board, status transitions & task CRUD
├── Data/
│   └── DevPulseDbContext.cs      # EF Core database context & schema mappings
├── Migrations/                   # Database versioning and schema migrations
├── Models/
│   ├── AiTool.cs                 # AI tool definition & external links
│   ├── DashboardViewModel.cs     # Dashboard composite metrics view model
│   ├── Project.cs                # Project entity with validation rules
│   └── TaskItem.cs               # Task entity with status & priority
├── Views/
│   ├── Dashboard/                # Dashboard overview views
│   ├── Home/                     # Privacy policy & general pages
│   ├── Projects/                 # Index, Create, Edit, Details, Delete
│   ├── Shared/                   # Layout, partials, and notifications
│   └── Tasks/                    # Kanban board, Create, and Edit views
├── wwwroot/
│   └── css/site.css              # Custom dark-mode developer design system
├── appsettings.json              # Connection strings and configuration
└── Program.cs                    # Application bootstrapping & middleware pipeline
```

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
* [PostgreSQL](https://www.postgresql.org/download/) (v14+ recommended)

---

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/devpulse.git
   cd devpulse
   ```

2. **Configure PostgreSQL Database:**
   Open `appsettings.json` and adjust the connection string to match your PostgreSQL server credentials:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=DevPulseDb;Username=postgres;Password=your_password"
   }
   ```

3. **Apply Database Migrations:**
   *(DevPulse automatically attempts to apply pending migrations upon app launch. You can also run it manually:)*
   ```bash
   dotnet ef database update
   ```

4. **Build and Run:**
   ```bash
   dotnet run
   ```

5. **Open in Browser:**
   Navigate to `http://localhost:5062` (or the port specified in your console).

---

## ⚡ Development Commands

* **Run with Hot-Reload:**
  ```bash
  dotnet watch run
  ```
* **Build Project:**
  ```bash
  dotnet build
  ```
* **Add New Migration:**
  ```bash
  dotnet ef migrations add <MigrationName>
  ```

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
