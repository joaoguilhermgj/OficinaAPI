using Oficina.Domain.Enums;

namespace Oficina.Domain.Entities
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Cor { get; set; } = string.Empty;
        public int Quilometragem { get; set; }
        public TipoVeiculo Tipo { get; set; }
        public StatusManutencao Status { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
