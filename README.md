# BudgetFlow 💰

A B2B SaaS platform for corporate budget management with multi-tenant architecture, expense approval workflows, and Stripe-powered subscription plans.

---

## Features

- **Multi-Tenant Architecture** — Each company gets a unique subdomain (`company.budgetflow.com`) with fully isolated data
- **Expense Approval Workflow** — Employee submits → Manager reviews → Finance approves → Budget auto-deducted
- **Budget Period Tracking** — Monthly budget allocation per department with automatic spent/remaining calculations and 80% threshold email alerts
- **Subscription Plans** — Free plan (2 departments, 5 users, USD only). Pro plan (unlimited + multi-currency + monthly PDF reports)
- **Stripe Integration** — Upgrade from Free to Pro via Stripe Checkout with webhook-based plan activation
- **Multi-Currency** — Submit expenses in any currency, auto-converted to USD via Frankfurter API (Pro only)
- **Monthly PDF Reports** — Auto-generated and emailed to TenantAdmin on the 1st of every month via Hangfire (Pro only)
- **Audit Logs** — Every critical action logged per tenant with old/new values and IP address
- **Role-Based Authorization** — TenantAdmin, Manager, Finance, Employee with enforced permissions at both Controller and Handler level

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 10 |
| Architecture | Clean Architecture + CQRS + MediatR |
| Database | SQL Server + EF Core 10 |
| Authentication | JWT Bearer Tokens |
| Validation | FluentValidation |
| Background Jobs | Hangfire |
| PDF Generation | QuestPDF |
| Email | MailKit |
| Currency Conversion | Frankfurter API |
| Payments | Stripe |
| Testing | xUnit + Moq + Shouldly |
| Containerization | Docker + Docker Compose |

---

## Architecture

```
BudgetFlow/
├── BudgetFlow.Domain/           # Entities, Enums, Base classes
├── BudgetFlow.Application/      # CQRS Handlers, Validators, Interfaces
│   └── Features/
│       ├── Auth/
│       ├── Users/
│       ├── Departments/
│       ├── Expenses/
│       ├── BudgetPeriods/
│       ├── Subscriptions/
│       └── AuditLogs/
├── BudgetFlow.Infrastructure/   # EF Core, JWT, Stripe, Email, PDF, Hangfire
└── BudgetFlow.API/              # Controllers, Middleware, Program.cs
```

---

## Multi-Tenancy

Each company registers with a unique subdomain:

```
acme.budgetflow.com      → Acme Corp  (TenantId: abc-123)
techco.budgetflow.com    → TechCo     (TenantId: xyz-456)
```

The `TenantResolutionMiddleware` extracts the subdomain from every request and attaches the `TenantId` to the HTTP context. Every handler filters data by `TenantId` — no tenant can access another tenant's data.

---

## Expense Approval Workflow

```
Employee  →  Submit Expense (any currency on Pro, USD only on Free)
                    ↓
Manager   →  Approve / Reject
                    ↓
Finance   →  Approve / Reject
                    ↓
          BudgetPeriod.SpentAmount += expense.AmountInBaseCurrency
                    ↓
          If SpentPercentage >= 80% → Email Alert to TenantAdmin
```

---

## Subscription Plans

| Feature | Free | Pro |
|---------|------|-----|
| Departments | Max 2 | Unlimited |
| Users | Max 5 | Unlimited |
| Currency | USD only | Multi-currency |
| Monthly PDF Reports | ❌ | ✅ |
| Expense Approval Workflow | ✅ | ✅ |
| Audit Logs | ✅ | ✅ |

---

## Getting Started

### Option 1 — Docker (Recommended)

```bash
# 1. Clone the repo
git clone https://github.com/Almoqdad-Gamal/BudgetFlow.git
cd BudgetFlow

# 2. Create env file
cp .env.example .env
# Open .env and add your Stripe keys

# 3. Run
docker-compose up --build
```

- API: `http://localhost:5261`
- Swagger: `http://localhost:5261/swagger`
- Hangfire Dashboard: `http://localhost:5261/hangfire`

### Option 2 — Local Development

```bash
# 1. Copy config
cp BudgetFlow.API/appsettings.example.json BudgetFlow.API/appsettings.json
# Update appsettings.json with your connection string and keys

# 2. Apply migrations
dotnet ef database update --project BudgetFlow.Infrastructure --startup-project BudgetFlow.API

# 3. Create Hangfire database in SSMS
# CREATE DATABASE BudgetFlowHangfire;

# 4. Run
dotnet run --project BudgetFlow.API
```

---

## API Endpoints

### Auth
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | ❌ | Register new tenant + admin |
| POST | `/api/auth/login` | ❌ | Login and get JWT token |

### Users
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/users` | TenantAdmin | Add user (Manager/Finance/Employee) |
| GET | `/api/users` | TenantAdmin | Get all tenant users |
| PATCH | `/api/users/{id}/deactivate` | TenantAdmin | Deactivate user |

### Departments
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/departments` | Authenticated | Create department |
| GET | `/api/departments` | Authenticated | Get all departments |
| PUT | `/api/departments/{id}` | Authenticated | Update department |
| DELETE | `/api/departments/{id}` | Authenticated | Delete department |

### Expenses
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/expenses` | Authenticated | Submit expense |
| GET | `/api/expenses` | Authenticated | Get expenses (paginated + filtered) |
| POST | `/api/expenses/{id}/review` | Manager, Finance, TenantAdmin | Approve or reject expense |

### Budget Periods
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/budgetperiods` | TenantAdmin, Finance | Create budget period |
| GET | `/api/budgetperiods` | Authenticated | Get budget periods (filtered) |
| PUT | `/api/budgetperiods/{id}` | TenantAdmin, Finance | Update allocated budget |

### Subscriptions
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/subscriptions/checkout` | TenantAdmin | Create Stripe checkout session |

### Audit Logs
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/auditlogs` | TenantAdmin | Get audit logs (paginated) |

---

## Testing the Application

### Full Test Flow

**1. Register a tenant**
```json
POST /api/auth/register
{
  "tenantName": "Acme Corp",
  "subdomain": "acme",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@acme.com",
  "password": "Password123!"
}
```

**2. Login**
```json
POST /api/auth/login
{
  "email": "john@acme.com",
  "password": "Password123!",
  "subdomain": "acme"
}
```

**3. Add a Manager**
```json
POST /api/users
Authorization: Bearer YOUR_TOKEN
{
  "firstName": "Ahmed",
  "lastName": "Ali",
  "email": "ahmed@acme.com",
  "password": "Password123!",
  "role": "Manager"
}
```

**4. Create a department**
```json
POST /api/departments
Authorization: Bearer YOUR_TOKEN
{
  "name": "Engineering",
  "budgetLimit": 10000,
  "currency": "USD"
}
```

**5. Create a budget period**
```json
POST /api/budgetperiods
Authorization: Bearer YOUR_TOKEN
{
  "departmentId": "DEPARTMENT_ID",
  "month": 5,
  "year": 2025,
  "allocatedBudget": 10000
}
```

**6. Submit an expense (as Manager)**
```json
POST /api/expenses
Authorization: Bearer MANAGER_TOKEN
{
  "title": "Office Supplies",
  "amount": 500,
  "currency": "USD",
  "departmentId": "DEPARTMENT_ID"
}
```

**7. Approve the expense (as Manager then Finance)**
```json
POST /api/expenses/{id}/review
Authorization: Bearer MANAGER_TOKEN
{
  "isApproved": true
}
```

**8. Upgrade to Pro**
```json
POST /api/subscriptions/checkout
Authorization: Bearer ADMIN_TOKEN
```
Open the returned `checkoutUrl` and pay with Stripe test card `4242 4242 4242 4242`

---

## Testing Stripe Locally

```bash
# 1. Install Stripe CLI
winget install Stripe.StripeCLI

# 2. Login
stripe login

# 3. Forward webhooks to local API
stripe listen --forward-to https://localhost:7235/api/webhook

# 4. Copy the webhook secret from CLI output
# Add it to appsettings.json → StripeSettings:WebhookSecret
```

### Stripe Test Cards

| Scenario | Card Number | Expiry | CVC |
|----------|-------------|--------|-----|
| Success | `4242 4242 4242 4242` | Any future | Any 3 digits |
| Declined | `4000 0000 0000 0002` | Any future | Any 3 digits |

---

## Environment Variables

| Variable | Description |
|----------|-------------|
| `JWT_SECRET_KEY` | Min 32 characters |
| `SA_PASSWORD` | SQL Server SA password |
| `STRIPE_SECRET_KEY` | From Stripe Dashboard → API Keys |
| `STRIPE_WEBHOOK_SECRET` | From Stripe CLI output (local) or Dashboard (production) |
| `STRIPE_PRO_PLAN_PRICE_ID` | From Stripe Dashboard → Products |

---

## Running Tests

```bash
dotnet test
```

7 unit tests covering Auth feature handlers (Register + Login).

---

## License

BudgetFlow.com