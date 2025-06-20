# 🛍️ E-Commerce Book Store (.NET Core 8.0)

![.NET Core Version](https://img.shields.io/badge/.NET%20Core-8.0-blue)
![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Stars](https://img.shields.io/github/stars/AbdullahShaikh-dotnet/.Net-Core-Project-E-Commerce?style=social)
![Forks](https://img.shields.io/github/forks/AbdullahShaikh-dotnet/.Net-Core-Project-E-Commerce?style=social)


## 🧾 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.


## 📝 Highlights

⌞Modern, full-featured e-commerce solution built with .NET Core 8.0.⌝

---

## ✨ Features

- 🛒 Product catalog management
- 🔒 User authentication (Google, Facebook)
- 🛍️ Shopping cart & Order processing
- 💳 Razorpay payment integration
- 🛡️ Google reCAPTCHA v3
- 📧 Mailjet email service
- 🔗 WebSocket support
- 📃 Reporting (Invoice)
- 📝 Error Logging
- 📊 (Coming Soon) Recommendation engine & Analytics dashboard

---

## 🛠️ Technologies

- **Backend:** .NET Core 8.0, EF Core, ASP.NET Core Identity, C#
- **Frontend:** Razor Pages, TailwindCSS, Bootstrap 5, JavaScript (ES6+)
- **Database:** SQL Server, Redis (caching)
- **DevOps:** Docker
- **External APIs:** Razorpay, Google Identity, Facebook Login, Mailjet

---

## 🏛️ Architecture Details
- Layered Architecture.
- SOLID Principle.
- DRY ( Dont Repeat Yourself ).
- Repository Pattern.
- Separation of Concerns (SoC)

---

## 📸 Screenshots

| Page                 | Preview                                        |
|----------------------|------------------------------------------------|
| Home Page            | ![Home](/screenshots/home.png)                |
| Product Page         | ![Product](/screenshots/product.png)           |
| Shopping Cart        | ![Cart](/screenshots/shoppingCart.png)         |
| Order Summary        | ![Summary](/screenshots/orderSummary.png)      |
| Manage Orders (Admin)| ![Manage1](/screenshots/manageOrder1.png) <br> ![Manage2](/screenshots/manageOrder2.png) |

---

## 💻 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [VS Code](https://code.visualstudio.com/)
- [Redis Server](https://redis.io/)
- [Docker](https://www.docker.com/)
- Developer Accounts (Razorpay, Google, Facebook, Mailjet)

---


## ⚙ Configuration
Add Configuration in appsettings.Development.json

**Recommanded Way**
Add API Keys in Enviroment Variable or in Secret.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ECommerceDB;Trusted_Connection=True;"
  },
  "Razorpay": {
    "KeyId": "your_razorpay_key",
    "KeySecret": "your_razorpay_secret"
  },
  "Google": {
    "ClientId": "your_google_client_id",
    "ClientSecret": "your_google_secret",
    "RecaptchaSiteKey": "your_site_key",
    "RecaptchaSecretKey": "your_secret_key"
  },
  "Facebook": {
    "AppId": "your_fb_app_id",
    "AppSecret": "your_fb_app_secret"
  },
  "Mailjet": {
    "ApiKey": "your_mailjet_key",
    "ApiSecret": "your_mailjet_secret",
    "SenderEmail": "noreply@yourdomain.com"
  }
}
```



## 🚀 Quick Setup
```bash
git clone https://github.com/AbdullahShaikh-dotnet/.Net-Core-Project-E-Commerce.git
cd .Net-Core-Project-E-Commerce
dotnet restore
dotnet ef database update
```


**To create a new migration**
``` bash
dotnet ef migrations add InitialMigration
dotnet ef database update
```


**Or using Package Manager Console**
```bash
add-migration InitialCreate
update-database
```



**🐋 Redis Container in Docker**
```bash
docker run -p 6379:6379 redis-master -e REDIS_REPLICATION_MODE=master -e ALLOW_EMPTY_PASSWORD=yes bitnami/redis:latest
```



<p align="center">
  Made with ❤️ by <a href="https://github.com/AbdullahShaikh-dotnet" target="_blank">Abdullah Shaikh</a>
</p>

<p align="center">
  <a href="https://github.com/AbdullahShaikh-dotnet/.Net-Core-Project-E-Commerce/stargazers" target="_blank">
    ⭐ Star this project
  </a>
  ·
  <a href="https://github.com/AbdullahShaikh-dotnet/.Net-Core-Project-E-Commerce/issues" target="_blank">
    🐛 Report Issues
  </a>
</p> 

