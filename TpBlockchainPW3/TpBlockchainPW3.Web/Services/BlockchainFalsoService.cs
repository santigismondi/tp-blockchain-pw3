using System.Collections.Concurrent;
using System.Security.Cryptography;
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Services.Interfaces;

namespace TpBlockchainPW3.Web.Services;

// Implementación FALSA para desarrollar sin red.
// Guarda las huellas en memoria: se borran al reiniciar la app.
// TODO: Se reemplaza por NethereumBlockchainService cuando esté T09.
public class BlockchainFalsoService : IBlockchainService
{
    // Diccionario seguro para varios pedidos simultáneos
    private readonly ConcurrentDictionary<string, DateTime> _registros = new();

    public async Task<string> RegistrarAsync(string hashHex)
    {
        await Task.Delay(500);   // simula la demora de la red

        if (!_registros.TryAdd(hashHex, DateTime.UtcNow))
            throw new InvalidOperationException("La huella ya estaba registrada.");

        // Inventa un txHash con el mismo formato que uno real
        return "0x" + Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
    }

    public Task<ResultadoBlockchain> VerificarAsync(string hashHex)
    {
        var resultado = _registros.TryGetValue(hashHex, out var fecha)
            ? new ResultadoBlockchain(true, "0xEmisorFalso", fecha)
            : new ResultadoBlockchain(false, null, null);

        return Task.FromResult(resultado);
    }
}