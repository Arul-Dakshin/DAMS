export interface PrescriptionItem {
  id?: number;
  medicineName: string;
  dosage: string;
  frequency: string;
  durationDays: number;
  instructions?: string | null;
}

export interface Prescription {
  id: number;
  appointmentId: number;
  doctorId: number;
  doctorName: string;
  patientId: number;
  patientName: string;
  diagnosis: string;
  notes?: string | null;
  createdAt: string;
  appointmentDate: string;
  items: PrescriptionItem[];
}

export interface CreatePrescriptionPayload {
  appointmentId: number;
  diagnosis: string;
  notes?: string | null;
  items: PrescriptionItem[];
}
