using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

public class CarsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public string Type { get; set; } = "All";
    [BindProperty(SupportsGet = true)] public string Status { get; set; } = "All";
    [BindProperty(SupportsGet = true)] public string Sort { get; set; } = "Newest";
    public List<Car> Vehicles { get; private set; } = new();
    public List<string> Categories { get; private set; } = new();
    public void OnGet() => Load();
    private void Load()
    {
        List<Car> all = DataStore.CarList();
        Categories = all.Select(c => c.Category).Distinct().OrderBy(x => x).ToList();
        IEnumerable<Car> query = all.Where(c =>
            (string.IsNullOrWhiteSpace(Search) || c.DisplayName.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase) || c.LicensePlate.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (Type == "All" || c.Category == Type) && (Status == "All" || c.Status == Status));
        Vehicles = (Sort switch
        {
            "RateAsc" => query.OrderBy(c => c.PricePerDay),
            "RateDesc" => query.OrderByDescending(c => c.PricePerDay),
            _ => query.OrderByDescending(c => c.CarID)
        }).ToList();
    }

    public IActionResult OnPostAdd(string brand, string model, string licensePlate, int year, string category, string transmission,
        int seats, decimal pricePerDay, string? description, int imageSlot)
        => Change(() => DataStore.AddCar(brand, model, licensePlate, year, category, transmission, seats, pricePerDay, description ?? "", imageSlot), "Vehicle added.");

    public IActionResult OnPostUpdate(int id, string brand, string model, string licensePlate, int year, string category, string transmission,
        int seats, decimal pricePerDay, string? description, int imageSlot)
        => Change(() => DataStore.UpdateCar(id, brand, model, licensePlate, year, category, transmission, seats, pricePerDay, description ?? "", imageSlot), "Vehicle updated.");

    public IActionResult OnPostDelete(int id) => Change(() => DataStore.DeleteCar(id), "Vehicle deleted.");
    public IActionResult OnPostMaintenance(int id, bool maintenance)
        => Change(() => DataStore.SetMaintenance(id, maintenance), maintenance ? "Car marked for maintenance." : "Car available again.");

    private IActionResult Change(Action action, string success)
    {
        try { action(); TempData["Success"] = success; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
