using Grpc.Core;
using InfraEstrutura.Producao.Business.CommandHandlers;
using InfraEstrutura.Producao.Business.Commands;
using InfraEstrutura.Producao.Server;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Producao.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Producao.Testes
{
 public class ApagarClienteHandlerTestes
    {
        private readonly Mock<ILaunchClient> _mockGrpcClient;
        private readonly ApagarClienteHandler _handler;
        private readonly Mock<ILogger<ApagarClienteHandler>> _mockLogger;

        public ApagarClienteHandlerTestes()
        {
            _mockLogger = new Mock<ILogger<ApagarClienteHandler>>();
            _mockGrpcClient = new Mock<ILaunchClient>();
            _handler = new ApagarClienteHandler(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_DeletarLancamentoGrpcServiceReturnsSuccess_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new ApagarCliente(Guid.NewGuid());

            var grpcResponse = new DeleteLaunchResponse
            {
                Success = true,
                Message = "Lançamento deletado com sucesso!"
            };

            // Simulando a chamada gRPC com sucesso
            _mockGrpcClient
                .Setup(client => client.DeleteLaunchAsync(It.IsAny<LaunchIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento deletado com sucesso!", result.Message);
        }

        [Fact]
        public async Task Handle_DeletarLancamentoGrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new ApagarCliente(Guid.NewGuid());

            // Simulando a exceção RpcException
            _mockGrpcClient
                .Setup(client => client.DeleteLaunchAsync(It.IsAny<LaunchIdRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            // Verificando se a exceção correta foi lançada
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);
        }
    }
}
