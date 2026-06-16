import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreatePrescriptionPayload, Prescription } from '../models/prescription.model';

@Injectable({ providedIn: 'root' })
export class PrescriptionService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/prescriptions`;

  getAll(): Observable<Prescription[]> {
    return this.http.get<Prescription[]>(this.api);
  }

  getById(id: number): Observable<Prescription> {
    return this.http.get<Prescription>(`${this.api}/${id}`);
  }

  create(payload: CreatePrescriptionPayload): Observable<Prescription> {
    return this.http.post<Prescription>(this.api, payload);
  }
}
