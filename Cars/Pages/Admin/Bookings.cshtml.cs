using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

public class BookingsModel : PageModel
{
    public List<Car> Vehicles { get; private set; } = new();
    public List<Customer> People { get; private set; } = new();
    [BindProperty] public BookingInput Input { get; set; } = new();

    public void OnGet(int? carId)
    {
        Load();
        Input.CarId = carId ?? Vehicles.FirstOrDefault()?.CarID ?? 0;
        Input.PickupDate = DateTime.Today;
        Input.ReturnDate = DateTime.Today.AddDays(1);
    }
    public IActionResult OnPost()
    {
        Load();
        try
        {
            Rental rental = DataStore.BookCar(Input.CarId, Input.FullName, Input.Phone, Input.LicenseNumber,
                Input.PickupDate, Input.ReturnDate, Input.PickupLocation, Input.ReturnLocation,
                Input.Insurance, Input.PaymentMethod);
            TempData["Success"] = $"Booking AD-{rental.RentalID:D6} confirmed.";
            return RedirectToPage("/Admin/Reservations", new { selectedId = rental.RentalID });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
    private void Load()
    {
        Vehicles = DataStore.CarList().Where(c => c.Status == "Available").ToList();
        People = DataStore.CustomerList();
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
