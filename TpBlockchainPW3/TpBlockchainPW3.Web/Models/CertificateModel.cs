namespace TpBlockchainPW3.Web.Models
{
    public class CertificateModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
