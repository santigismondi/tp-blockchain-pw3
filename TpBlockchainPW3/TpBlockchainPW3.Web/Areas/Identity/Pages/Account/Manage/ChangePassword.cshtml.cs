using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<ChangePasswordModel> _logger;

    public ChangePasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                               ILogger<ChangePasswordModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Ingresá tu contraseña actual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá la contraseña nueva.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña nueva")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repetí la contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Repetí la contraseña nueva")]
        [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        return usuario == null ? NotFound("No se pudo cargar tu cuenta.") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return NotFound("No se pudo cargar tu cuenta.");

        var resultado = await _userManager.ChangePasswordAsync(usuario, Input.OldPassword, Input.NewPassword);
        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        // Renueva la cookie para que la sesión siga activa con la contraseña nueva
        await _signInManager.RefreshSignInAsync(usuario);
        _logger.LogInformation("Un usuario cambió su contraseña.");
        StatusMessage = "Tu contraseña se actualizó.";

        return RedirectToPage();
    }
}
