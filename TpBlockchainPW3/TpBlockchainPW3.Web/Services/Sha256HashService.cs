using System.Security.Cryptography;
using TpBlockchainPW3.Web.Services.Interfaces;

namespace TpBlockchainPW3.Web.Services;

public class Sha256HashService : IHashService
{
    public string CalcularHash(byte[] contenido)
    {
        // SHA256.HashData viene con .NET: devuelve 32 bytes
        byte[] hash = SHA256.HashData(contenido);

        // Convertimos los 32 bytes a texto hexadecimal (64 caracteres)
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}