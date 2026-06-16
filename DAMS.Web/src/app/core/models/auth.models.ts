export type Role = 'Admin' | 'Doctor' | 'Receptionist' | 'Patient';

export interface UserInfo {
  id: number;
  fullName: string;
  email: string;
  role: Role;
}

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  user: UserInfo;
}
