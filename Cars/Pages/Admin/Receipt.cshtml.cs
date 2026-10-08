using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

public class ReceiptModel : PageModel
{
    public Payment Payment { get; private set; } = new();
    public Rental Rental { get; private set; } = new();
    public Customer Customer { get; private set; } = new();
    public Car Car { get; private set; } = new();
    public IActionResult OnGet(int id)
    {
        Payment? payment = DataStore.PaymentList().FirstOrDefault(p => p.PaymentID == id);
        if (payment == null) return NotFound();
        Payment = payment;
        Rental = DataStore.FindRental(payment.RentalID)!;
        Customer = DataStore.FindCustomer(Rental.CustomerID)!;
        Car = DataStore.FindCar(Rental.CarID)!;
        return Page();
    }
}
