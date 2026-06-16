import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Doctor, DoctorPayload, DoctorSchedule, SchedulePayload, Slot } from '../models/doctor.model';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/doctors`;

  getAll(search?: string): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(this.api, { params: search ? { search } : {} });
  }

  getById(id: number): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.api}/${id}`);
  }

  create(payload: DoctorPayload): Observable<Doctor> {
    return this.http.post<Doctor>(this.api, payload);
  }

  update(id: number, payload: DoctorPayload): Observable<Doctor> {
    return this.http.put<Doctor>(`${this.api}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }

  getSchedules(doctorId: number): Observable<DoctorSchedule[]> {
    return this.http.get<DoctorSchedule[]>(`${this.api}/${doctorId}/schedules`);
  }

  addSchedule(doctorId: number, payload: SchedulePayload): Observable<DoctorSchedule> {
    return this.http.post<DoctorSchedule>(`${this.api}/${doctorId}/schedules`, payload);
  }

  deleteSchedule(doctorId: number, scheduleId: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${doctorId}/schedules/${scheduleId}`);
  }

  getSlots(doctorId: number, date: string): Observable<Slot[]> {
    return this.http.get<Slot[]>(`${this.api}/${doctorId}/slots`, { params: { date } });
  }
}
