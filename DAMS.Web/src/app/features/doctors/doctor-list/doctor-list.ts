import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { DoctorService } from '../../../core/services/doctor.service';
import { AuthService } from '../../../core/services/auth.service';
import { Doctor } from '../../../core/models/doctor.model';

@Component({
  selector: 'app-doctor-list',
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './doctor-list.html'
})
export class DoctorList {
  private readonly docs = inject(DoctorService);
  private readonly auth = inject(AuthService);

  readonly doctors = signal<Doctor[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly isAdmin = this.auth.hasRole('Admin');
  readonly canEditSchedules = this.auth.hasRole('Admin', 'Doctor');

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.docs.getAll().subscribe({
      next: (d) => {
        this.doctors.set(d);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load doctors.');
        this.loading.set(false);
      }
    });
  }
}
