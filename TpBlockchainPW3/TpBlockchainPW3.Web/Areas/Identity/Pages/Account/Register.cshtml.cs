using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
                         IEmailSender emailSender, ILogger<RegisterModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Ingresá tu email.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá una contraseña.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repetí la contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Repetí la contraseña")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return Page();

        // El email es también el nombre de usuario
        var usuario = new IdentityUser { UserName = Input.Email, Email = Input.Email };
        var resultado = await _userManager.CreateAsync(usuario, Input.Password);

        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        _logger.LogInformation("Se creó una cuenta nueva.");

        var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
        codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
        var enlace = Url.Page("/Account/ConfirmEmail", pageHandler: null,
            values: new { area = "Identity", userId = usuario.Id, code = codigo, returnUrl },
            protocol: Request.Scheme)!;

        await _emailSender.SendEmailAsync(Input.Email, "Confirmá tu cuenta de DocChain",
            $"Para activar tu cuenta, <a href='{HtmlEncoder.Default.Encode(enlace)}'>hacé clic acá</a>.");

        if (_userManager.Options.SignIn.RequireConfirmedAccount)
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });

        await _signInManager.SignInAsync(usuario, isPersistent: false);
        return LocalRedirect(returnUrl);
    }
}
