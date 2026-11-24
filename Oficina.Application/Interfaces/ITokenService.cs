namespace Oficina.Application.Interfaces
{
    public interface ITokenService
    {
        string GerarToken(string usuarioId, string email, string nome, IEnumerable<string> roles);
    }
}
