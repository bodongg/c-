using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages;

public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public string? Category { get; set; }
    [BindProperty(SupportsGet = true)] public decimal? MaxPrice { get; set; }
    [BindProperty(SupportsGet = true)] public string? Transmission { get; set; }
    [BindProperty(SupportsGet = true)] public int? MinimumSeats { get; set; }
    [BindProperty(SupportsGet = true)] public bool AvailableOnly { get; set; }
    public List<Car> Vehicles { get; private set; } = new();
    public List<string> Categories { get; private set; } = new();

    public void OnGet()
    {
        List<Car> all = DataStore.CarList();
        Categories = all.Select(c => c.Category).Distinct().OrderBy(x => x).ToList();
        Vehicles = all.Where(c =>
            (string.IsNullOrWhiteSpace(Search) || c.DisplayName.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(Category) || c.Category.Equals(Category, StringComparison.OrdinalIgnoreCase)) &&
            (!MaxPrice.HasValue || c.PricePerDay <= MaxPrice.Value) &&
            (string.IsNullOrWhiteSpace(Transmission) || c.Transmission.Equals(Transmission, StringComparison.OrdinalIgnoreCase)) &&
            (!MinimumSeats.HasValue || c.Seats >= MinimumSeats.Value) &&
            (!AvailableOnly || c.Status == "Available"))
            .ToList();
    }
}
