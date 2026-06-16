# DAMS — Hospital Management System

A full-stack hospital management system built with **ASP.NET Core 10**, **Angular 22**, and
**Entity Framework Core**. It features role-based authentication, patient registration, OPD/IPD
admissions with bed management, doctor scheduling & appointment booking, prescriptions &
diagnosis records, billing & invoicing, and an analytics dashboard with live charts.

> **Runs with zero database setup** — uses a file-based SQLite database that is created and
> seeded automatically on first run.

## Tech stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 10 Web API, Entity Framework Core 10 (Code-First) |
| Frontend | Angular 22 (standalone components, signals), Bootstrap 5, Chart.js (ng2-charts) |
| Database | SQLite (portable; swap the EF provider for SQL Server / PostgreSQL in production) |
| Auth | JWT bearer tokens, BCrypt password hashing, role-based authorization |

## Architecture

```
DAMS/
├─ DAMS.Core/    # Domain entities, enums, EF Core DbContext + migrations
├─ DAMS.API/     # Web API: controllers, DTOs, services, JWT auth, seeding
└─ DAMS.Web/     # Angular SPA: features, core (auth/guards/interceptors), shared layout
```

- **DAMS.Core** owns the data model and `DamsDbContext`.
- **DAMS.API** exposes REST endpoints, issues JWTs, enforces role policies, and seeds demo data.
- **DAMS.Web** is a standalone Angular app; a JWT interceptor attaches the token and route
  guards enforce per-role access.

## Features

- 🔐 **Auth & roles** — JWT login, patient self-registration, four roles (Admin / Doctor /
  Receptionist / Patient) enforced on both API and UI
- 🧑‍⚕️ **Patients** — registration & management; patients view their own profile
- 👨‍⚕️ **Doctors & scheduling** — staff management and weekly availability
- 📅 **Appointments** — slot generation, booking, double-booking prevention, status workflow
- 🛏️ **OPD/IPD** — wards, beds, admit/discharge with live bed availability
- 💊 **Prescriptions** — diagnosis + medicine records per appointment
- 🧾 **Billing** — invoices with line items, mark-as-paid, printable receipts, generate-from-visit
- 📊 **Dashboard** — KPI cards, bed-availability widget, revenue & appointment-status charts

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — `winget install Microsoft.DotNet.SDK.10`
- Node.js 20+ and Angular CLI 22 — `npm install -g @angular/cli`

*(No database server required — SQLite is file-based.)*

## Running locally

**Backend** (http://localhost:5100, Swagger at `/swagger`)
```powershell
cd DAMS.API
dotnet run
```
The SQLite database (`dams.db`) is created, migrated, and seeded automatically on first run.

**Frontend** (http://localhost:4200)
```bash
cd DAMS.Web
npm install
ng serve
```

## Seeded demo accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@dams.com` | `Admin@123` |
| Doctor | `doctor@dams.com` | `Doctor@123` |
| Receptionist | `reception@dams.com` | `Reception@123` |
| Patient | `patient@dams.com` | `Patient@123` |

The login page has one-click buttons to fill these in. A seeded doctor (Dr. Alice Carter) has a
Mon–Fri schedule, one patient is pre-registered, and two wards with beds are set up.

## Deployment (free)

- **Frontend → Netlify** — `netlify.toml` is included (base `DAMS.Web`, publish
  `dist/DAMS.Web/browser`, SPA redirect). Set `environment.prod.ts` `apiUrl` to your API URL.
- **API → Render** — a `Dockerfile` and `render.yaml` blueprint are included; the app honors
  Render's `PORT`. Optionally set `Jwt__Key` (strong secret) and `Cors__AllowedOrigins__0`
  (your Netlify URL) as environment variables.

## Notes

- EF Core Code-First; migrations apply automatically at startup. Add a migration with:
  ```powershell
  dotnet ef migrations add <Name> --project DAMS.Core --startup-project DAMS.API
  ```
- For production, override the JWT signing key and restrict CORS via environment variables, and
  consider switching the EF provider to SQL Server or PostgreSQL.
