using TpBlockchainPW3.Web.Models;

namespace TpBlockchainPW3.Web.Services.Interfaces;

public interface IDocumentoService
{
    Task<ResultadoRegistro> RegistrarAsync(NuevoDocumento datos);
    Task<List<Documento>> ListarPorEmisorAsync(string emisorId);
    Task<Documento?> ObtenerAsync(int id, string emisorId);
    Task<ResultadoVerificacion> VerificarAsync(byte[] contenido);
    Task<Documento?> ObtenerPorCodigoAsync(string codigo);
}