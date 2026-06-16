namespace DAMS.API.Authorization;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Receptionist = "Receptionist";
    public const string Patient = "Patient";

    public const string Staff = Admin + "," + Doctor + "," + Receptionist;
    public const string FrontDesk = Admin + "," + Receptionist;
}
