import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { DoctorService } from '../../../core/services/doctor.service';
import { PatientService } from '../../../core/services/patient.service';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AuthService } from '../../../core/services/auth.service';
import { Doctor, Slot } from '../../../core/models/doctor.model';
import { Patient } from '../../../core/models/patient.model';
import { AppointmentType, BookAppointmentPayload } from '../../../core/models/appointment.model';

@Component({
  selector: 'app-appointment-booking',
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './appointment-booking.html'
})
export class AppointmentBooking {
  private readonly fb = inject(FormBuilder);
  private readonly doctorService = inject(DoctorService);
  private readonly patientService = inject(PatientService);
  private readonly appointmentService = inject(AppointmentService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly isPatient = this.auth.hasRole('Patient');
  readonly doctors = signal<Doctor[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly slots = signal<Slot[]>([]);
  readonly selectedSlot = signal<string | null>(null);
  readonly loadingSlots = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly today = new Date().toISOString().slice(0, 10);

  readonly form = this.fb.nonNullable.group({
    patientId: [0],
    doctorId: [0, [Validators.min(1)]],
    date: ['', Validators.required],
    type: ['OPD' as AppointmentType, Validators.required],
    reason: ['']
  });

  readonly canSubmit = computed(() => this.selectedSlot() !== null);

  constructor() {
    this.doctorService.getAll().subscribe((d) => this.doctors.set(d));
    if (!this.isPatient) {
      this.patientService.getAll().subscribe((p) => this.patients.set(p));
      this.form.controls.patientId.addValidators(Validators.min(1));
    }
  }

  onCriteriaChange(): void {
    this.selectedSlot.set(null);
    this.slots.set([]);
    this.error.set(null);
    const doctorId = this.form.controls.doctorId.value;
    const date = this.form.controls.date.value;
    if (doctorId > 0 && date) {
      this.loadingSlots.set(true);
      this.doctorService.getSlots(doctorId, date).subscribe({
        next: (s) => {
          this.slots.set(s);
          this.loadingSlots.set(false);
        },
        error: () => this.loadingSlots.set(false)
      });
    }
  }

  pickSlot(slot: Slot): void {
    this.selectedSlot.set(slot.start);
  }

  book(): void {
    if (this.form.invalid || !this.selectedSlot()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();
    const payload: BookAppointmentPayload = {
      doctorId: v.doctorId,
      scheduledStart: this.selectedSlot()!,
      type: v.type,
      reason: v.reason || null
    };
    if (!this.isPatient) {
      payload.patientId = v.patientId;
    }
    this.appointmentService.book(payload).subscribe({
      next: () => this.router.navigate(['/appointments']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Booking failed. Please try again.');
        this.saving.set(false);
      }
    });
  }
}
