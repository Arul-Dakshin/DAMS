import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Bed, BedStatus, CreateWardPayload, Ward } from '../models/ward.model';

@Injectable({ providedIn: 'root' })
export class WardService {
  private readonly http = inject(HttpClient);
  private readonly api = `${environment.apiUrl}/wards`;

  getWards(): Observable<Ward[]> {
    return this.http.get<Ward[]>(this.api);
  }

  getAvailableBeds(): Observable<Bed[]> {
    return this.http.get<Bed[]>(`${this.api}/available-beds`);
  }

  createWard(payload: CreateWardPayload): Observable<Ward> {
    return this.http.post<Ward>(this.api, payload);
  }

  deleteWard(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }

  addBed(wardId: number, bedNumber: string): Observable<Bed> {
    return this.http.post<Bed>(`${this.api}/${wardId}/beds`, { bedNumber });
  }

  deleteBed(wardId: number, bedId: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${wardId}/beds/${bedId}`);
  }

  updateBedStatus(bedId: number, status: BedStatus): Observable<Bed> {
    return this.http.put<Bed>(`${this.api}/beds/${bedId}/status`, { status });
  }
}
