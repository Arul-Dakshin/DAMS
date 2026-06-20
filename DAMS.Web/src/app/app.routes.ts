import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { PATIENT_ROUTES } from './features/patients/patients.routes';
import { DOCTOR_ROUTES } from './features/doctors/doctors.routes';
import { APPOINTMENT_ROUTES } from './features/appointments/appointments.routes';
import { BED_ROUTES } from './features/beds/beds.routes';
import { ADMISSION_ROUTES } from './features/admissions/admissions.routes';
import { PRESCRIPTION_ROUTES } from './features/prescriptions/prescriptions.routes';
import { INVOICE_ROUTES } from './features/invoices/invoices.routes';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register)
  },
  {
    // Authenticated area — everything below renders inside the app shell.
    path: '',
    loadComponent: () => import('./shared/layout/shell').then((m) => m.Shell),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard)
      },
      ...PATIENT_ROUTES,
      ...DOCTOR_ROUTES,
      ...APPOINTMENT_ROUTES,
      ...BED_ROUTES,
      ...ADMISSION_ROUTES,
      ...PRESCRIPTION_ROUTES,
      ...INVOICE_ROUTES,
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' }
    ]
  },
  { path: '**', redirectTo: '' }
];
