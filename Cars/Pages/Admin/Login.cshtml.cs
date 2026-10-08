using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AUTODOK.Pages.Admin;

[AllowAnonymous]
public class LoginModel : PageModel
{
    [BindProperty] public string Username { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public bool RememberMe { get; set; }

    public IActionResult OnGet() => User.Identity?.IsAuthenticated == true ? RedirectToPage("/Admin/Index") : Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (Username != "admin" || Password != "admin123")
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }
        ClaimsIdentity identity = new(new[] { new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, "Admin") }, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = RememberMe, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(RememberMe ? 24 : 8) });
        return RedirectToPage("/Admin/Index");
    }
}
