using TpBlockchainPW3.Web.Models;

namespace TpBlockchainPW3.Web.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateCertificate(CertificateModel data);
    }
}
