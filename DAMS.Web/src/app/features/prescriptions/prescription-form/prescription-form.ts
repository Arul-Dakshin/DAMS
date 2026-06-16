import { Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AppointmentService } from '../../../core/services/appointment.service';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { Appointment } from '../../../core/models/appointment.model';

@Component({
  selector: 'app-prescription-form',
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './prescription-form.html'
})
export class PrescriptionForm {
  private readonly fb = inject(FormBuilder);
  private readonly appointmentService = inject(AppointmentService);
  private readonly prescriptionService = inject(PrescriptionService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly appointments = signal<Appointment[]>([]);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    appointmentId: [0, [Validators.min(1)]],
    diagnosis: ['', [Validators.required, Validators.maxLength(1000)]],
    notes: [''],
    items: this.fb.array([this.newItem()])
  });

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  newItem() {
    return this.fb.nonNullable.group({
      medicineName: ['', [Validators.required]],
      dosage: ['', [Validators.required]],
      frequency: ['', [Validators.required]],
      durationDays: [5, [Validators.required, Validators.min(1)]],
      instructions: ['']
    });
  }

  addItem(): void {
    this.items.push(this.newItem());
  }

  removeItem(i: number): void {
    if (this.items.length > 1) this.items.removeAt(i);
  }

  constructor() {
    this.appointmentService.getAll().subscribe((a) => {
      this.appointments.set(a);
      const pre = Number(this.route.snapshot.queryParamMap.get('appointmentId'));
      if (pre) this.form.controls.appointmentId.setValue(pre);
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();
    this.prescriptionService
      .create({
        appointmentId: v.appointmentId,
        diagnosis: v.diagnosis,
        notes: v.notes || null,
        items: v.items.map((i) => ({ ...i, instructions: i.instructions || null }))
      })
      .subscribe({
        next: () => this.router.navigate(['/prescriptions']),
        error: (e) => {
          this.error.set(e?.error?.message ?? 'Could not save prescription.');
          this.saving.set(false);
        }
      });
  }
}
