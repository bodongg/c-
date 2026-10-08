using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

public class CustomersModel : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    public List<Customer> People { get; private set; } = new();
    public void OnGet() => People = DataStore.CustomerList()
        .Where(c => string.IsNullOrWhiteSpace(Search) || c.FullName.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase) || c.Phone.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase))
        .OrderBy(c => c.CustomerID).ToList();
    public IActionResult OnPostAdd(string fullName, string phone, string licenseNumber)
        => Change(() => DataStore.AddCustomer(fullName, phone, licenseNumber), "Customer added.");
    public IActionResult OnPostUpdate(int id, string fullName, string phone, string licenseNumber)
        => Change(() => DataStore.UpdateCustomer(id, fullName, phone, licenseNumber), "Customer updated.");
    public IActionResult OnPostDelete(int id) => Change(() => DataStore.DeleteCustomer(id), "Customer deleted.");
    private IActionResult Change(Action action, string success)
    {
        try { action(); TempData["Success"] = success; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
