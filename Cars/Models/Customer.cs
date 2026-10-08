namespace AUTODOK.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string LicenseNumber { get; set; } = "";
    public bool IsSample { get; set; }
}
