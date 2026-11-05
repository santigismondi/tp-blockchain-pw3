using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Services.Interfaces;
using TpBlockchainPW3.Web.ViewModels;

namespace TpBlockchainPW3.Web.Controllers
{
    [Authorize]
    public class DocumentosController : Controller
    {
        private readonly IDocumentoService _documentoService;
        private readonly IPdfService _pdfService;

        public DocumentosController(IDocumentoService documentoService, IPdfService pdfService)
        {
            _documentoService = documentoService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var emisorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(emisorId)) return Unauthorized();

            var documentos = await _documentoService.ListarPorEmisorAsync(emisorId);
            return View(documentos);
        }

        [HttpGet]
        public IActionResult Subir()
        {
            return View(new SubirDocumentoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(SubirDocumentoModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emisorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(emisorId))
            {
                return Unauthorized();
            }

            byte[] contenidoArchivo;
            using (var memoryStream = new MemoryStream())
            {
                await model.Archivo.CopyToAsync(memoryStream);
                contenidoArchivo = memoryStream.ToArray();
            }

            // Path.GetFileName descarta rutas que mandan algunos navegadores viejos
            var nombreArchivo = Path.GetFileName(model.Archivo.FileName);
            if (nombreArchivo.Length > 100)
                nombreArchivo = nombreArchivo[^100..]; // los últimos 100: conserva la extensión

            var nuevoDoc = new NuevoDocumento
            {
                Titulo = model.Titulo,
                Tipo = model.Tipo,
                NombreArchivo = nombreArchivo,
                Contenido = contenidoArchivo,
                EmisorId = emisorId,
                NombreDestinatario = model.NombreDestinatario,
                DniDestinatario = model.DniDestinatario,
                Materia = model.Materia,
                Nota = model.Nota
            };

            var resultado = await _documentoService.RegistrarAsync(nuevoDoc);

            // Sin documento, o el duplicado pertenece a otro emisor: error en el mismo formulario
            if (resultado.Documento is null || resultado.Documento.EmisorId != emisorId)
            {
                ModelState.AddModelError(string.Empty, resultado.Documento is null
                    ? resultado.Mensaje
                    : "Este archivo ya fue registrado por otro emisor.");
                return View(model);
            }

            // Éxito → verde. Duplicado propio o falla de blockchain → rojo.
            TempData[resultado.Exito ? "MensajeExito" : "MensajeError"] = resultado.Mensaje;

            return RedirectToAction(nameof(Detalle), new { id = resultado.Documento.Id });
        }

        [HttpGet]
        public IActionResult NuevoCertificado()
        {
            return View(new GenerarCertificadoViewModel { Fecha = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoCertificado(GenerarCertificadoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emisorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(emisorId)) return Unauthorized();

            // 1. El código primero: va impreso en el pie del PDF
            var codigo = await _documentoService.GenerarCodigoAsync(TipoDocumento.Certificado);

            // 2. El PDF se genera UNA sola vez
            var pdfBytes = _pdfService.GenerateCertificate(new CertificateModel
            {
                FullName = model.Nombre,
                Dni = model.Dni,
                Subject = model.Materia,
                Grade = model.Nota!.Value.ToString(),
                Date = model.Fecha!.Value,
                Code = codigo
            });

            // 3. Esos mismos bytes se hashean y se guardan
            var resultado = await _documentoService.RegistrarAsync(new NuevoDocumento
            {
                Titulo = $"Certificado - {model.Nombre}", // máx. 14 + 100 = 114 caracteres
                Tipo = TipoDocumento.Certificado,
                NombreArchivo = $"Certificado_{codigo}.pdf",
                Contenido = pdfBytes,
                EmisorId = emisorId,
                NombreDestinatario = model.Nombre,
                DniDestinatario = model.Dni,
                Materia = model.Materia,
                Nota = model.Nota,
                Codigo = codigo
            });

            // Sin documento, o el duplicado pertenece a otro emisor: error en el mismo formulario
            if (resultado.Documento is null || resultado.Documento.EmisorId != emisorId)
            {
                ModelState.AddModelError(string.Empty, resultado.Documento is null
                    ? resultado.Mensaje
                    : "Este archivo ya fue registrado por otro emisor.");
                return View(model);
            }

            // Éxito → verde. Duplicado propio o falla de blockchain → rojo.
            TempData[resultado.Exito ? "MensajeExito" : "MensajeError"] = resultado.Mensaje;

            return RedirectToAction(nameof(Detalle), new { id = resultado.Documento.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var emisorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(emisorId)) return Unauthorized();

            var documento = await _documentoService.ObtenerAsync(id, emisorId);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        [HttpGet]
        public async Task<IActionResult> Descargar(int id)
        {
            var emisorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(emisorId)) return Unauthorized();

            var documento = await _documentoService.ObtenerAsync(id, emisorId);
            if (documento == null)
            {
                return NotFound();
            }

            if (documento.Contenido == null || documento.Contenido.Length == 0)
            {
                return NotFound("El archivo no contiene datos almacenados.");
            }

            var nombre = documento.NombreArchivo ?? $"documento_{documento.Codigo}.dat";
            var tipoContenido = nombre.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? "application/pdf"
                : "application/octet-stream";

            return File(documento.Contenido, tipoContenido, nombre);
        }
    }
}