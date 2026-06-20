import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { AuthService } from '../../../core/services/auth.service';
import { Prescription } from '../../../core/models/prescription.model';
import { EmptyState, LoadingSpinner, PageHeader } from '../../../shared/ui';

@Component({
  selector: 'app-prescription-list',
  imports: [RouterLink, DatePipe, PageHeader, LoadingSpinner, EmptyState],
  templateUrl: './prescription-list.html'
})
export class PrescriptionList {
  private readonly service = inject(PrescriptionService);
  private readonly auth = inject(AuthService);

  readonly prescriptions = signal<Prescription[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly isDoctor = this.auth.hasRole('Doctor');

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (p) => {
        this.prescriptions.set(p);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load prescriptions.');
        this.loading.set(false);
      }
    });
  }
}
