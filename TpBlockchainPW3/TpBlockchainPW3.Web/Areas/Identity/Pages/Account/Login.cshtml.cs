using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Ingresá tu email.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        // Los bool son obligatorios implícitamente: se define el mensaje para que no quede en inglés
        [Required(ErrorMessage = "Indicá si querés que te recordemos.")]
        [Display(Name = "Recordarme en este equipo")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);

        // Limpia una cookie de login externo que haya quedado a medias
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return Page();

        var resultado = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        if (resultado.Succeeded)
        {
            _logger.LogInformation("Un usuario inició sesión.");
            return LocalRedirect(returnUrl);
        }
        if (resultado.RequiresTwoFactor)
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
        if (resultado.IsLockedOut)
            return RedirectToPage("./Lockout");
        if (resultado.IsNotAllowed)
        {
            // Con RequireConfirmedAccount, una cuenta sin confirmar no puede entrar
            ModelState.AddModelError(string.Empty, "Tenés que confirmar tu cuenta antes de iniciar sesión.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "El email o la contraseña son incorrectos.");
        return Page();
    }
}
