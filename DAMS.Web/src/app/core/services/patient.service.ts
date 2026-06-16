import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Patient, PatientPayload } from '../models/patient.model';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/patients`;

  getAll(search?: string): Observable<Patient[]> {
    return this.http.get<Patient[]>(this.api, {
      params: search ? { search } : {}
    });
  }

  getById(id: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.api}/${id}`);
  }

  getMine(): Observable<Patient> {
    return this.http.get<Patient>(`${this.api}/me`);
  }

  create(payload: PatientPayload): Observable<Patient> {
    return this.http.post<Patient>(this.api, payload);
  }

  update(id: number, payload: PatientPayload): Observable<Patient> {
    return this.http.put<Patient>(`${this.api}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}
