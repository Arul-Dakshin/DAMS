import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DoctorService } from '../../../core/services/doctor.service';
import { DayOfWeek, Doctor, DoctorSchedule, SchedulePayload } from '../../../core/models/doctor.model';

@Component({
  selector: 'app-doctor-schedules',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './doctor-schedules.html'
})
export class DoctorSchedules {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(DoctorService);
  private readonly route = inject(ActivatedRoute);

  readonly doctorId = Number(this.route.snapshot.paramMap.get('id'));
  readonly doctor = signal<Doctor | null>(null);
  readonly schedules = signal<DoctorSchedule[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly days: DayOfWeek[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  readonly form = this.fb.nonNullable.group({
    dayOfWeek: ['Monday' as DayOfWeek, Validators.required],
    startTime: ['09:00', Validators.required],
    endTime: ['13:00', Validators.required],
    slotMinutes: [30, [Validators.required, Validators.min(5)]]
  });

  constructor() {
    this.service.getById(this.doctorId).subscribe((d) => this.doctor.set(d));
    this.loadSchedules();
  }

  loadSchedules(): void {
    this.loading.set(true);
    this.service.getSchedules(this.doctorId).subscribe({
      next: (s) => {
        this.schedules.set(s);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  add(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error.set(null);
    const v = this.form.getRawValue();
    const payload: SchedulePayload = {
      dayOfWeek: v.dayOfWeek,
      startTime: this.toHms(v.startTime),
      endTime: this.toHms(v.endTime),
      slotMinutes: v.slotMinutes
    };
    this.service.addSchedule(this.doctorId, payload).subscribe({
      next: () => this.loadSchedules(),
      error: (err) => this.error.set(err?.error?.message ?? 'Could not add schedule.')
    });
  }

  remove(id: number): void {
    this.service.deleteSchedule(this.doctorId, id).subscribe(() => this.loadSchedules());
  }

  // 'HH:mm' -> 'HH:mm:ss' (TimeOnly JSON binding expects seconds)
  private toHms(t: string): string {
    return t.length === 5 ? `${t}:00` : t;
  }
}
