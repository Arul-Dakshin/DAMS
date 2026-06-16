import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AuthService } from '../../../core/services/auth.service';
import { Appointment, AppointmentStatus } from '../../../core/models/appointment.model';

@Component({
  selector: 'app-appointment-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './appointment-list.html'
})
export class AppointmentList {
  private readonly service = inject(AppointmentService);
  private readonly auth = inject(AuthService);

  readonly appointments = signal<Appointment[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly canBook = this.auth.hasRole('Admin', 'Receptionist', 'Patient');
  readonly canManage = this.auth.hasRole('Admin', 'Receptionist', 'Doctor');

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

  badgeClass(status: AppointmentStatus): string {
    const map: Record<AppointmentStatus, string> = {
      Booked: 'bg-primary',
      Completed: 'bg-success',
      Cancelled: 'bg-secondary',
      NoShow: 'bg-danger'
    };
    return map[status];
  }
}
