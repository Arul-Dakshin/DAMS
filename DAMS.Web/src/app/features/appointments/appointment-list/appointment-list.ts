import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AppointmentService } from '../../../core/services/appointment.service';
import { InvoiceService } from '../../../core/services/invoice.service';
import { AuthService } from '../../../core/services/auth.service';
import { Appointment, AppointmentStatus } from '../../../core/models/appointment.model';
import { EmptyState, LoadingSpinner, PageHeader, StatusBadge } from '../../../shared/ui';

@Component({
  selector: 'app-appointment-list',
  imports: [RouterLink, DatePipe, PageHeader, LoadingSpinner, EmptyState, StatusBadge],
  templateUrl: './appointment-list.html'
})
export class AppointmentList {
  private readonly service = inject(AppointmentService);
  private readonly invoiceService = inject(InvoiceService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly appointments = signal<Appointment[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly canBook = this.auth.hasRole('Admin', 'Receptionist', 'Patient');
  readonly canManage = this.auth.hasRole('Admin', 'Receptionist', 'Doctor');
  readonly isDoctor = this.auth.hasRole('Doctor');
  readonly canBill = this.auth.hasRole('Admin', 'Receptionist');

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (a) => {
        this.appointments.set(a);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load appointments.');
        this.loading.set(false);
      }
    });
  }

  setStatus(a: Appointment, status: AppointmentStatus): void {
    this.service.updateStatus(a.id, status).subscribe(() => this.load());
  }

  bill(a: Appointment): void {
    this.invoiceService.createFromAppointment(a.id).subscribe({
      next: (inv) => this.router.navigate(['/invoices', inv.id]),
      error: (e) => this.error.set(e?.error?.message ?? 'Could not create invoice.')
    });
  }
}
