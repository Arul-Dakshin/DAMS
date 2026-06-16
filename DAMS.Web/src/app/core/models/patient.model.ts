export type Gender = 'Male' | 'Female' | 'Other';

export interface Patient {
  id: number;
  fullName: string;
  gender: Gender;
  dateOfBirth: string; // 'yyyy-MM-dd'
  phone: string;
  address?: string | null;
  bloodGroup?: string | null;
  userId?: number | null;
  createdAt: string;
}

export interface PatientPayload {
  fullName: string;
  gender: Gender;
  dateOfBirth: string;
  phone: string;
  address?: string | null;
  bloodGroup?: string | null;
}
