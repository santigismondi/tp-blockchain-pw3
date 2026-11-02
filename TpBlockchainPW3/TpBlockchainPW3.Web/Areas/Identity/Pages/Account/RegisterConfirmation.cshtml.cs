using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class RegisterConfirmationModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;

    public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public string Email { get; set; } = string.Empty;

    // Sin servicio de email real, el enlace de confirmación se muestra en pantalla
    public bool DisplayConfirmAccountLink { get; set; }

    public string? EmailConfirmationUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string? email, string? returnUrl = null)
    {
        if (email == null)
            return LocalRedirect("~/");

        var usuario = await _userManager.FindByEmailAsync(email);
        if (usuario == null)
            return NotFound("No existe una cuenta con ese email.");

        Email = email;
        DisplayConfirmAccountLink = EmailSimulado.Activo(_emailSender);

        if (DisplayConfirmAccountLink)
        {
            var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            EmailConfirmationUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null,
                values: new { area = "Identity", userId = usuario.Id, code = codigo, returnUrl },
                protocol: Request.Scheme);
        }

        return Page();
    }
}
