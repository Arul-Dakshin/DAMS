# DAMS — Hospital Management System

A full-stack hospital management system built with **ASP.NET Core 10**, **Angular 22**, and
**SQL Server**. It demonstrates role-based authentication, patient registration, doctor
scheduling, and appointment booking — with OPD/IPD, prescriptions, billing, and analytics
dashboards on the roadmap.

## Tech stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 10 Web API, Entity Framework Core 10 (Code-First) |
| Frontend | Angular 22 (standalone components, signals), Bootstrap 5, Chart.js |
| Database | SQL Server Express LocalDB |
| Auth | JWT bearer tokens, BCrypt password hashing, role-based authorization |

## Architecture

```
DAMS/
├─ DAMS.Core/    # Domain entities, enums, EF Core DbContext + migrations
├─ DAMS.API/     # Web API: controllers, DTOs, services, JWT auth, seeding
└─ DAMS.Web/     # Angular SPA: features, core (auth/guards/interceptors), shared layout
```

- **DAMS.Core** owns the data model (`User`, `Patient`, `Doctor`, `DoctorSchedule`,
  `Appointment`) and `DamsDbContext`.
- **DAMS.API** exposes REST endpoints, issues JWTs, enforces role policies, and seeds demo data.
- **DAMS.Web** is a standalone Angular app that calls the API; a JWT interceptor attaches the
  token and route guards enforce per-role access.

## Roles

`Admin` · `Doctor` · `Receptionist` · `Patient` — each sees a tailored dashboard, navigation,
and permission set (enforced on both the API and the Angular routes).

## Features

**Implemented (MVP)**
- 🔐 JWT login + patient self-registration, role-based routing
- 🧑‍⚕️ Patient registration & management (search, create, edit; patients view their own profile)
- 👨‍⚕️ Doctor management and weekly availability scheduling
- 📅 Appointment booking with generated time slots and double-booking prevention
- 📋 Role-scoped appointment lists and status workflow (Booked → Completed/Cancelled/No-show)

**Roadmap**
- 🛏️ OPD/IPD admissions with ward & bed availability
- 💊 Prescriptions & diagnosis records
- 🧾 Billing & invoice generation
- 📊 Dashboards: bed availability, patient stats, revenue charts

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — `winget install Microsoft.DotNet.SDK.10`
- SQL Server Express **LocalDB** (instance `(localdb)\MSSQLLocalDB`)
- Node.js 20+ and Angular CLI 22 — `npm install -g @angular/cli`
- EF Core CLI — `dotnet tool install --global dotnet-ef`

## Running locally

**Backend** (http://localhost:5100, Swagger at `/swagger`)
```powershell
cd DAMS.API
dotnet run
```
The database is created and seeded automatically on first run (migrations are applied at
startup).

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

The seeded doctor (Dr. Alice Carter, General Medicine) has a Mon–Fri 09:00–13:00 schedule with
30-minute slots, and one patient (John Miller) is pre-registered.

## Key API endpoints

| Method | Route | Roles |
|--------|-------|-------|
| POST | `/api/auth/login`, `/api/auth/register` | Public |
| GET/POST/PUT | `/api/patients` | Staff (create/edit: Admin, Receptionist) |
| GET | `/api/patients/me` | Patient |
| GET/POST/PUT/DELETE | `/api/doctors` | View: all · Manage: Admin |
| GET/POST/DELETE | `/api/doctors/{id}/schedules` | Admin, Doctor |
| GET | `/api/doctors/{id}/slots?date=yyyy-MM-dd` | Authenticated |
| GET/POST | `/api/appointments` | Book: Admin, Receptionist, Patient |
| PUT | `/api/appointments/{id}/status` | Staff |

## Database

EF Core Code-First. Inspect the data in SSMS by connecting to `(localdb)\MSSQLLocalDB` and
opening the `DamsDb` database. To add a migration after model changes:

```powershell
dotnet ef migrations add <Name> --project DAMS.Core --startup-project DAMS.API
dotnet ef database update --project DAMS.Core --startup-project DAMS.API
```
