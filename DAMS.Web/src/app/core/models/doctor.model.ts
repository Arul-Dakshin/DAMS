export type DayOfWeek =
  | 'Sunday' | 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday';

export interface Doctor {
  id: number;
  fullName: string;
  specialization: string;
  phone?: string | null;
  consultationFee: number;
  userId?: number | null;
}

export interface DoctorPayload {
  fullName: string;
  specialization: string;
  phone?: string | null;
  consultationFee: number;
}

export interface DoctorSchedule {
  id: number;
  doctorId: number;
  dayOfWeek: DayOfWeek;
  startTime: string; // 'HH:mm:ss'
  endTime: string;
  slotMinutes: number;
}

export interface SchedulePayload {
  dayOfWeek: DayOfWeek;
  startTime: string; // 'HH:mm:ss'
  endTime: string;
  slotMinutes: number;
}

export interface Slot {
  start: string; // ISO datetime
  end: string;
}
