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

        [Fact] 
        public async Task CriarAsync_ComPlacaDuplicada_DeveLancarExcecao()
        {
            _repositoryMock
                .Setup(r => r.PlacaExisteAsync("ABC1D23", It.IsAny<int?>()))
                .ReturnsAsync(true);

            var dto = CriarDtoValido() with { Placa = "abc-1d23" };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
            _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Theory]
        [InlineData("ABC12")]
        [InlineData("ABC12345")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CriarAsync_ComPlacaDeTamanhoInvalido_DeveLancarExcecao(string? placa)
        {
            var dto = CriarDtoValido() with { Placa = placa! };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CriarAsync_ComMarcaVazia_DeveLancarExcecao(string? marca)
        {
            var dto = CriarDtoValido() with { Marca = marca! };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CriarAsync_ComModeloVazio_DeveLancarExcecao(string? modelo)
        {
            var dto = CriarDtoValido() with { Modelo = modelo! };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Theory]
        [InlineData(1800)]
        [InlineData(1885)]
        public async Task CriarAsync_ComAnoMuitoAntigo_DeveLancarExcecao(int ano)
        {
            var dto = CriarDtoValido() with { Ano = ano };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_ComAnoNoFuturo_DeveLancarExcecao()
        {
            var dto = CriarDtoValido() with { Ano = DateTime.Now.Year + 2 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-1000)]
        public async Task CriarAsync_ComQuilometragemNegativa_DeveLancarExcecao(int quilometragem)
        {
            var dto = CriarDtoValido() with { Quilometragem = quilometragem };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_ComTipoInvalido_DeveLancarExcecao()
        {
            var dto = CriarDtoValido() with { Tipo = (TipoVeiculo)99 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task AtualizarAsync_VeiculoInexistente_DeveRetornarFalse()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Veiculo?)null);

            var resultado = await _service.AtualizarAsync(99, AtualizarDtoValido());

            Assert.False(resultado);
            _repositoryMock.Verify(r => r.Atualizar(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComDadosValidos_DeveAtualizarVeiculo()
        {
            var veiculo = VeiculoNoBanco();
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(veiculo);

            var resultado = await _service.AtualizarAsync(1, AtualizarDtoValido());

            Assert.True(resultado);
            Assert.Equal(90000, veiculo.Quilometragem);
            Assert.Equal(StatusManutencao.EmManutencao, veiculo.Status);
            _repositoryMock.Verify(r => r.Atualizar(veiculo), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComPlacaDeOutroVeiculo_DeveLancarExcecao()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(VeiculoNoBanco());
            _repositoryMock.Setup(r => r.PlacaExisteAsync("ABC1D23", 1)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(1, AtualizarDtoValido()));
        }

        [Fact]
        public async Task AtualizarAsync_ComQuilometragemMenor_DeveLancarExcecao()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(VeiculoNoBanco());

            var dto = AtualizarDtoValido() with { Quilometragem = 80000 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(1, dto));
        }

        [Fact]
        public async Task AtualizarAsync_ComMesmaQuilometragem_DevePermitir()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(VeiculoNoBanco());

            var dto = AtualizarDtoValido() with { Quilometragem = 85000 };

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.True(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_ComStatusInvalido_DeveLancarExcecao()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(VeiculoNoBanco());

            var dto = AtualizarDtoValido() with { Status = (StatusManutencao)99 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(1, dto));
        }

        [Fact]
        public async Task RemoverAsync_VeiculoInexistente_DeveRetornarFalse()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Veiculo?)null);

            var resultado = await _service.RemoverAsync(99);

            Assert.False(resultado);
            _repositoryMock.Verify(r => r.Remover(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact]
        public async Task RemoverAsync_VeiculoExistente_DeveRemover()
        {
            var veiculo = VeiculoNoBanco();
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(veiculo);

            var resultado = await _service.RemoverAsync(1);

            Assert.True(resultado);
            _repositoryMock.Verify(r => r.Remover(veiculo), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_VeiculoInexistente_DeveRetornarNull()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Veiculo?)null);

            var resultado = await _service.ObterPorIdAsync(99);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterTodosVeiculosAsync_DeveRetornarTodosOsVeiculos()
        {
            var veiculos = new List<Veiculo> { VeiculoNoBanco(), VeiculoNoBanco() };
            _repositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(veiculos);

            var resultado = await _service.ObterTodosVeiculosAsync();

            Assert.Equal(2, resultado.Count());
        }
    }
}
