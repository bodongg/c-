namespace AUTODOK.Models;

public class Rental
{
    public int RentalID { get; set; }
    public int CustomerID { get; set; }
    public int CarID { get; set; }
    public DateTime RentalDate { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime PlannedReturnDate { get; set; }
    public string PickupLocation { get; set; } = "Davao City";
    public string ReturnLocation { get; set; } = "Davao City";
    public DateTime? ReturnDate { get; set; }
    public int NumberOfDays { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string Status { get; set; } = "Active";
}
