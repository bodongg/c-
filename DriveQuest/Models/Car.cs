namespace DriveQuest.Models;

public class Car
{
    public int CarID { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public string Category { get; set; } = "";
    public decimal PricePerDay { get; set; }
    public string Status { get; set; } = "Available";

    public Car() { }

    public Car(int id, string brand, string model, int year, string category, decimal price)
    {
        CarID = id;
        Brand = brand;
        Model = model;
        Year = year;
        Category = category;
        PricePerDay = price;
    }

    public override string ToString() => $"{Brand} {Model} (₱{PricePerDay:N0}/day)";
}
