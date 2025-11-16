
# 🏥 **Pharmazy – Online Medicine Store**

### ASP.NET Core MVC • SQL Server • Identity Framework • EF Core Code-First • Repository Pattern • jQuery SPA

**Pharmazy** is a full-featured **medicine e-commerce platform** built with **ASP.NET Core MVC**, following clean, scalable multi-layer architecture using **DAL, BAL, Models**, **Repository Pattern**, **Dependency Injection**, and **Entity Framework Core (Code-First)**.

It supports **Admin**, **Buyer**, and **Hybrid Roles**, offering features like **Medicine & Category Management**, **Multi-Stock per Medicine**, **Add to Cart**, **Checkout**, **Identity-based Authentication**, and a **jQuery-powered single-page experience**.

---

## 🚀 **Features**

### 👨‍💼 **Role-Based Access (ASP.NET Identity)**

#### **Admin**

* Manage Categories (CRUD)
* Manage Medicines (CRUD)
* Manage Medicine Stock (multiple expiry cycles)
* View & manage all buyers
* Track & view orders

#### **Buyer**

* Browse medicines
* Add items to cart
* Purchase medicines
* View own orders

#### **Hybrid Role (Admin + Buyer)**

* Access both Admin & Buyer dashboards

---

## 📦 **Core Functionalities**

### 🔹 **Medicine & Category Management**

* Full CRUD using MVC + EF Core
* Server-side validation on all inputs
* Clean admin interface using jQuery & Bootstrap

### 🔹 **Multi-Stock Management (Expiry-Based)**

Each medicine can have **multiple stock entries**, each containing:

* Expiry Date
* Quantity
* Batch Number
* Manufacturing Details

System supports:

* **Batch-wise stock deduction**
* **Expiry-based stock priority (FEFO logic)**
* Preventing checkout of expired stock

### 🛒 **E-Commerce & Cart System**

* Add to cart
* Update quantity
* Remove items
* Real-time cart updates using jQuery AJAX
* Order placement & order history

### 🔐 **Authentication & Security**

* ASP.NET Core **Identity Framework**
* Login, Registration, Logout
* Hashed passwords
* Code-first Identity tables
* Role-based authorization for Admin/Buyer

### 🧱 **Architecture**

* **ASP.NET Core MVC 7+**
* **EF Core Code-First** Migrations
* **Data Access Layer (DAL)** – Repositories
* **Business Access Layer (BAL)** – Services
* **Model Library** – Entities, ViewModels, DTOs
* **Repository Pattern** – Clean, testable, decoupled
* **Dependency Injection** – Scoped services & repos
* **jQuery SPA-like Flow** – AJAX-rendered UI
* **Toastr** notifications for smooth UX

---

## 🛠️ **Tech Stack**

| Layer              | Technologies Used                                         |
| ------------------ | --------------------------------------------------------- |
| **Frontend**       | jQuery, AJAX, Bootstrap 5, HTML5, CSS3, Toastr            |
| **Backend**        | ASP.NET Core MVC 7+, C#, LINQ                             |
| **Database**       | SQL Server, Entity Framework Core (Code-First Migrations) |
| **Authentication** | ASP.NET Core Identity Framework                           |
| **Architecture**   | N-Tier: DAL, BAL, Models, UI + Repository Pattern + DI    |
| **Tools**          | Visual Studio, SSMS, Git, NuGet, Postman                  |

---

## 🏗️ **Project Structure**

```
Pharmazy/
│
├── Pharmazy.Web/                # ASP.NET Core MVC Application (UI Layer)
│   ├── Areas/                   # Admin/Buyer Panels
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   ├── wwwroot/
│   │   ├── js/                  # jQuery + AJAX scripts
│   │   └── css/
│   └── Program.cs
│
├── Pharmazy.DAL/                # Data Access Layer
│   ├── Contacts/                # Repository Interfaces
│   ├── Data/                    # DbContext
│   ├── Migrations/              # EF Core Migrations
│   └── Repositories/            # Repository Implementations
│
├── Pharmazy.BAL/                # Business Layer
│   └── Services/                # Business Logic
│
├── Pharmazy.Models/             # Entity & View Models
│   └── DTO/                     # Data Transfer Objects
│
└── README.md
```

---

## ⚙️ **Setup Instructions**

### 1️⃣ Clone the project

```bash
git clone https://github.com/Rahilsamani/PharmEazy.git
```

### 2️⃣ Configure SQL Server connection

Open `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=PharmazyDB;Trusted_Connection=True;"
}
```

### 3️⃣ Apply EF Core Migrations

```bash
update-database
```

### 4️⃣ Run the application

```bash
dotnet run
```

---

## 💡 **Key Highlights**

✔ Fully structured multi-layer architecture
✔ EF Core Code-First with clean migrations
✔ ASP.NET Identity authentication
✔ Role-based authorization
✔ Multi-stock system with expiry dates
✔ Repository Pattern + DI
✔ SPA-like experience using jQuery & AJAX
✔ Responsive and modern UI with Toastr alerts

---

## 🤝 **Contributing**

Pull requests are welcome.
For major changes, please open an issue first to discuss the proposal.

---

## ⭐ **Support the Project**

If you found this helpful, please **star ⭐ the repository** — it motivates future updates!
