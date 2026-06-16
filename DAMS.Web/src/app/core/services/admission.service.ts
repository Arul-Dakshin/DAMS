import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Admission, AdmissionStatus, AdmitPayload } from '../models/admission.model';

@Injectable({ providedIn: 'root' })
export class AdmissionService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/admissions`;

  getAll(status?: AdmissionStatus): Observable<Admission[]> {
    return this.http.get<Admission[]>(this.api, { params: status ? { status } : {} });
  }

  admit(payload: AdmitPayload): Observable<Admission> {
    return this.http.post<Admission>(this.api, payload);
  }

  discharge(id: number): Observable<Admission> {
    return this.http.put<Admission>(`${this.api}/${id}/discharge`, {});
  }
}
