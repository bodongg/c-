namespace AUTODOK.Models;

public class Car
{
    public int CarID { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public int Year { get; set; }
    public string Category { get; set; } = "";
    public string Transmission { get; set; } = "Automatic";
    public int Seats { get; set; } = 5;
    public decimal PricePerDay { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Available";
    public int ImageSlot { get; set; } = 1;
    public bool IsSample { get; set; }
    public string DisplayName => $"{Brand} {Model}";
}
