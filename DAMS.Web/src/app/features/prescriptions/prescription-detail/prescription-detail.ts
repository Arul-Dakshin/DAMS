import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { Prescription } from '../../../core/models/prescription.model';

@Component({
  selector: 'app-prescription-detail',
  imports: [RouterLink, DatePipe],
  templateUrl: './prescription-detail.html'
})
export class PrescriptionDetail {
  private readonly service = inject(PrescriptionService);
  private readonly route = inject(ActivatedRoute);

  readonly prescription = signal<Prescription | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.service.getById(id).subscribe({
      next: (p) => {
        this.prescription.set(p);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load this prescription.');
        this.loading.set(false);
      }
    });
  }

  print(): void {
    window.print();
  }
}
