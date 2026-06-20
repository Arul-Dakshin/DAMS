import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const BED_ROUTES: Routes = [
  {
    path: 'beds',
    canActivate: [roleGuard('Admin', 'Doctor', 'Receptionist')],
    loadComponent: () => import('./bed-availability/bed-availability').then((m) => m.BedAvailability)
  }
];
