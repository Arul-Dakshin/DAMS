import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const PATIENT_ROUTES: Routes = [
  {
    path: 'patients',
    canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
    loadComponent: () => import('./patient-list/patient-list').then((m) => m.PatientList)
  },
  {
    path: 'patients/new',
    canActivate: [roleGuard('Admin', 'Receptionist')],
    loadComponent: () => import('./patient-form/patient-form').then((m) => m.PatientForm)
  },
  {
    path: 'patients/:id/edit',
    canActivate: [roleGuard('Admin', 'Receptionist')],
    loadComponent: () => import('./patient-form/patient-form').then((m) => m.PatientForm)
  },
  {
    path: 'my-profile',
    canActivate: [roleGuard('Patient')],
    loadComponent: () => import('./my-profile/my-profile').then((m) => m.MyProfile)
  }
];
