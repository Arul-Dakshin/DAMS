import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

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
    path: '',
    loadComponent: () => import('./shared/layout/shell').then((m) => m.Shell),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard)
      },
      {
        path: 'patients',
        canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
        loadComponent: () => import('./features/patients/patient-list/patient-list').then((m) => m.PatientList)
      },
      {
        path: 'patients/new',
        canActivate: [roleGuard('Admin', 'Receptionist')],
        loadComponent: () => import('./features/patients/patient-form/patient-form').then((m) => m.PatientForm)
      },
      {
        path: 'patients/:id/edit',
        canActivate: [roleGuard('Admin', 'Receptionist')],
        loadComponent: () => import('./features/patients/patient-form/patient-form').then((m) => m.PatientForm)
      },
      {
        path: 'my-profile',
        canActivate: [roleGuard('Patient')],
        loadComponent: () => import('./features/patients/my-profile/my-profile').then((m) => m.MyProfile)
      },
      {
        path: 'doctors',
        canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
        loadComponent: () => import('./features/doctors/doctor-list/doctor-list').then((m) => m.DoctorList)
      },
      {
        path: 'doctors/new',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./features/doctors/doctor-form/doctor-form').then((m) => m.DoctorForm)
      },
      {
        path: 'doctors/:id/edit',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./features/doctors/doctor-form/doctor-form').then((m) => m.DoctorForm)
      },
      {
        path: 'doctors/:id/schedules',
        canActivate: [roleGuard('Admin', 'Doctor')],
        loadComponent: () => import('./features/doctors/doctor-schedules/doctor-schedules').then((m) => m.DoctorSchedules)
      },
      {
        path: 'appointments',
        loadComponent: () => import('./features/appointments/appointment-list/appointment-list').then((m) => m.AppointmentList)
      },
      {
        path: 'appointments/book',
        canActivate: [roleGuard('Admin', 'Receptionist', 'Patient')],
        loadComponent: () => import('./features/appointments/appointment-booking/appointment-booking').then((m) => m.AppointmentBooking)
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' }
    ]
  },
  { path: '**', redirectTo: '' }
];
