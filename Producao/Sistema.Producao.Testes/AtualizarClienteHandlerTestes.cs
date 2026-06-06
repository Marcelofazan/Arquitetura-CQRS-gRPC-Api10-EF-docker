using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using InfraEstrutura.Producao.Business.CommandHandlers;
using InfraEstrutura.Producao.Business.Commands;
using InfraEstrutura.Producao.Server;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Producao.Sdk;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Sistema.Producao.Testes
{
    public class AtualizarClienteHandlerTestes
    {
        private readonly Mock<ILaunchClient> _mockGrpcClient;
        private readonly AtualizarClienteHandler _handler;
        private readonly Mock<ILogger<AtualizarClienteHandler>> _mockLogger;

        public AtualizarClienteHandlerTestes()
        {
            _mockLogger = new Mock<ILogger<AtualizarClienteHandler>>();
            _mockGrpcClient = new Mock<ILaunchClient>();
            _handler = new AtualizarClienteHandler(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_AtualizarLancamentoGrpcServiceReturnsSuccess_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new AtualizarCliente
            (
                 Guid.NewGuid(),
                "Marcelo",
                "FISICA",
                "999.999.999-99",
                "Rua Avenida",
                100.90,
                "2024-09-21T00:00:00Z"
            );

            var grpcResponse = new LaunchResponse
            {
                Id = command.Id.ToString(),
                Success = true,
                Message = "Lançamento atualizado com sucesso!"
            };

            // Simulando a chamada gRPC com sucesso
            _mockGrpcClient
                .Setup(client => client.UpdateLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento atualizado com sucesso", result.Message);
        }

        [Fact]
        public async Task Handle_AtualizarLancamentoGrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new AtualizarCliente
            (
                 Guid.NewGuid(),
                "Marcelo",
                "FISICA",
                "999.999.999-99",
                "Rua Avenida",
                100.90,
                "2024-09-21T00:00:00Z"
            );

            // Simulando a exceção RpcException
            _mockGrpcClient
                .Setup(client => client.UpdateLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            // Verificando se a exceção correta foi lançada
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);

        }
    }
}

