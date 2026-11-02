using Microsoft.AspNetCore.Mvc.Rendering;

namespace TpBlockchainPW3.Web.Areas.Identity.Pages.Account.Manage;

// Marca la sección activa del menú de "Mi cuenta". Cada página pone ViewData["ActivePage"].
public static class ManageNavPages
{
    public const string Index = "Index";
    public const string ChangePassword = "ChangePassword";
    public const string PersonalData = "PersonalData";

    public static string? Activa(ViewContext viewContext, string pagina)
    {
        var activa = viewContext.ViewData["ActivePage"] as string
                     ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activa, pagina, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }
}
