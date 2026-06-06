using Grpc.Core;
using InfraEstrutura.Producao.Business;
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
 public class RegistrarClienteHandlerTestes
    {
        private readonly Mock<ILaunchClient> _mockGrpcClient;
        private readonly RegistrarClienteHandler _handler;
        private readonly Mock<ILogger<RegistrarClienteHandler>> _mockLogger;
        private readonly Mock<RabbitMQPublisher> _mockRabbitMQPublisher;

        public RegistrarClienteHandlerTestes()
        {
            _mockLogger = new Mock<ILogger<RegistrarClienteHandler>>();
            _mockGrpcClient = new Mock<ILaunchClient>();
            _mockRabbitMQPublisher = new Mock<RabbitMQPublisher>();

            _mockRabbitMQPublisher
              .Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>()))
              .Verifiable();

            _handler = new RegistrarClienteHandler(_mockGrpcClient.Object, _mockLogger.Object, _mockRabbitMQPublisher.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new RegistrarCliente(
                 Guid.NewGuid(),
                "Marcelo",
                "FISICA",
                "999.999.999-99",
                "Rua Avenida",
                100.90,
                "2024-09-21T00:00:00Z"
            );

            var grpcResponse = new LaunchResponse { Success = true };

            _mockGrpcClient.Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockRabbitMQPublisher.Verify(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new RegistrarCliente(
                 Guid.NewGuid(),
                "Gustavo",
                "FISICA",
                "999.999.999-99",
                "Rua Avenida",
                200.59,
                "2024-09-21T00:00:00Z"
            );
            _mockGrpcClient.Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);
        }

    }
}
