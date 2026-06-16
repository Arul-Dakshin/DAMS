import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DoctorService } from '../../../core/services/doctor.service';
import { DoctorPayload } from '../../../core/models/doctor.model';

@Component({
  selector: 'app-doctor-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './doctor-form.html'
})
export class DoctorForm {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(DoctorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly id = signal<number | null>(null);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly isEdit = computed(() => this.id() !== null);

  readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    specialization: ['', [Validators.required, Validators.maxLength(100)]],
    phone: [''],
    consultationFee: [0, [Validators.required, Validators.min(0)]]
  });

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.id.set(id);
      this.loading.set(true);
      this.service.getById(id).subscribe({
        next: (d) => {
          this.form.patchValue({
            fullName: d.fullName,
            specialization: d.specialization,
            phone: d.phone ?? '',
            consultationFee: d.consultationFee
          });
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load doctor.');
          this.loading.set(false);
        }
      });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();
    const payload: DoctorPayload = { ...v, phone: v.phone || null };
    const req = this.isEdit()
      ? this.service.update(this.id()!, payload)
      : this.service.create(payload);
    req.subscribe({
      next: () => this.router.navigate(['/doctors']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Save failed.');
        this.saving.set(false);
      }
    });
  }
}
