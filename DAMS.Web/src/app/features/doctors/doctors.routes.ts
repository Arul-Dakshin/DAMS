import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const DOCTOR_ROUTES: Routes = [
  {
    path: 'doctors',
    canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
    loadComponent: () => import('./doctor-list/doctor-list').then((m) => m.DoctorList)
  },
  {
    path: 'doctors/new',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./doctor-form/doctor-form').then((m) => m.DoctorForm)
  },
  {
    path: 'doctors/:id/edit',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./doctor-form/doctor-form').then((m) => m.DoctorForm)
  },
  {
    path: 'doctors/:id/schedules',
    canActivate: [roleGuard('Admin', 'Doctor')],
    loadComponent: () => import('./doctor-schedules/doctor-schedules').then((m) => m.DoctorSchedules)
  }
];
