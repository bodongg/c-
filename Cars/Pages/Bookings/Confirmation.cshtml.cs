using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Bookings;

public class ConfirmationModel : PageModel
{
    public Rental Rental { get; private set; } = new();
    public Car Car { get; private set; } = new();
    public Customer Customer { get; private set; } = new();

    public IActionResult OnGet(int id)
    {
        Rental? rental = DataStore.FindRental(id);
        if (rental == null) return NotFound();
        Rental = rental;
        Car = DataStore.FindCar(rental.CarID)!;
        Customer = DataStore.FindCustomer(rental.CustomerID)!;
        return Page();
    }
}
