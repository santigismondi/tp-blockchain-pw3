using Microsoft.AspNetCore.Identity.UI.Services;

namespace TpBlockchainPW3.Web.Areas.Identity;

// La app no tiene un servicio de email real: Identity registra uno "falso" (NoOpEmailSender)
// que no envía nada. Mientras sea así, mostramos los enlaces de confirmación en pantalla.
public static class EmailSimulado
{
    public static bool Activo(IEmailSender sender) => sender.GetType().Name == "NoOpEmailSender";
}
