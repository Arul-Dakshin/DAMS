import { Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { PatientService } from '../../../core/services/patient.service';
import { InvoiceService } from '../../../core/services/invoice.service';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-invoice-form',
  imports: [ReactiveFormsModule, RouterLink, CurrencyPipe],
  templateUrl: './invoice-form.html'
})
export class InvoiceForm {
  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);
  private readonly invoiceService = inject(InvoiceService);
  private readonly router = inject(Router);

  readonly patients = signal<Patient[]>([]);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    patientId: [0, [Validators.min(1)]],
    notes: [''],
    items: this.fb.array([this.newItem()])
  });

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  newItem() {
    return this.fb.nonNullable.group({
      description: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  addItem(): void {
    this.items.push(this.newItem());
  }

  removeItem(i: number): void {
    if (this.items.length > 1) this.items.removeAt(i);
  }

  lineAmount(i: number): number {
    const c = this.items.at(i);
    return (Number(c.get('quantity')?.value) || 0) * (Number(c.get('unitPrice')?.value) || 0);
  }

  total(): number {
    return this.items.controls.reduce(
      (sum, _, i) => sum + this.lineAmount(i),
      0
    );
  }

  constructor() {
    this.patientService.getAll().subscribe((p) => this.patients.set(p));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();
    this.invoiceService.create({ patientId: v.patientId, notes: v.notes || null, items: v.items }).subscribe({
      next: (inv) => this.router.navigate(['/invoices', inv.id]),
      error: (e) => {
        this.error.set(e?.error?.message ?? 'Could not create invoice.');
        this.saving.set(false);
      }
    });
  }
}
