using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class ResendEmailConfirmationModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;

    public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Enviado { get; set; }

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
        if (usuario != null && !await _userManager.IsEmailConfirmedAsync(usuario))
        {
            // Sin email real, la pantalla de confirmación ya muestra el enlace
            if (EmailSimulado.Activo(_emailSender))
                return RedirectToPage("./RegisterConfirmation", new { email = Input.Email });

            var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            var enlace = Url.Page("/Account/ConfirmEmail", pageHandler: null,
                values: new { area = "Identity", userId = usuario.Id, code = codigo }, protocol: Request.Scheme)!;

            await _emailSender.SendEmailAsync(Input.Email, "Confirmá tu cuenta de DocChain",
                $"Para activar tu cuenta, <a href='{HtmlEncoder.Default.Encode(enlace)}'>hacé clic acá</a>.");
        }

        // Misma respuesta exista o no la cuenta
        Enviado = true;
        return Page();
    }
}
