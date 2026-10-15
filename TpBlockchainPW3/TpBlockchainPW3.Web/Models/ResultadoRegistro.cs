namespace TpBlockchainPW3.Web.Models;

// Lo que devuelve el servicio al intentar registrar
public class ResultadoRegistro
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public Documento? Documento { get; set; }   // el nuevo, o el existente si era duplicado
}