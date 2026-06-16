export type AdmissionStatus = 'Admitted' | 'Discharged';

export interface Admission {
  id: number;
  patientId: number;
  patientName: string;
  bedId: number;
  bedNumber: string;
  wardName: string;
  admitDate: string;
  dischargeDate?: string | null;
  status: AdmissionStatus;
  reason?: string | null;
}

export interface AdmitPayload {
  patientId: number;
  bedId: number;
  reason?: string | null;
}
