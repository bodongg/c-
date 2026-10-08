using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Cars;

public class DetailsModel : PageModel
{
    public Car Car { get; private set; } = new();
    public IActionResult OnGet(int id)
    {
        Car? car = DataStore.FindCar(id);
        if (car == null) return NotFound();
        Car = car;
        return Page();
    }
}
