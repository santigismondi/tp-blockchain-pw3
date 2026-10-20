using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TpBlockchainPW3.Web.Models;
/*using System.Security.Claims;                        
using Microsoft.AspNetCore.Authorization;            
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Services.Interfaces;*/

namespace TpBlockchainPW3.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //DESCOMENTAR PARA PROBAR
        /*public async Task<IActionResult> ProbarBlockchain([FromServices] IBlockchainService blockchain)
        {
            // Hash aleatorio de 32 bytes, para que nunca se repita
            var hash = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

            var txHash = await blockchain.RegistrarAsync(hash);                 // escribe en la blockchain
            var registrada = await blockchain.VerificarAsync(hash);                 // debería decir Existe = true
            var inventada = await blockchain.VerificarAsync(new string('0', 64));  // debería decir Existe = false

            return Json(new { hash, txHash, registrada, inventada });
        }

        // TEMPORAL: borrar antes del PR
        [Authorize] // exige estar logueado
        public async Task<IActionResult> ProbarRegistro([FromServices] IDocumentoService servicio)
        {
            var datos = new NuevoDocumento
            {
                Contenido = System.Text.Encoding.UTF8.GetBytes($"Prueba {DateTime.Now:O}"),
                NombreArchivo = "prueba.txt",
                Titulo = "Documento de prueba",
                Tipo = TipoDocumento.Externo,
                EmisorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            var r = await servicio.RegistrarAsync(datos);
            return Json(new { r.Exito, r.Mensaje, r.Documento?.Codigo, r.Documento?.HashSha256, r.Documento?.TxHash, r.Documento?.Estado });
        }*/

    }
}
