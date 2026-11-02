using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TpBlockchainPW3.Web.Data;
using TpBlockchainPW3.Web.Models;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IndexModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmado { get; set; }

    // Actividad del emisor
    public int TotalDocumentos { get; set; }
    public int Registrados { get; set; }
    public int SinRegistrar { get; set; }
    public DateTime? UltimoDocumento { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return NotFound("No se pudo cargar tu cuenta.");

        Email = usuario.Email ?? usuario.UserName ?? string.Empty;
        EmailConfirmado = usuario.EmailConfirmed;

        var documentos = _context.Documentos.AsNoTracking().Where(d => d.EmisorId == usuario.Id);
        TotalDocumentos = await documentos.CountAsync();
        Registrados = await documentos.CountAsync(d => d.Estado == EstadoDocumento.Registrado);
        SinRegistrar = TotalDocumentos - Registrados;
        UltimoDocumento = await documentos.MaxAsync(d => (DateTime?)d.FechaCreacion);

        return Page();
    }
}
