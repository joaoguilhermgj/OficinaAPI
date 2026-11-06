using Oficina.Application.DTOs;
using Oficina.Application.Interfaces;
using Oficina.Domain.Entities;
using Oficina.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Oficina.Application.Service
{
    public class VeiculoService
    {
        private readonly IVeiculoRepository _repository;

        public VeiculoService(IVeiculoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VeiculoDto>> ObterTodosVeiculosAsync()
        {
            var veiculos = await _repository.ObterTodosAsync();
            return veiculos.Select(MapToDto);
        }

        public async Task<VeiculoDto?> ObterPorIdAsync(int id)
        {
            var veiculo = await _repository.ObterPorIdAsync(id);
            return veiculo is null ? null : MapToDto(veiculo);
        }

        public async Task <VeiculoDto> CriarAsync(CriarVeiculoDto dto)
        {
            var placa = NormalizarPlaca(dto.Placa);
            ValidarPlaca(placa, dto.Marca, dto.Modelo, dto.Ano, dto.Quilometragem, dto.Tipo);

            if (await _repository.PlacaExisteAsync(placa))
                throw new ArgumentException("Já existe um veículo cadastrado com essa placa.");
            
            var veiculo = new Veiculo
            {
                Placa = placa,
                Marca = dto.Marca.Trim(),
                Modelo = dto.Modelo.Trim(),
                Ano = dto.Ano,
                Cor = (dto.Cor ?? string.Empty).Trim(),
                Quilometragem = dto.Quilometragem,
                Tipo = dto.Tipo,
                Status = StatusManutencao.Disponivel,
            };

            await _repository.AdicionarAsync(veiculo);
            await _repository.SalvarAlteracoesAsync();
            return MapToDto(veiculo);
        }

        public async Task <bool> AtualizarAsync (int id, AtualizarVeiculoDto dto)
        {
            var atualizado = await _repository.ObterPorIdAsync(id);
            if (atualizado is null) return false;
           
            var placa = NormalizarPlaca(dto.Placa);
            ValidarPlaca(placa, dto.Marca, dto.Modelo, dto.Ano, dto.Quilometragem, dto.Tipo);

            if (await _repository.PlacaExisteAsync(placa, id))
                throw new ArgumentException("Já existe um veículo cadastrado com essa placa.");

            if (dto.Quilometragem < atualizado.Quilometragem)
                throw new ArgumentException("A quilometragem não pode ser menor que a atual.");
            
            if (!Enum.IsDefined(dto.Status))
                throw new ArgumentException("O status do veículo é inválido.");

            atualizado.Placa = placa;
            atualizado.Marca = dto.Marca.Trim();
            atualizado.Modelo = dto.Modelo.Trim();
            atualizado.Ano = dto.Ano;
            atualizado.Cor = (dto.Cor ?? string.Empty).Trim();
            atualizado.Quilometragem = dto.Quilometragem;
            atualizado.Tipo = dto.Tipo;
            atualizado.Status = dto.Status;

            _repository.Atualizar(atualizado);
            return await _repository.SalvarAlteracoesAsync();
            
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var veiculo = await _repository.ObterPorIdAsync(id);
            if (veiculo is null) return false;

            _repository.Remover(veiculo);
            return await _repository.SalvarAlteracoesAsync();
        }

        private static string NormalizarPlaca(string placa) =>
            (placa ?? string.Empty).Replace("-", "").Replace(" ", "").ToUpper();

        private static void ValidarPlaca(string placa, string marca, string modelo, int ano, int quilometragem, TipoVeiculo tipo)
        {

            if (placa.Length != 7)
                throw new ArgumentException("A placa deve ter exatamente 7 caracteres.");
            if (string.IsNullOrWhiteSpace(marca))
                throw new ArgumentException("A marca é obrigatória.");
            if (string.IsNullOrWhiteSpace(modelo))
                throw new ArgumentException("O modelo é obrigatório.");
            if (ano < 1886 || ano > DateTime.Now.Year + 1)
                throw new ArgumentException("O ano do veículo é inválido.");
            if (quilometragem < 0)
                throw new ArgumentException("A quilometragem não pode ser negativa.");
            if (!Enum.IsDefined(tipo))
                throw new ArgumentException("O tipo de veículo é inválido.");

        }
        private static VeiculoDto MapToDto(Veiculo v) =>
            new(v.Id, v.Placa, v.Marca, v.Modelo, v.Ano, v.Cor, v.Quilometragem, v.Tipo, v.Status, v.DataCadastro);
    }
}
