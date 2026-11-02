using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ConfirmEmailModel(UserManager<IdentityUser> userManager) => _userManager = userManager;

    public bool Confirmado { get; set; }

    public async Task<IActionResult> OnGetAsync(string? userId, string? code)
    {
        if (userId == null || code == null)
            return LocalRedirect("~/");

        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario == null)
            return NotFound("No existe la cuenta que querés confirmar.");

        try
        {
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            Confirmado = (await _userManager.ConfirmEmailAsync(usuario, token)).Succeeded;
        }
        catch (FormatException)
        {
            Confirmado = false; // el enlace llegó cortado o modificado
        }

        return Page();
    }
}
