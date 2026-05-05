using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WreFigDemo.Identity;

namespace WreFigDemo.Pages.Account;

public class LogoutModel(SignInManager<AppUser> signInManager) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await signInManager.SignOutAsync();
        return LocalRedirect("/Account/Login");
    }
}
