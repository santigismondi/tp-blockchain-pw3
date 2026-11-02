using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ResetPasswordModel(UserManager<IdentityUser> userManager) => _userManager = userManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Ingresá tu email.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá una contraseña.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña nueva")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repetí la contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Repetí la contraseña")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Falta el código para restablecer la contraseña.")]
        public string Code { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? code = null)
    {
        if (code == null)
            return BadRequest("Falta el código para restablecer la contraseña.");

        try
        {
            Input = new InputModel { Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)) };
        }
        catch (FormatException)
        {
            return BadRequest("El enlace para restablecer la contraseña no es válido.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var usuario = await _userManager.FindByEmailAsync(Input.Email);
        if (usuario == null)
            return RedirectToPage("./ResetPasswordConfirmation"); // no revelamos si el email existe

        var resultado = await _userManager.ResetPasswordAsync(usuario, Input.Code, Input.Password);
        if (resultado.Succeeded)
            return RedirectToPage("./ResetPasswordConfirmation");

        foreach (var error in resultado.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
        return Page();
    }
}
