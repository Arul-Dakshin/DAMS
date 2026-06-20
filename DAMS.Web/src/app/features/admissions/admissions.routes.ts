import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const ADMISSION_ROUTES: Routes = [
  {
    path: 'admissions',
    canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
    loadComponent: () => import('./admission-list/admission-list').then((m) => m.AdmissionList)
  },
  {
    path: 'admissions/admit',
    canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
    loadComponent: () => import('./admit-form/admit-form').then((m) => m.AdmitForm)
  }
];
