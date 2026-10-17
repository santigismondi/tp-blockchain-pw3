using Microsoft.AspNetCore.Mvc;
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Services.Interfaces;

namespace TpBlockchainPW3.Web.Controllers
{
    public class PdfTestController : ControllerBase
    {
        private readonly IPdfService _pdfService;

        public PdfTestController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpGet("test")]
        public IActionResult TestPdfGeneration()
        {
            var sampleData = new CertificateModel
            {
                FullName = "Ignacio Pascale",
                Dni = "40123456",
                Subject = "Programación Web III",
                Grade = "10 (Diez)",
                Date = DateTime.Now,
                Code = "CERT-2026-TEST-XYZ"
            };

            byte[] pdfBytes = _pdfService.GenerateCertificate(sampleData);

            return File(pdfBytes, "application/pdf", "certificado-prueba.pdf");
        }
    }
}
