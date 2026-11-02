using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _entorno;

    public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, IWebHostEnvironment entorno)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _entorno = entorno;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Ingresá tu email.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var usuario = await _userManager.FindByEmailAsync(Input.Email);

        // No revelamos si el email existe: la respuesta es siempre la misma
        if (usuario == null || !await _userManager.IsEmailConfirmedAsync(usuario))
            return RedirectToPage("./ForgotPasswordConfirmation");

        var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);
        codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
        var enlace = Url.Page("/Account/ResetPassword", pageHandler: null,
            values: new { area = "Identity", code = codigo }, protocol: Request.Scheme)!;

        await _emailSender.SendEmailAsync(Input.Email, "Restablecé tu contraseña de DocChain",
            $"Para elegir una contraseña nueva, <a href='{HtmlEncoder.Default.Encode(enlace)}'>hacé clic acá</a>.");

        // Solo en desarrollo y sin email real: mostramos el enlace para poder probar el flujo
        if (_entorno.IsDevelopment() && EmailSimulado.Activo(_emailSender))
            TempData["EnlaceRestablecer"] = enlace;

        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
