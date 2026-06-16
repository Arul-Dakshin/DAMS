namespace DAMS.API.DTOs;

public record MonthlyRevenue(string Month, decimal Total);

public record DashboardStatsDto(
    int PatientCount,
    int DoctorCount,
    int TodaysAppointments,
    int TotalAppointments,
    int TotalBeds,
    int AvailableBeds,
    int OccupiedBeds,
    int AdmittedCount,
    int UnpaidInvoiceCount,
    decimal OutstandingAmount,
    decimal PaidRevenue,
    List<MonthlyRevenue> RevenueByMonth,
    int AppointmentsBooked,
    int AppointmentsCompleted,
    int AppointmentsCancelled,
    int AppointmentsNoShow);
