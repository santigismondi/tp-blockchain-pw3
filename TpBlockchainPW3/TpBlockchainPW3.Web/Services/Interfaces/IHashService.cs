namespace TpBlockchainPW3.Web.Services.Interfaces;

// Contrato: "dame un archivo y te devuelvo su huella"
public interface IHashService
{
    // Devuelve el SHA-256 en hexadecimal: 64 caracteres en minúscula
    string CalcularHash(byte[] contenido);
}