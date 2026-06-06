using Sistema.Reporter.Business.CommandHandlers;
using Sistema.Reporter.Business.Commands;
using Sistema.Reporter.Business.Interface;
using Sistema.Reporter.Server.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Sistema.Reporter.Testes
{
    public class ConsolidacaoDiariaCommandHandlerTestes
    {
        private readonly Mock<IRelatorioGrpcService> _mockGrpcService;
        private readonly Mock<IRelatorioMongoService> _mockMongoService;
        private readonly Mock<IRabbitMQService> _mockRabbitMQService;
        private readonly Mock<ILogger<ConsolidacaoDiariaCommandHandler>> _mockLogger;
        private readonly ConsolidacaoDiariaCommandHandler _handler;

        public ConsolidacaoDiariaCommandHandlerTestes()
        {
            _mockGrpcService = new Mock<IRelatorioGrpcService>();
            _mockMongoService = new Mock<IRelatorioMongoService>();
            _mockRabbitMQService = new Mock<IRabbitMQService>();
            _mockLogger = new Mock<ILogger<ConsolidacaoDiariaCommandHandler>>();

            _handler = new ConsolidacaoDiariaCommandHandler(
                _mockGrpcService.Object,
                _mockMongoService.Object,
                _mockRabbitMQService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_DeveChamarRelatorioGrpcServiceQuandoRelatorioNaoExisteNoMongo()
        {
            // Arrange
            var request = new ConsolidacaoDiariaCommand("2024-10-07");

            // Simula que não existe relatório no MongoDB
            _mockMongoService.Setup(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()))
                             .ReturnsAsync((RelatorioDiario)null);

            var grpcResponse = new RelatorioResponse
            {
                Data = "2026-06-06",
                SaldoTotal = 1000,
                RelatorioClientes = {
                    new Cliente { Id = Guid.NewGuid().ToString(), Name = "Marcelo", Typeofperson = "fisica", Personalnumber = "999.999.999", Address = "R St Sebastian", Value = 500, Registrationdate = "2026-06-06" },
                    new Cliente { Id = Guid.NewGuid().ToString(), Name = "Marcelo", Typeofperson = "juridica", Personalnumber = "999.999.999", Address = "R St Sebastian", Value = 500, Registrationdate = "2026-06-06" }
                }
            };

            _mockGrpcService.Setup(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(1000, result.SaldoTotal);
            _mockGrpcService.Verify(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMongoService.Verify(m => m.InsertOrUpdateClienteAsync(It.IsAny<ClienteDataModel>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarRelatorioDoMongoSeExistir()
        {
            // Arrange
            var request = new ConsolidacaoDiariaCommand("2024-10-07");

            // Simula um relatório existente no MongoDB
            var relatorioDiario = new RelatorioDiario
            {
                Data = "2024-10-07",
                TotalFisica = 500,
                TotalJuridica = 200,
                Clientes =
            [
                new ClienteDataModel { Id = Guid.NewGuid(), Nome = "Marcelo", TipoPessoa = "fisica", CpfCnpj = "999.999.999", Endereco = "R St Sebastian", Valor = 500,  DataCadastro = "2026-06-06" },
                new ClienteDataModel { Id = Guid.NewGuid(), Nome = "Marcelo", TipoPessoa = "juridica", CpfCnpj = "999.999.999", Endereco = "R St Sebastian", Valor = 200, DataCadastro = "2026-06-06" }
            ]
            };

            _mockMongoService.Setup(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()))
                             .ReturnsAsync(relatorioDiario);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(700, result.SaldoTotal);
            _mockGrpcService.Verify(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockMongoService.Verify(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarErroQuandoGrpcFalha()
        {
            // Arrange
            var request = new ConsolidacaoDiariaCommand("2026-06-06");

            // Simula que não existe relatório no MongoDB
            _mockMongoService.Setup(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()))
                             .ReturnsAsync((RelatorioDiario)null);

            // Simula uma falha no gRPC
            _mockGrpcService.Setup(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "Erro de gRPC")));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Sucesso);
            Assert.Equal("Erro ao consultar relatório consolidado.", result.Mensagem);
            _mockGrpcService.Verify(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
