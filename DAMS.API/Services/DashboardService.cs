using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class DashboardService : IDashboardService
{
    private readonly DamsDbContext _db;

    public DashboardService(DamsDbContext db) => _db = db;

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var todayStart = DateTime.Now.Date;
        var todayEnd = todayStart.AddDays(1);

        var patientCount = await _db.Patients.CountAsync();
        var doctorCount = await _db.Doctors.CountAsync();
        var todaysAppointments = await _db.Appointments
            .CountAsync(a => a.ScheduledStart >= todayStart && a.ScheduledStart < todayEnd);
        var totalAppointments = await _db.Appointments.CountAsync();

        var totalBeds = await _db.Beds.CountAsync();
        var availableBeds = await _db.Beds.CountAsync(b => b.Status == BedStatus.Available);
        var occupiedBeds = await _db.Beds.CountAsync(b => b.Status == BedStatus.Occupied);
        var admittedCount = await _db.Admissions.CountAsync(a => a.Status == AdmissionStatus.Admitted);

        var unpaidInvoiceCount = await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Unpaid);
        var outstanding = await _db.InvoiceItems
            .Where(it => it.Invoice.Status == InvoiceStatus.Unpaid)
            .SumAsync(it => (decimal?)(it.Quantity * it.UnitPrice)) ?? 0m;
        var paidRevenue = await _db.InvoiceItems
            .Where(it => it.Invoice.Status == InvoiceStatus.Paid)
            .SumAsync(it => (decimal?)(it.Quantity * it.UnitPrice)) ?? 0m;

        var statusGroups = await _db.Appointments
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();
        int CountFor(AppointmentStatus s) => statusGroups.FirstOrDefault(x => x.Status == s)?.Count ?? 0;

        // Revenue for the last 6 calendar months, from paid invoices by payment date.
        var paidItems = await _db.InvoiceItems
            .Where(it => it.Invoice.Status == InvoiceStatus.Paid && it.Invoice.PaidDate != null)
            .Select(it => new { it.Invoice.PaidDate, Amount = it.Quantity * it.UnitPrice })
            .ToListAsync();

        var revenueByMonth = new List<MonthlyRevenue>();
        var thisMonth = new DateTime(todayStart.Year, todayStart.Month, 1);
        for (var m = thisMonth.AddMonths(-5); m <= thisMonth; m = m.AddMonths(1))
        {
            var total = paidItems
                .Where(p => p.PaidDate!.Value.Year == m.Year && p.PaidDate.Value.Month == m.Month)
                .Sum(p => p.Amount);
            revenueByMonth.Add(new MonthlyRevenue(m.ToString("MMM yyyy"), total));
        }

        return new DashboardStatsDto(
            patientCount, doctorCount, todaysAppointments, totalAppointments,
            totalBeds, availableBeds, occupiedBeds, admittedCount,
            unpaidInvoiceCount, outstanding, paidRevenue, revenueByMonth,
            CountFor(AppointmentStatus.Booked), CountFor(AppointmentStatus.Completed),
            CountFor(AppointmentStatus.Cancelled), CountFor(AppointmentStatus.NoShow));
    }
}
