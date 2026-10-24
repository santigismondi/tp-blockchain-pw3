using Microsoft.AspNetCore.Mvc;
using TpBlockchainPW3.Web.Services.Interfaces;
using TpBlockchainPW3.Web.ViewModels;

namespace TpBlockchainPW3.Web.Controllers;

// Sin [Authorize]: cualquiera puede verificar
public class VerificarController : Controller
{
    private const long TamanioMaximo = 10 * 1024 * 1024; // 10 MB

    private readonly IDocumentoService _documentos;
    private readonly ILogger<VerificarController> _logger;

    public VerificarController(IDocumentoService documentos, ILogger<VerificarController> logger)
    {
        _documentos = documentos;
        _logger = logger;
    }

    // GET /Verificar → muestra el formulario vacío
    [HttpGet]
    public IActionResult Index() => View(new VerificarViewModel());

    // POST /Verificar → procesa el archivo
    [HttpPost]
    [ValidateAntiForgeryToken] // protege contra envíos falsos desde otros sitios (CSRF)
    public async Task<IActionResult> Index(VerificarViewModel modelo)
    {
        // Validaciones que las Data Annotations no cubren
        if (modelo.Archivo is { Length: 0 })
            ModelState.AddModelError(nameof(modelo.Archivo), "El archivo está vacío.");
        else if (modelo.Archivo?.Length > TamanioMaximo)
            ModelState.AddModelError(nameof(modelo.Archivo), "El archivo supera los 10 MB.");

        if (!ModelState.IsValid)
            return View(modelo); // vuelve al formulario mostrando los errores

        // IFormFile → byte[] (lo que espera nuestro servicio)
        using var memoria = new MemoryStream();
        await modelo.Archivo!.CopyToAsync(memoria);
        var contenido = memoria.ToArray();

        try
        {
            var resultado = await _documentos.VerificarAsync(contenido);
            return View("Resultado", resultado); // usa Views/Verificar/Resultado.cshtml
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar la blockchain durante una verificación");
            ModelState.AddModelError(string.Empty, "No se pudo consultar la blockchain. Probá de nuevo en unos minutos.");
            return View(modelo);
        }
    }
}