using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class PersonalDataModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public PersonalDataModel(UserManager<IdentityUser> userManager) => _userManager = userManager;

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        return usuario == null ? NotFound("No se pudo cargar tu cuenta.") : Page();
    }
}
