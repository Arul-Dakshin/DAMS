import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { PatientService } from '../../../core/services/patient.service';
import { AuthService } from '../../../core/services/auth.service';
import { Patient } from '../../../core/models/patient.model';
import { EmptyState, LoadingSpinner, PageHeader } from '../../../shared/ui';

@Component({
  selector: 'app-patient-list',
  imports: [RouterLink, FormsModule, DatePipe, PageHeader, LoadingSpinner, EmptyState],
  templateUrl: './patient-list.html'
})
export class PatientList {
  private readonly patientService = inject(PatientService);
  private readonly auth = inject(AuthService);

  readonly patients = signal<Patient[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  search = '';

  readonly canManage = this.auth.hasRole('Admin', 'Receptionist');

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.patientService.getAll(this.search.trim() || undefined).subscribe({
      next: (data) => {
        this.patients.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load patients.');
        this.loading.set(false);
      }
    });
  }
}
