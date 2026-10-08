namespace DriveQuest.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string LicenseNumber { get; set; } = "";

    public Customer() { }

    public Customer(int id, string name, string phone, string license)
    {
        CustomerID = id;
        FullName = name;
        Phone = phone;
        LicenseNumber = license;
    }

    public override string ToString() => FullName;
}
