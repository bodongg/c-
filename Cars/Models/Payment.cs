namespace AUTODOK.Models;

public class Payment
{
    public int PaymentID { get; set; }
    public int RentalID { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Cash";
    public string Status { get; set; } = "Pending";
    public DateTime? PaidDate { get; set; }
}
