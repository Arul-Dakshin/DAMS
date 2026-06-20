import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const PRESCRIPTION_ROUTES: Routes = [
  {
    path: 'prescriptions',
    canActivate: [roleGuard('Admin', 'Doctor', 'Patient')],
    loadComponent: () => import('./prescription-list/prescription-list').then((m) => m.PrescriptionList)
  },
  {
    path: 'prescriptions/new',
    canActivate: [roleGuard('Doctor')],
    loadComponent: () => import('./prescription-form/prescription-form').then((m) => m.PrescriptionForm)
  },
  {
    path: 'prescriptions/:id',
    canActivate: [roleGuard('Admin', 'Doctor', 'Patient')],
    loadComponent: () => import('./prescription-detail/prescription-detail').then((m) => m.PrescriptionDetail)
  }
];
