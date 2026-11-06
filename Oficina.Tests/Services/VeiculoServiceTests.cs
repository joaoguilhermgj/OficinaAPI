using Moq;
using Oficina.Application.DTOs;
using Oficina.Application.Interfaces;
using Oficina.Application.Service;
using Oficina.Domain.Entities;
using Oficina.Domain.Enums;

namespace Oficina.Tests.Services
{
    public class VeiculoServiceTests
    {
        private readonly Mock<IVeiculoRepository> _repositoryMock;
        private readonly VeiculoService _service;

        public VeiculoServiceTests()
        {
            _repositoryMock = new Mock<IVeiculoRepository>();
            _service = new VeiculoService(_repositoryMock.Object);

            _repositoryMock
                .Setup(r => r.PlacaExisteAsync(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(r => r.SalvarAlteracoesAsync())
                .ReturnsAsync(true);
        }

        private static CriarVeiculoDto CriarDtoValido() =>
            new("ABC1D23", "Fiat", "Uno", 2015, "Prata", 85000, TipoVeiculo.Carro);
        private static AtualizarVeiculoDto AtualizarDtoValido() =>
            new("ABC1D23", "Fiat", "Uno", 2015, "Prata", 90000, TipoVeiculo.Carro, StatusManutencao.EmManutencao);

        private static Veiculo VeiculoNoBanco() => new()
        {
            Id = 1,
            Placa = "ABC1D23",
            Marca = "Fiat",
            Modelo = "Uno",
            Ano = 2015,
            Cor = "Prata",
            Quilometragem = 85000,
            Tipo = TipoVeiculo.Carro,
            Status = StatusManutencao.Disponivel
        };

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveSalvarVeiculo()
        {
            await _service.CriarAsync(CriarDtoValido());

            _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Veiculo>()), Times.Once);
            _repositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("abc-1d23")]
        [InlineData("ABC 1D23")]
        [InlineData("abc1d23")]
        public async Task CriarAsync_DeveNormalizarPlaca(string placaDigitada)
        {
            var dto = CriarDtoValido() with { Placa = placaDigitada };

            var resultado = await _service.CriarAsync(dto);

            Assert.Equal("ABC1D23", resultado.Placa);
        }

        [Fact]
        public async Task CriarAsync_DeveCriarComStatusDisponivel()
        {
            var resultado = await _service.CriarAsync(CriarDtoValido());
            Assert.Equal(StatusManutencao.Disponivel, resultado.Status);
        }

        [Fact] 
        public async Task CriarAsync_DeveRemoverEspacosDosTextos()
        {
            var dto = CriarDtoValido() with { Marca = "  Fiat  ", Modelo = " Uno ", Cor = " Prata " };
            var resultado = await _service.CriarAsync(dto);

            Assert.Equal("Fiat", resultado.Marca);
            Assert.Equal("Uno", resultado.Modelo);
            Assert.Equal("Prata", resultado.Cor);
        }

        [Fact]
        public async Task CriarAsync_ComCorNula_DeveSalvarCorVazia()
        {
            var dto = CriarDtoValido() with { Cor = null! };
            var resultado = await _service.CriarAsync(dto);

            Assert.Equal(string.Empty, resultado.Cor);
        }
    }
}
