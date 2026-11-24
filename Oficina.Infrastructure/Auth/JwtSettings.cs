namespace Oficina.Infrastructure.Auth
{
    public class JwtSettings
    {
        public string ChaveSecreta { get; set; } = string.Empty;
        public string Emissor { get; set; } = string.Empty;
        public string PublicoAlvo { get; set; } = string.Empty;
        public int ExpiracaoMinutos { get; set; }   
    }
}
