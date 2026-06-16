namespace DAMS.Core.Enums;

public enum UserRole
{
    Admin,
    Doctor,
    Receptionist,
    Patient
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum AppointmentStatus
{
    Booked,
    Completed,
    Cancelled,
    NoShow
}

public enum AppointmentType
{
    OPD,
    IPD
}

public enum BedStatus
{
    Available,
    Occupied,
    Maintenance
}

public enum AdmissionStatus
{
    Admitted,
    Discharged
}

public enum InvoiceStatus
{
    Unpaid,
    Paid,
    Cancelled
}
