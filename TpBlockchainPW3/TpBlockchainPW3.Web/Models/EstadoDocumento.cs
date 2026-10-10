namespace TpBlockchainPW3.Web.Models;

public enum EstadoDocumento
{
    Pendiente = 0,   // guardado, esperando confirmación de la red
    Registrado = 1,  // la red confirmó el registro
    Error = 2        // falló el registro; se puede reintentar
}