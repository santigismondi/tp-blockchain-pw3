using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TpBlockchainPW3.Web.Data;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class DeletePersonalDataModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DeletePersonalDataModel> _logger;

    public DeletePersonalDataModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                                   ApplicationDbContext context, ILogger<DeletePersonalDataModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int CantidadDocumentos { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Ingresá tu contraseña para confirmar.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return NotFound("No se pudo cargar tu cuenta.");

        CantidadDocumentos = await _context.Documentos.CountAsync(d => d.EmisorId == usuario.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return NotFound("No se pudo cargar tu cuenta.");

        CantidadDocumentos = await _context.Documentos.CountAsync(d => d.EmisorId == usuario.Id);

        if (!ModelState.IsValid)
            return Page();

        if (!await _userManager.CheckPasswordAsync(usuario, Input.Password))
        {
            ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
            return Page();
        }

        // Los documentos de la cuenta se borran en cascada (FK con ON DELETE CASCADE)
        var resultado = await _userManager.DeleteAsync(usuario);
        if (!resultado.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "No se pudo eliminar la cuenta. Probá de nuevo.");
            return Page();
        }

        await _signInManager.SignOutAsync();
        _logger.LogInformation("Un usuario eliminó su cuenta.");

        TempData["MensajeExito"] = "Tu cuenta se eliminó.";
        return LocalRedirect("~/");
    }
}
