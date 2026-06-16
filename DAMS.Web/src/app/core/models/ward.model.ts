export type BedStatus = 'Available' | 'Occupied' | 'Maintenance';

export interface Bed {
  id: number;
  wardId: number;
  bedNumber: string;
  status: BedStatus;
}

export interface Ward {
  id: number;
  name: string;
  category?: string | null;
  totalBeds: number;
  availableBeds: number;
  beds: Bed[];
}

export interface CreateWardPayload {
  name: string;
  category?: string | null;
}
