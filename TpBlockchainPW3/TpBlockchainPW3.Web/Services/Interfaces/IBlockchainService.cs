using TpBlockchainPW3.Web.Models;

namespace TpBlockchainPW3.Web.Services.Interfaces;

public interface IBlockchainService
{
    // Anota la huella en la blockchain y devuelve el comprobante (txHash)
    Task<string> RegistrarAsync(string hashHex);

    // Pregunta si la huella está anotada
    Task<ResultadoBlockchain> VerificarAsync(string hashHex);
}