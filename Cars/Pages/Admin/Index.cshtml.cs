using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace AUTODOK.Pages.Admin;

public class IndexModel : PageModel
{
    public int TotalCars => DataStore.CarList().Count;
    public int AvailableCars => DataStore.AvailableCount;
    public int ActiveRentals => DataStore.ActiveRentalCount;
    public int TotalBookings => DataStore.TotalRentalCount;
    public int Customers => DataStore.CustomerCount;
    public decimal Revenue => DataStore.CompletedRevenue;
    public List<BookingRow> RecentBookings { get; private set; } = new();
    public List<BookingRow> UpcomingReturns { get; private set; } = new();
    public List<VehicleType> VehicleTypes { get; private set; } = new();
    public List<string> ChartDates { get; private set; } = new();
    public string ChartPoints { get; private set; } = "";
    public string DonutStyle { get; private set; } = "";

    public void OnGet()
    {
        List<Car> cars = DataStore.CarList();
        List<Customer> people = DataStore.CustomerList();
        List<Rental> rentals = DataStore.RentalList();
        List<BookingRow> rows = rentals.Select(r => new BookingRow(r,
            people.FirstOrDefault(c => c.CustomerID == r.CustomerID)?.FullName ?? "Unknown",
            cars.FirstOrDefault(c => c.CarID == r.CarID)?.DisplayName ?? "Vehicle",
            cars.FirstOrDefault(c => c.CarID == r.CarID)?.ImageSlot ?? 1,
            cars.FirstOrDefault(c => c.CarID == r.CarID)?.PhotoStyle ?? "")).ToList();
        RecentBookings = rows.OrderByDescending(x => x.Rental.RentalID).Take(5).ToList();
        UpcomingReturns = rows.Where(x => x.Rental.Status == "Active").OrderBy(x => x.Rental.PlannedReturnDate).Take(3).ToList();

        int[] counts = Enumerable.Range(0, 7).Select(i => rentals.Count(r => r.RentalDate.Date == DateTime.Today.AddDays(i - 6))).ToArray();
        int max = Math.Max(1, counts.Max());
        ChartPoints = string.Join(" ", counts.Select((count, i) => $"{40 + i * 86},{150 - count * 110 / max}"));
        ChartDates = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(i - 6).ToString("MMM d")).ToList();

        string[] colors = ["#f47b20", "#f0ad57", "#c85f37", "#7b6558", "#c68e72", "#a8432c"];
        VehicleTypes = cars.GroupBy(c => c.Category).OrderByDescending(g => g.Count())
            .Select((g, i) => new VehicleType(g.Key, g.Count(), cars.Count == 0 ? 0 : g.Count() * 100m / cars.Count, colors[i % colors.Length]))
            .ToList();
        decimal start = 0;
        List<string> pieces = new();
        foreach (VehicleType type in VehicleTypes)
        {
            decimal end = start + type.Percent;
            pieces.Add($"{type.Color} {start.ToString(CultureInfo.InvariantCulture)}% {end.ToString(CultureInfo.InvariantCulture)}%");
            start = end;
        }
        DonutStyle = pieces.Count == 0 ? "background:#453c36" : $"background:conic-gradient({string.Join(",", pieces)})";
    }

    public record BookingRow(Rental Rental, string Customer, string Car, int ImageSlot, string PhotoStyle);
    public record VehicleType(string Name, int Count, decimal Percent, string Color);
}
