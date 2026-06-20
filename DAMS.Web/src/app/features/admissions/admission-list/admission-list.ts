import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AdmissionService } from '../../../core/services/admission.service';
import { Admission } from '../../../core/models/admission.model';
import { EmptyState, LoadingSpinner, PageHeader, StatusBadge } from '../../../shared/ui';

@Component({
  selector: 'app-admission-list',
  imports: [RouterLink, DatePipe, PageHeader, LoadingSpinner, EmptyState, StatusBadge],
  templateUrl: './admission-list.html'
})
export class AdmissionList {
  private readonly service = inject(AdmissionService);

  readonly admissions = signal<Admission[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (a) => {
        this.admissions.set(a);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load admissions.');
        this.loading.set(false);
      }
    });
  }

  discharge(a: Admission): void {
    if (!confirm(`Discharge ${a.patientName} from ${a.wardName} (${a.bedNumber})?`)) return;
    this.service.discharge(a.id).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Discharge failed.')
    });
  }
}
