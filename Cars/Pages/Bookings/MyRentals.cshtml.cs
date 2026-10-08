using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Bookings;

public class MyRentalsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Phone { get; set; }
    public List<RentalItem> Items { get; private set; } = new();
    public bool Searched => !string.IsNullOrWhiteSpace(Phone);

    public void OnGet()
    {
        if (!Searched) return;
        List<Customer> customers = DataStore.CustomerList();
        Customer? customer = customers.FirstOrDefault(c => c.Phone.Equals(Phone!.Trim(), StringComparison.OrdinalIgnoreCase));
        if (customer == null) return;
        Items = DataStore.RentalList().Where(r => r.CustomerID == customer.CustomerID)
            .OrderByDescending(r => r.RentalID)
            .Select(r => new RentalItem(r, DataStore.FindCar(r.CarID)?.DisplayName ?? "Vehicle"))
            .ToList();
    }

    public record RentalItem(Rental Rental, string CarName);
}
