import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';
import { Gender, PatientPayload } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './patient-form.html'
})
export class PatientForm {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(PatientService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly id = signal<number | null>(null);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly isEdit = computed(() => this.id() !== null);
  readonly genders: Gender[] = ['Male', 'Female', 'Other'];

  readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    gender: ['Male' as Gender, [Validators.required]],
    dateOfBirth: ['', [Validators.required]],
    phone: ['', [Validators.required, Validators.maxLength(20)]],
    address: [''],
    bloodGroup: ['']
  });

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.id.set(id);
      this.loading.set(true);
      this.service.getById(id).subscribe({
        next: (p) => {
          this.form.patchValue({
            fullName: p.fullName,
            gender: p.gender,
            dateOfBirth: p.dateOfBirth,
            phone: p.phone,
            address: p.address ?? '',
            bloodGroup: p.bloodGroup ?? ''
          });
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load patient.');
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
    const payload: PatientPayload = {
      ...v,
      address: v.address || null,
      bloodGroup: v.bloodGroup || null
    };
    const req = this.isEdit()
      ? this.service.update(this.id()!, payload)
      : this.service.create(payload);
    req.subscribe({
      next: () => this.router.navigate(['/patients']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Save failed. Check the form and try again.');
        this.saving.set(false);
      }
    });
  }
}
