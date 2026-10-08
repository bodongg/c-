using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

public class ReservationsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public string Status { get; set; } = "All";
    [BindProperty(SupportsGet = true)] public string Category { get; set; } = "All";
    [BindProperty(SupportsGet = true)] public int? SelectedId { get; set; }
    public List<ReservationRow> Rows { get; private set; } = new();
    public ReservationRow? Selected { get; private set; }
    public List<string> Categories { get; private set; } = new();

    public void OnGet()
    {
        List<Car> cars = DataStore.CarList();
        List<Customer> customers = DataStore.CustomerList();
        Categories = cars.Select(c => c.Category).Distinct().OrderBy(x => x).ToList();
        Rows = DataStore.RentalList().Select(r => new ReservationRow(r,
            customers.FirstOrDefault(c => c.CustomerID == r.CustomerID) ?? new Customer { FullName = "Unknown" },
            cars.FirstOrDefault(c => c.CarID == r.CarID) ?? new Car { Brand = "Unknown" }))
            .Where(x => (Status == "All" || x.Rental.Status == Status) &&
                (Category == "All" || x.Car.Category == Category) &&
                (string.IsNullOrWhiteSpace(Search) || x.Customer.FullName.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase) ||
                 x.Car.DisplayName.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase) || x.Rental.RentalID.ToString().Contains(Search.Trim())))
            .OrderByDescending(x => x.Rental.RentalID).ToList();
        Selected = Rows.FirstOrDefault(x => x.Rental.RentalID == SelectedId) ?? Rows.FirstOrDefault();
    }

    public IActionResult OnPostReturn(int id)
    {
        try { DataStore.ReturnCar(id); TempData["Success"] = "Car returned. It is available again."; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage(new { selectedId = id });
    }
    public IActionResult OnPostCancel(int id)
    {
        try { DataStore.CancelRental(id); TempData["Success"] = "Reservation cancelled. The car is available again."; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage(new { selectedId = id });
    }
    public record ReservationRow(Rental Rental, Customer Customer, Car Car);
}
