---

# **IdentityService Microservice**

**IdentityService** is a dedicated **authentication and authorization microservice** for a larger microservices architecture. It provides centralized user management, token issuance, and role-based access control for all other microservices.

It is built with **.NET 8**, **ASP.NET Core Identity**, **OpenIddict**, and **PostgreSQL**.

---

## **Architecture Overview**

```
┌─────────────────────┐
│  IdentityService    │
│---------------------│
│ User Registration   │
│ Login / Tokens      │
│ Role Management     │
│ OpenID Connect      │
└─────────┬───────────┘
          │ JWT / OAuth2 Tokens
          ▼
┌─────────────────────┐
│ Other Microservices │
│---------------------│
│ OrdersService       │
│ PaymentsService     │
│ ProductService      │
│ ...                 │
└─────────────────────┘
```

**Key Idea:**
All other microservices do **not handle user credentials directly**. They rely on **JWT tokens issued by IdentityService** to authorize requests.

---

## **Features**

* **User Registration** (`/api/auth/register`)
* **User Login** (`/api/auth/login`) with JWT access + refresh tokens
* **Refresh Access Token** (`/api/auth/refresh`)
* **Logout** (`/api/auth/logout`)
* **Fetch Current User Profile** (`/api/users/me`)
* **Admin Management** (list users, assign roles)
* **Role-Based Authorization**
* **OpenID Connect Endpoints** (`/connect/token`, `/connect/authorize`, etc.)
* **Global Exception Handling** for consistent API responses

---

## **Technologies**

* **.NET 8 / ASP.NET Core Identity**
* **OpenIddict** (OAuth2/OpenID Connect)
* **PostgreSQL** (via EF Core)
* **Entity Framework Core**
* **JWT Authentication**
* **Docker** (optional for database)
* **Swagger / OpenAPI** (optional for API docs)

---

## **Getting Started**

### **1. Prerequisites**

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* PostgreSQL (local installation or Docker)
* EF Core CLI tools

---

### **2. Clone Repository**

```bash
git clone https://github.com/usr-0123/Identity
cd identity-service
```

---

### **3. Configure PostgreSQL**

**Docker Example:**

```bash
docker run --name identity-postgres -e POSTGRES_USER=identityuser -e POSTGRES_PASSWORD=YourStrongPassword -e POSTGRES_DB=identitydb -p 5432:5432 -d postgres
```

Update `appsettings.json`:

```json
{
   "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5432;Database=identitydb;Username=identityuser;Password=YourStrongPassword"
   }
}
```

---

### **4. Run Database Migrations**

```bash
dotnet ef database update
```

> This creates ASP.NET Identity and OpenIddict tables in PostgreSQL.

---

### **5. Seed Roles and OpenIddict Client**

```csharp
using var scope = app.Services.CreateScope();
await DbSeeder.SeedRolesAndClients(scope.ServiceProvider);
```

Seeds:

* Default roles: `Admin`, `User`
* Default client: `default-client` (for password/refresh flows)

---

### **6. Run the API**

```bash
dotnet run
```

**Default URL:** `https://localhost:5001`

---

## **Endpoints**

### **Auth Endpoints**

| Endpoint             | Method | Auth | Description           |
| -------------------- | ------ | ---- | --------------------- |
| `/api/auth/register` | POST   | ❌    | Register a new user   |
| `/api/auth/login`    | POST   | ❌    | Login & get tokens    |
| `/api/auth/refresh`  | POST   | ❌    | Refresh access token  |
| `/api/auth/logout`   | POST   | ✔️   | Revoke token / logout |

### **User Endpoints**

| Endpoint                | Method | Auth     | Description                |
| ----------------------- | ------ | -------- | -------------------------- |
| `/api/users/me`         | GET    | ✔️       | Get logged-in user profile |
| `/api/users`            | GET    | ✔️ Admin | List all users             |
| `/api/users/{id}/roles` | POST   | ✔️ Admin | Assign role to user        |

### **OpenIddict Endpoints**

* `/connect/token` – Token issuance
* `/connect/authorize` – Authorization code flow
* `/connect/logout` – Logout
* `/.well-known/openid-configuration` – Discovery endpoint
* `/.well-known/jwks` – Public signing keys

---

## **Integration with Other Microservices**

1. **Authentication:**

    * Client microservices send JWT token in `Authorization: Bearer <token>` header.
    * IdentityService validates the token via OpenIddict validation.

2. **Authorization:**

    * Microservices can use `[Authorize(Roles="Admin")]` or `[Authorize]` to enforce access.

3. **Token Refresh:**

    * When tokens expire, client microservices call `/api/auth/refresh` to obtain new tokens.

---

## **Global Response Format**

All endpoints return `ResponseDto<T>`:

```json
{
  "timestamp": "2025-11-23T13:00:00Z",
  "success": true,
  "message": "Operation successful",
  "data": {}
}
```

> Ensures consistent, predictable responses across all services.

---

## **Security**

* JWT Bearer Authentication
* Role-based Authorization
* Refresh tokens for long-lived sessions
* Global exception handling to hide sensitive server errors

---

## **Running in Docker**

```dockerfile
# Build image
docker build -t identity-service .

# Run container
docker run -p 5001:5001 --env-file .env identity-service
```

---

## **References**

* [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
* [OpenIddict Documentation](https://documentation.openiddict.com/)
* [PostgreSQL Documentation](https://www.postgresql.org/docs/)
* [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---
