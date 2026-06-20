import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const INVOICE_ROUTES: Routes = [
  {
    path: 'invoices',
    canActivate: [roleGuard('Admin', 'Receptionist', 'Patient')],
    loadComponent: () => import('./invoice-list/invoice-list').then((m) => m.InvoiceList)
  },
  {
    path: 'invoices/new',
    canActivate: [roleGuard('Admin', 'Receptionist')],
    loadComponent: () => import('./invoice-form/invoice-form').then((m) => m.InvoiceForm)
  },
  {
    path: 'invoices/:id',
    canActivate: [roleGuard('Admin', 'Receptionist', 'Patient')],
    loadComponent: () => import('./invoice-detail/invoice-detail').then((m) => m.InvoiceDetail)
  }
];
