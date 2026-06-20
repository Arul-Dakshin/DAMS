# DAMS — Hospital Management System

A production-style, full-stack hospital management system built with **ASP.NET Core 10**,
**Angular 22**, and **Entity Framework Core**. It covers the real workflows of a hospital —
role-based access, patient registration, OPD/IPD admissions with bed management, doctor
scheduling & appointment booking, prescriptions, billing & invoicing, and an analytics
dashboard with live charts.

### 🔗 Live demo

| | URL |
|---|---|
| **Web app** | **https://doctorsappointmentmanagement.netlify.app** |
| **REST API (Swagger)** | **https://dams-api.onrender.com/swagger** |

> Use a one-click demo account on the login page (e.g. **Admin** — `admin@dams.com` / `Admin@123`).
> The API is on a free tier, so the **first request may take ~30–60s** to wake.

---

## Tech stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Angular 22 — standalone components, **signals**, reactive forms, lazy-loaded routes, Bootstrap 5, Chart.js (ng2-charts) |
| **Backend** | ASP.NET Core 10 Web API — layered controllers → services → EF Core, Swagger/OpenAPI |
| **Data** | Entity Framework Core 10 (Code-First, migrations), SQLite (provider-swappable to SQL Server / PostgreSQL) |
| **Auth** | JWT bearer tokens, BCrypt password hashing, role-based authorization (4 roles) |
| **DevOps** | Dockerised API on **Render**, SPA on **Netlify**, CI-on-push from GitHub |

## Features

- 🔐 **Auth & roles** — JWT login + patient self-registration; four roles (Admin / Doctor /
  Receptionist / Patient) enforced on **both** the API (`[Authorize(Roles=…)]`) and the Angular routes
- 🧑‍⚕️ **Patients** — searchable registration & management; patients see their own profile
- 👨‍⚕️ **Doctors & scheduling** — staff management and weekly availability
- 📅 **Appointments** — slot generation from schedules, booking, **double-booking prevention**, status workflow
- 🛏️ **OPD/IPD** — wards, beds, admit/discharge with **live bed availability**
- 💊 **Prescriptions** — diagnosis + medicine records tied to an appointment
- 🧾 **Billing** — invoices with line items, mark-as-paid, **printable receipts**, generate-from-visit
- 📊 **Dashboard** — KPI cards, bed-availability widget, **revenue & appointment-status charts**
- 📱 **Responsive** — desktop sidebar collapses to an off-canvas drawer on mobile

## Solution architecture

```
DAMS/
├─ DAMS.Core/    # Domain entities, enums, EF Core DbContext + migrations
├─ DAMS.API/     # Web API: controllers, DTOs, services, JWT auth, DB seeding
└─ DAMS.Web/     # Angular SPA
```

**Backend** follows a clean **controller → service → DbContext** layering. Controllers stay thin;
business logic (slot generation, double-booking checks, admit/discharge, invoice totals,
dashboard aggregation) lives in injectable services; DTOs isolate the API contract from entities.

**Frontend** is organised by feature, with a clear separation of concerns:

```
DAMS.Web/src/app/
├─ core/                     # app-wide singletons
│  ├─ models/                # typed interfaces (Patient, Appointment, …)
│  ├─ services/              # HTTP data services (one per domain)
│  ├─ guards/                # authGuard + roleGuard(...roles)
│  └─ interceptors/          # jwt.interceptor (attach token), error.interceptor (401 → login)
├─ features/                 # one folder per feature, each with its own *.routes.ts
│  ├─ auth/  patients/  doctors/  appointments/
│  ├─ beds/  admissions/  prescriptions/  invoices/  dashboard/
└─ shared/
   ├─ layout/                # app shell (responsive navbar + sidebar)
   └─ ui/                    # reusable presentational components
                            #   PageHeader · StatusBadge · LoadingSpinner · EmptyState
```

### Angular patterns used

- **Standalone components** throughout (no NgModules) with **signals** for state and `input()` APIs
- **Lazy loading** — every route uses `loadComponent`, and routes are split into **modular
  per-feature route files** (`features/*/*.routes.ts`) composed in `app.routes.ts`
- **Functional guards & interceptors** — `authGuard`, a parameterised `roleGuard('Admin', …)`,
  a JWT interceptor and a global 401-handling error interceptor
- **Reusable UI library** (`shared/ui`) — presentational components keep feature templates DRY
- **Reactive forms** with validation; **environment-based** API config with production file replacement

## Run it locally

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) and Node.js 20+ with
Angular CLI 22 (`npm i -g @angular/cli`). **No database server needed** — SQLite is file-based.

```powershell
# API  →  http://localhost:5100  (Swagger at /swagger)
cd DAMS.API
dotnet run            # creates, migrates & seeds dams.db automatically
```
```bash
# Web  →  http://localhost:4200
cd DAMS.Web
npm install
ng serve
```

### Seeded demo accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@dams.com` | `Admin@123` |
| Doctor | `doctor@dams.com` | `Doctor@123` |
| Receptionist | `reception@dams.com` | `Reception@123` |
| Patient | `patient@dams.com` | `Patient@123` |

## Deployment

- **API → Render** (Docker) — `Dockerfile` + `render.yaml` included; the app honours Render's
  `PORT`. Production CORS allows the deployed origin; override `Jwt__Key` for a real secret.
- **Web → Netlify** — `netlify.toml` builds `DAMS.Web` and publishes `dist/DAMS.Web/browser` with
  an SPA fallback. Both redeploy automatically on every push to `main`.

## Notes

- EF Core Code-First; migrations apply automatically on startup. Add one with
  `dotnet ef migrations add <Name> --project DAMS.Core --startup-project DAMS.API`.
- For a real deployment, set a strong `Jwt__Key`, restrict CORS to your domain, and swap the EF
  provider to SQL Server or PostgreSQL (single line in `Program.cs`).
