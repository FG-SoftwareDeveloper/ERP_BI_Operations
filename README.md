# ERP_BI_Operations

**ERP_BI_Operations** is a Business Intelligence and ERP web platform built for the construction industry. It delivers real-time KPIs, project insights, and AI-powered forecasting to streamline decision-making across departments like Finance, HR, Projects, and Operations.

## 🚀 Overview

This application combines traditional ERP capabilities with advanced business intelligence, integrating AI-powered insights using external services for prediction and analytics. It empowers teams to make data-driven decisions with clean, interactive dashboards and powerful backend logic.

---

## 🌐 Live Design & Community

- 🎨 **Figma Design System**: [View UI/UX Prototypes](#) ← *Replace with actual Figma link*
- 💼 **Connect on LinkedIn**: [Faith Garza](https://www.linkedin.com/in/faith-garza/)  
- 🧠 **Developer Portfolio**: [Place developer portfolio](#) ← *Optional*

---

## 💡 Key Features

- ✅ Role-based dashboards (Admin, HR, Finance, Projects, Operations)
- 📊 Financial summaries and real-time KPIs
- 🤖 AI-enhanced forecasting (project risks, cost overruns, predictive trends)
- 📈 Visual reporting via Chart.js and third-party plugins
- 🧾 Invoice management (AR/AP), payroll, job costing, resource tracking
- 🔐 Secure authentication with ASP.NET Identity
- 🔄 Scalable and extensible modular architecture

---

## 🧰 Tech Stack

### 🔧 Backend

- **.NET 8.0 (ASP.NET Core)** – Web API + Razor Pages
- **Entity Framework Core** – Code-first and reverse engineering from existing DB
- **SQL Server** – Relational database with normalized schemas
- **ASP.NET Identity** – Authentication and role-based authorization
- **Swagger** – API documentation and testing
- **Dependency Injection (DI)** – For context, services, and repositories

### 🧠 AI & Analytics

- **OpenAI API** (future integration) – Forecasting insights and recommendations
- **Microsoft Azure AI / ML Studio** – Predictive modeling (optional)
- **Chart.js + D3.js** – Interactive visual data insights
- **ML.NET** (optional) – Local training and predictive insights

### 💅 Frontend / UX

- **Razor Pages** – Templating and page-based routing
- **Bootstrap 5.3** – Responsive styling and UI components
- **Figma** – Full UI/UX design, wireframes, and prototyping
- **Lottie Animations** – Optional animated UI components

### 🛠 DevOps & Tools

- **Git + GitHub** – Version control and collaboration
- **Visual Studio 2022** – Main IDE
- **NuGet** – Package management
- **EF Power Tools** – DB scaffolding and reverse engineering


**Explanation of the Structure**:
**Controllers**/: Contains the C# controller classes that handle incoming HTTP requests, process data, and return views or API responses. Each major entity or functional area typically has its own controller.
**Views**/: Holds the Razor View files (.cshtml) that define the user interface.
**Shared**/: Contains layout files (_Layout.cshtml for the overall page structure including navigation), partial views, and common imports.
Subfolders per Controller: Each controller typically has a corresponding subfolder in Views (e.g., Views/Projects for ProjectsController). Inside these, you'll find views for Index (list/dashboard), Details, Create, Edit, etc.
**wwwroot**/: This is where all your static client-side assets are stored and served directly by the web server.
**css**/: Your CSS files, including your Tailwind CSS output.
**js**/: Your custom JavaScript files for client-side logic and interactions.
**lib**/: For third-party client-side libraries (like Chart.js) that you might include via CDN or local downloads.
**images**/: Any static image assets.
**Models**/: Contains your C# classes that represent the data entities in your database (as defined in your DbInitializer and SQL scripts). This also includes ApplicationUser for Identity.
**Data**/: Contains your Entity Framework Core DbContext class (ApplicationDbContext.cs) and the generated migration files.
**Services**/: A common place for business logic, data access abstractions (repositories), and other reusable services that don't directly map to a controller or model. This helps keep your controllers lean.
**Program.cs**: The entry point of your ASP.NET Core application, where services are configured and the request pipeline is built.
**appsettings.json**: Configuration files for your application (e.g., database connection strings).
This architecture provides a clean separation of concerns, making the project easier to manage, scale, and for multiple developers to work on.


