namespace DriveQuest.Models;

public class Rental
{
    public int RentalID { get; set; }
    public int CustomerID { get; set; }
    public int CarID { get; set; }
    public DateTime RentalDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int NumberOfDays { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Active";

    public Rental() { }

    public Rental(int id, int customerId, int carId, int days, decimal total)
    {
        RentalID = id;
        CustomerID = customerId;
        CarID = carId;
        RentalDate = DateTime.Now;
        NumberOfDays = days;
        TotalAmount = total;
    }
}
