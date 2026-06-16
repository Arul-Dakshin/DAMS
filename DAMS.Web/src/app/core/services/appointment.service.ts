import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Appointment, AppointmentStatus, BookAppointmentPayload } from '../models/appointment.model';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/appointments`;

  getAll(status?: AppointmentStatus): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(this.api, { params: status ? { status } : {} });
  }

  book(payload: BookAppointmentPayload): Observable<Appointment> {
    return this.http.post<Appointment>(this.api, payload);
  }

  updateStatus(id: number, status: AppointmentStatus): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.api}/${id}/status`, { status });
  }
}
