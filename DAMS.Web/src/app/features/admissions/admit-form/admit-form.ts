import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { WardService } from '../../../core/services/ward.service';
import { PatientService } from '../../../core/services/patient.service';
import { AdmissionService } from '../../../core/services/admission.service';
import { Ward } from '../../../core/models/ward.model';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-admit-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './admit-form.html'
})
export class AdmitForm {
  private readonly fb = inject(FormBuilder);
  private readonly wardService = inject(WardService);
  private readonly patientService = inject(PatientService);
  private readonly admissionService = inject(AdmissionService);
  private readonly router = inject(Router);

  readonly patients = signal<Patient[]>([]);
  readonly wards = signal<Ward[]>([]);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  // Flatten every available bed across wards into one labelled list.
  readonly availableBeds = computed(() =>
    this.wards().flatMap((w) =>
      w.beds
        .filter((b) => b.status === 'Available')
        .map((b) => ({ id: b.id, label: `${w.name} — ${b.bedNumber}` }))
    )
  );

  readonly form = this.fb.nonNullable.group({
    patientId: [0, [Validators.min(1)]],
    bedId: [0, [Validators.min(1)]],
    reason: ['']
  });

  constructor() {
    this.patientService.getAll().subscribe((p) => this.patients.set(p));
    this.wardService.getWards().subscribe((w) => this.wards.set(w));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();
    this.admissionService.admit({ patientId: v.patientId, bedId: v.bedId, reason: v.reason || null }).subscribe({
      next: () => this.router.navigate(['/admissions']),
      error: (e) => {
        this.error.set(e?.error?.message ?? 'Admission failed. Please try again.');
        this.saving.set(false);
      }
    });
  }
}
