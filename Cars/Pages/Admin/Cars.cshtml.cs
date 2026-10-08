using AUTODOK.Data;
using AUTODOK.Models;
using AUTODOK.Services;
using Microsoft.AspNetCore.Http;
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

    public Task<IActionResult> OnPostAddAsync(string brand, string model, string licensePlate, int year, string category, string transmission,
        int seats, decimal pricePerDay, string? description, IFormFile? photo)
        => ChangeAsync(async () =>
        {
            string imageDataUrl = (await VehiclePhoto.ReadAsync(photo, required: true))!;
            DataStore.AddCar(brand, model, licensePlate, year, category, transmission, seats, pricePerDay, description ?? "", imageDataUrl);
        }, "Vehicle added.");

    public Task<IActionResult> OnPostUpdateAsync(int id, string brand, string model, string licensePlate, int year, string category, string transmission,
        int seats, decimal pricePerDay, string? description, IFormFile? photo)
        => ChangeAsync(async () =>
        {
            string? imageDataUrl = await VehiclePhoto.ReadAsync(photo, required: false);
            DataStore.UpdateCar(id, brand, model, licensePlate, year, category, transmission, seats, pricePerDay, description ?? "", imageDataUrl);
        }, "Vehicle updated.");

    public IActionResult OnPostDelete(int id) => Change(() => DataStore.DeleteCar(id), "Vehicle deleted.");

    private IActionResult Change(Action action, string success)
    {
        try { action(); TempData["Success"] = success; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    private async Task<IActionResult> ChangeAsync(Func<Task> action, string success)
    {
        try { await action(); TempData["Success"] = success; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
