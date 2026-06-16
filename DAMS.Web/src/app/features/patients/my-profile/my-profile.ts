import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { PatientService } from '../../../core/services/patient.service';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-my-profile',
  imports: [DatePipe],
  templateUrl: './my-profile.html'
})
export class MyProfile {
  private readonly service = inject(PatientService);

  readonly patient = signal<Patient | null>(null);
  readonly loading = signal(true);
  readonly notFound = signal(false);

  constructor() {
    this.service.getMine().subscribe({
      next: (p) => {
        this.patient.set(p);
        this.loading.set(false);
      },
      error: (err) => {
        this.notFound.set(err.status === 404);
        this.loading.set(false);
      }
    });
  }
}
