import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const APPOINTMENT_ROUTES: Routes = [
  {
    path: 'appointments',
    loadComponent: () => import('./appointment-list/appointment-list').then((m) => m.AppointmentList)
  },
  {
    path: 'appointments/book',
    canActivate: [roleGuard('Admin', 'Receptionist', 'Patient')],
    loadComponent: () => import('./appointment-booking/appointment-booking').then((m) => m.AppointmentBooking)
  }
];
