using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Services.Interfaces;

namespace TpBlockchainPW3.Web.Services
{
    public class QuestPdfService : IPdfService
    {
        public byte[] GenerateCertificate(CertificateModel data)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Courier));

                    // Lineas de estilos para darle forma al Certificado. Chequear.
                    // Encabezado
                    page.Header()
                        .Text("CERTIFICADO DE APROBACIÓN")
                        .SemiBold()
                        .FontSize(22)
                        .FontColor(Colors.Blue.Darken2)
                        .AlignCenter();

                    // Contenido
                    page.Content()
                        .PaddingVertical(2, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Text("Por medio del presente documento, se certifica que:")
                                .Italic();

                            column.Item().Text(text =>
                            {
                                text.Span("Nombre completo: ").Bold();
                                text.Span(data.FullName);
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("DNI: ").Bold();
                                text.Span(data.Dni);
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("Materia cursada: ").Bold();
                                text.Span(data.Subject);
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("Calificación obtenida: ").Bold();
                                text.Span(data.Grade);
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("Fecha de emisión: ").Bold();
                                text.Span(data.Date.ToString("dd/MM/yyyy"));
                            });

                            column.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                    // Pie de página
                    page.Footer()
                        .AlignCenter()
                        .Column(column =>
                        {
                            column.Spacing(5);
                            column.Item()
                                .Text($"Código de verificación único: {data.Code}")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken1);

                            column.Item()
                                .Text("Documento generado digitalmente.")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Medium);
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
