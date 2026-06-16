import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateInvoicePayload, Invoice } from '../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/invoices`;

  getAll(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.api);
  }

  getById(id: number): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.api}/${id}`);
  }

  create(payload: CreateInvoicePayload): Observable<Invoice> {
    return this.http.post<Invoice>(this.api, payload);
  }

  createFromAppointment(appointmentId: number): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.api}/from-appointment/${appointmentId}`, {});
  }

  markPaid(id: number): Observable<Invoice> {
    return this.http.put<Invoice>(`${this.api}/${id}/pay`, {});
  }
}
