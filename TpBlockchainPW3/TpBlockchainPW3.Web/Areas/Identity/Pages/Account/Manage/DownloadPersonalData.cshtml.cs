using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TpBlockchainPW3.Web.Data;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class DownloadPersonalDataModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DownloadPersonalDataModel> _logger;

    public DownloadPersonalDataModel(UserManager<IdentityUser> userManager, ApplicationDbContext context,
                                     ILogger<DownloadPersonalDataModel> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public IActionResult OnGet() => NotFound();

    public async Task<IActionResult> OnPostAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return NotFound("No se pudo cargar tu cuenta.");

        _logger.LogInformation("Un usuario descargó sus datos personales.");

        var documentos = await _context.Documentos
            .AsNoTracking()
            .Where(d => d.EmisorId == usuario.Id)
            .OrderBy(d => d.FechaCreacion)
            .Select(d => new
            {
                d.Codigo,
                d.Titulo,
                Tipo = d.Tipo.ToString(),
                Estado = d.Estado.ToString(),
                d.NombreArchivo,
                HuellaSha256 = d.HashSha256,
                d.TxHash,
                d.FechaCreacion,
                d.FechaRegistro
            })
            .ToListAsync();

        var datos = new
        {
            Cuenta = new
            {
                usuario.Id,
                usuario.Email,
                EmailConfirmado = usuario.EmailConfirmed,
                Telefono = usuario.PhoneNumber
            },
            Documentos = documentos
        };

        var opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // deja las tildes legibles
        };

        return File(JsonSerializer.SerializeToUtf8Bytes(datos, opciones), "application/json", "MisDatos-DocChain.json");
    }
}
