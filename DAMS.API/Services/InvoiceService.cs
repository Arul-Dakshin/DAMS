using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class InvoiceService : IInvoiceService
{
    private readonly DamsDbContext _db;

    public InvoiceService(DamsDbContext db) => _db = db;

    public async Task<List<InvoiceDto>> GetForUserAsync(string role, int userId)
    {
        var query = _db.Invoices.AsNoTracking().Include(i => i.Patient).Include(i => i.Items).AsQueryable();

        if (role == Roles.Patient)
        {
            var patientId = await _db.Patients.Where(p => p.UserId == userId).Select(p => (int?)p.Id).FirstOrDefaultAsync();
            query = query.Where(i => i.PatientId == patientId);
        }

        var invoices = await query.OrderByDescending(i => i.IssuedDate).ToListAsync();
        return invoices.Select(Map).ToList();
    }

    public async Task<InvoiceDto?> GetByIdAsync(int id, string role, int userId)
    {
        var invoice = await _db.Invoices.AsNoTracking()
            .Include(i => i.Patient).Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null) return null;

        if (role == Roles.Patient)
        {
            var patientId = await _db.Patients.Where(p => p.UserId == userId).Select(p => (int?)p.Id).FirstOrDefaultAsync();
            if (invoice.PatientId != patientId) return null;
        }

        return Map(invoice);
    }

    public async Task<InvoiceDto?> CreateAsync(CreateInvoiceRequest request)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == request.PatientId))
            return null;

        var invoice = new Invoice
        {
            PatientId = request.PatientId,
            IssuedDate = DateTime.UtcNow,
            Status = InvoiceStatus.Unpaid,
            Notes = request.Notes?.Trim(),
            Items = request.Items.Select(i => new InvoiceItem
            {
                Description = i.Description.Trim(),
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        return await GetRawAsync(invoice.Id);
    }

    public async Task<InvoiceDto?> CreateFromAppointmentAsync(int appointmentId)
    {
        var appointment = await _db.Appointments
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment is null) return null;

        var invoice = new Invoice
        {
            PatientId = appointment.PatientId,
            AppointmentId = appointment.Id,
            IssuedDate = DateTime.UtcNow,
            Status = InvoiceStatus.Unpaid,
            Items = new List<InvoiceItem>
            {
                new()
                {
                    Description = $"Consultation — {appointment.Doctor.FullName} ({appointment.Doctor.Specialization})",
                    Quantity = 1,
                    UnitPrice = appointment.Doctor.ConsultationFee
                }
            }
        };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        return await GetRawAsync(invoice.Id);
    }

    public async Task<InvoiceDto?> MarkPaidAsync(int id)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null || invoice.Status == InvoiceStatus.Paid) return null;

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetRawAsync(id);
    }

    private async Task<InvoiceDto?> GetRawAsync(int id)
    {
        var invoice = await _db.Invoices.AsNoTracking()
            .Include(i => i.Patient).Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);
        return invoice is null ? null : Map(invoice);
    }

    private static InvoiceDto Map(Invoice inv)
    {
        var items = inv.Items
            .Select(i => new InvoiceItemDto(i.Id, i.Description, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice))
            .ToList();
        return new InvoiceDto(
            inv.Id,
            $"INV-{inv.Id:D4}",
            inv.PatientId,
            inv.Patient.FullName,
            inv.AppointmentId,
            inv.IssuedDate,
            inv.Status,
            inv.PaidDate,
            inv.Notes,
            items.Sum(x => x.Amount),
            items);
    }
}
