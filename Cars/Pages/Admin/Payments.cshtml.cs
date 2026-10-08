using AUTODOK.Data;
using AUTODOK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

public class PaymentsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public string Status { get; set; } = "All";
    [BindProperty(SupportsGet = true)] public string Method { get; set; } = "All";
    [BindProperty(SupportsGet = true)] public int? SelectedId { get; set; }
    public List<PaymentRow> Rows { get; private set; } = new();
    public PaymentRow? Selected { get; private set; }
    public decimal Collected { get; private set; }
    public int PaidCount { get; private set; }
    public int PendingCount { get; private set; }
    public int RefundCount { get; private set; }

    public void OnGet()
    {
        List<Payment> payments = DataStore.PaymentList();
        List<Rental> rentals = DataStore.RentalList();
        Collected = payments.Where(p => p.Status == "Paid").Sum(p => p.Amount);
        PaidCount = payments.Count(p => p.Status == "Paid");
        PendingCount = payments.Count(p => p.Status == "Pending");
        RefundCount = payments.Count(p => p.Status == "Refunded");
        Rows = payments.Select(p =>
        {
            Rental rental = rentals.First(r => r.RentalID == p.RentalID);
            return new PaymentRow(p, rental, DataStore.FindCustomer(rental.CustomerID)?.FullName ?? "Unknown",
                DataStore.FindCar(rental.CarID)?.DisplayName ?? "Vehicle");
        })
        .Where(x => (Status == "All" || x.Payment.Status == Status) &&
            (Method == "All" || x.Payment.Method == Method) &&
            (string.IsNullOrWhiteSpace(Search) || x.Customer.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase) ||
             x.Payment.PaymentID.ToString().Contains(Search.Trim()) || x.Rental.RentalID.ToString().Contains(Search.Trim())))
        .OrderByDescending(x => x.Payment.PaymentID).ToList();
        Selected = Rows.FirstOrDefault(x => x.Payment.PaymentID == SelectedId) ?? Rows.FirstOrDefault();
    }

    public IActionResult OnPostRecord(int id)
    {
        try { DataStore.RecordPayment(id); TempData["Success"] = "Payment marked paid."; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage(new { selectedId = id });
    }
    public IActionResult OnPostRefund(int id)
    {
        try { DataStore.RefundPayment(id); TempData["Success"] = "Payment marked refunded (demo record only)."; }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { TempData["Error"] = ex.Message; }
        return RedirectToPage(new { selectedId = id });
    }
    public record PaymentRow(Payment Payment, Rental Rental, string Customer, string Car);
}
