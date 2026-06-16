export type AppointmentStatus = 'Booked' | 'Completed' | 'Cancelled' | 'NoShow';
export type AppointmentType = 'OPD' | 'IPD';

export interface Appointment {
  id: number;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  specialization: string;
  scheduledStart: string;
  scheduledEnd: string;
  status: AppointmentStatus;
  type: AppointmentType;
  reason?: string | null;
}

export interface BookAppointmentPayload {
  patientId?: number;
  doctorId: number;
  scheduledStart: string;
  type: AppointmentType;
  reason?: string | null;
}
