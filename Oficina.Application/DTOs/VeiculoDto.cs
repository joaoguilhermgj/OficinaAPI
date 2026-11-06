using Oficina.Domain.Enums;

namespace Oficina.Application.DTOs
{
     public record CriarVeiculoDto(string Placa, string Marca, string Modelo, int Ano, string Cor,  int Quilometragem, TipoVeiculo Tipo);
     public record AtualizarVeiculoDto(string Placa, string Marca, string Modelo, int Ano, string Cor, int Quilometragem, TipoVeiculo Tipo, StatusManutencao Status);
     public record VeiculoDto(int Id, string Placa, string Marca, string Modelo, int Ano, string Cor, int Quilometragem, TipoVeiculo Tipo, StatusManutencao Status, DateTime DataCadastro);

}
