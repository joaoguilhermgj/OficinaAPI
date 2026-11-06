using Oficina.Application.Interfaces;
using Oficina.Domain.Entities;
using Oficina.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Oficina.Infrastructure.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly OficinaDbContext _context;

        public VeiculoRepository(OficinaDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Veiculo veiculo) => 
           await _context.Veiculos.AddAsync(veiculo);
        

        public void Atualizar(Veiculo veiculo) => 
            _context.Veiculos.Update(veiculo);
        

        public async Task<Veiculo?> ObterPorIdAsync(int id) =>
            await _context.Veiculos.FindAsync(id);


        public async Task<IEnumerable<Veiculo>> ObterTodosAsync() =>
           await _context.Veiculos.AsNoTracking().ToListAsync();


        public async Task<bool> PlacaExisteAsync(string placa, int? ignorarId = null) =>
            await _context.Veiculos.AnyAsync(v => v.Placa == placa && v.Id != ignorarId);


        public void Remover(Veiculo veiculo) =>
            _context.Veiculos.Remove(veiculo);
       

        public async Task<bool> SalvarAlteracoesAsync() =>
            await _context.SaveChangesAsync() > 0;
        
    }
}
