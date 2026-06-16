export type InvoiceStatus = 'Unpaid' | 'Paid' | 'Cancelled';

export interface InvoiceItem {
  id?: number;
  description: string;
  quantity: number;
  unitPrice: number;
  amount?: number;
}

export interface Invoice {
  id: number;
  invoiceNumber: string;
  patientId: number;
  patientName: string;
  appointmentId?: number | null;
  issuedDate: string;
  status: InvoiceStatus;
  paidDate?: string | null;
  notes?: string | null;
  total: number;
  items: InvoiceItem[];
}

export interface CreateInvoicePayload {
  patientId: number;
  notes?: string | null;
  items: { description: string; quantity: number; unitPrice: number }[];
}
