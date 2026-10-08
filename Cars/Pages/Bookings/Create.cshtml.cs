using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Bookings;

public class CreateModel : PageModel
{
    public Car Car { get; private set; } = new();
    [BindProperty] public BookingInput Input { get; set; } = new();

    public IActionResult OnGet(int carId)
    {
        Car? car = DataStore.FindCar(carId);
        if (car == null) return NotFound();
        Car = car;
        Input.CarId = carId;
        Input.PickupDate = DateTime.Today;
        Input.ReturnDate = DateTime.Today.AddDays(1);
        return Page();
    }

    public IActionResult OnPost()
    {
        Car? car = DataStore.FindCar(Input.CarId);
        if (car == null) return NotFound();
        Car = car;
        try
        {
            Rental rental = DataStore.BookCar(Input.CarId, Input.FullName, Input.Phone, Input.LicenseNumber,
                Input.PickupDate, Input.ReturnDate, Input.PickupLocation, Input.ReturnLocation,
                Input.Insurance, Input.PaymentMethod);
            return RedirectToPage("/Bookings/Confirmation", new { id = rental.RentalID });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public class BookingInput
    {
        public int CarId { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string PickupLocation { get; set; } = "Davao City";
        public string ReturnLocation { get; set; } = "Davao City";
        public bool Insurance { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
    }
}
