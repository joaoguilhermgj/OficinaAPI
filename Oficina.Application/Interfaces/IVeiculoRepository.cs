using Oficina.Domain.Entities;

namespace Oficina.Application.Interfaces
{
    public interface IVeiculoRepository
    {
        Task<Veiculo?> ObterPorIdAsync(int id);
        Task<IEnumerable<Veiculo>> ObterTodosAsync();
        Task<bool> PlacaExisteAsync(string placa, int? ignorarId = null);
        Task AdicionarAsync(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Remover(Veiculo veiculo);
        Task<bool> SalvarAlteracoesAsync();
    }
}
