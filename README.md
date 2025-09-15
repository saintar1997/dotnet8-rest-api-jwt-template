# .NET 8 REST API Template

โครงการนี้เป็น **REST API Template** ที่สามารถนำไปใช้ต่อยอดได้ทันที  
ใช้เทคโนโลยีหลักดังนี้:
```markdown
- .NET 8
- SQL Server + Dapper
- JWT Bearer Token Authentication
- Unit Test ด้วย xUnit (AAA: Arrange – Act – Assert)

```

## 📐 สถาปัตยกรรม

ข้อมูลจะไหลดังนี้:

```markdown

HTTP Request → Controller → Service → Repository (Dapper, SQL Server) → DTOs → Models → HTTP Response

```

---

## 📂 โครงสร้างโฟลเดอร์

```markdown
Dotnet8RestApiJwtTemplate.Api/
┣ Attributes/        # Custom Attributes (Validation, Authorization, etc.)
┣ Clients/           # External Clients (HTTP Client, gRPC, etc.)
┣ Configs/           # Configuration & Options
┃ ┣ AppSettings.cs
┃ ┣ DatabaseOptions.cs
┃ ┣ JwtOptions.cs
┃ ┣ ISqlConnectionFactory.cs
┃ ┣ SqlConnectionFactory.cs
┃ ┣ UrlService.cs
┃ ┗ KebabCaseParameterTransformer.cs
┣ Constants/         # Constant Values (Claim keys, default values)
┣ Controllers/       # API Controllers
┃ ┣ AuthenController.cs
┃ ┗ HealthCheckController.cs
┣ DTOs/              # Data Transfer Objects (DB ↔ DTO ↔ Models)
┣ Enums/             # Enumerations (Status, Role, etc.)
┣ Models/            # Request/Response Models (used at HTTP layer)
┃ ┣ AuthenModel/
┃ ┃ ┣ AuthenRequest.cs
┃ ┃ ┗ AuthenResponse.cs
┃ ┗ HealthCheckModel/
┃   ┗ HealthCheckResponse.cs
┣ Repositories/      # Data Access Layer (Dapper queries to SQL Server)
┣ Services/          # Business Logic Layer
┃ ┣ AuthenService/
┃ ┃ ┣ IAuthenService.cs
┃ ┃ ┗ AuthenService.cs
┃ ┗ HealthCheckService/
┃ ┃ ┣ IHealthCheckService.cs
┃ ┃ ┗ HealthCheckService.cs
┗ Utilities/         # Helpers/Utilities (e.g., ZipJsonExporter)
Dotnet8RestApiJwtTemplate.Test/
┣ Controllers/       # Unit tests for Controllers (AAA pattern)
┃ ┣ AuthControllerTests.cs
┃ ┗ HealthCheckControllerTests.cs
┣ Services/          # Unit tests for Services (AAA pattern)
┃ ┣ AuthenServiceTests.cs
┃ ┗ HealthCheckServiceTests.cs
┗ Dotnet8RestApiJwtTemplate.Test.csproj

````

---

## 🛠 Project Setup (Create from scratch)

ถ้าต้องการสร้าง solution และ projects แบบเดียวกับ template นี้จากศูนย์ ใช้คำสั่งต่อไปนี้:

```bash
# สร้าง solution
dotnet new sln -n Dotnet8RestApiJwtTemplate

# สร้าง Web API project (.NET 8, มี Controller)
dotnet new webapi -n Dotnet8RestApiJwtTemplate.Api --use-controllers

# สร้าง xUnit test project
dotnet new xunit -n Dotnet8RestApiJwtTemplate.Test

# เพิ่มทั้งสอง project เข้าไปใน solution
dotnet sln Dotnet8RestApiJwtTemplate.sln add \
  Dotnet8RestApiJwtTemplate.Api/Dotnet8RestApiJwtTemplate.Api.csproj \
  Dotnet8RestApiJwtTemplate.Test/Dotnet8RestApiJwtTemplate.Test.csproj
```
---
## ⚙️ การติดตั้งและการใช้งาน

### 1. Clone repository
```bash
git clone https://github.com/<your-username>/dotnet8-rest-api-template.git
cd dotnet8-rest-api-template
````

### 2. ตั้งค่า Database

* สร้าง SQL Server Database
* อัพเดต connection string ใน `appsettings.Development.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TemplateDb;User Id=sa;Password=your_password;TrustServerCertificate=True"
}
```

### 3. ตั้งค่า JWT

แก้ไข section `JwtOptions` ใน `appsettings.Development.json`

```json
"JwtOptions": {
  "Issuer": "TemplateApi",
  "Audience": "TemplateApiUsers",
  "SecretKey": "YourSuperSecretKeyForJwtToken"
}
```

### 4. รัน API

```bash
dotnet run --project src/YourApiProject
```

เปิด Swagger UI:
[http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## ✅ การทดสอบ

ใช้ **xUnit** + AAA Pattern (Arrange – Act – Assert)

### รันทดสอบทั้งหมด

```bash
dotnet test
```

### ตัวอย่าง Test (AAA)

```csharp
[Fact]
public void GenerateToken_ShouldReturnValidJwt()
{
    // Arrange
    var service = new TokenService(...);

    // Act
    var token = service.Generate("clientSecret", "clientId");

    // Assert
    Assert.NotNull(token);
    Assert.Contains("ey", token); // JWT header prefix
}
```

---

## 🔄 Flow ตัวอย่าง

```
HTTP Request → TerritorialCustomersController
    → TerritorialCustomerService
        → TerritorialCustomerRepository
            → (Dapper) SQL Server Query → DTO
        → DTO → Response Model
    → HTTP Response (JSON)
```

---

## 🚀 Features

* ✅ Authentication ด้วย JWT Bearer Token
* ✅ Data Access ผ่าน Dapper (lightweight + performance)
* ✅ แยก Layer ตาม Clean Architecture (Controller, Service, Repository)
* ✅ Unit Test ครอบคลุม Business Logic ด้วย xUnit

---

## 📜 License

MIT License


