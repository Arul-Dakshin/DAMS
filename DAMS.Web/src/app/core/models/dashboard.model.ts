export interface MonthlyRevenue {
  month: string;
  total: number;
}

export interface DashboardStats {
  patientCount: number;
  doctorCount: number;
  todaysAppointments: number;
  totalAppointments: number;
  totalBeds: number;
  availableBeds: number;
  occupiedBeds: number;
  admittedCount: number;
  unpaidInvoiceCount: number;
  outstandingAmount: number;
  paidRevenue: number;
  revenueByMonth: MonthlyRevenue[];
  appointmentsBooked: number;
  appointmentsCompleted: number;
  appointmentsCancelled: number;
  appointmentsNoShow: number;
}
