using Grpc.Core;
using InfraEstrutura.Producao.Server;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Producao.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using static InfraEstrutura.Producao.Server.LaunchRegisterService;

namespace Sistema.Producao.Testes
{
public class RegistrarClienteTestes
    {
        private readonly Mock<LaunchRegisterServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public RegistrarClienteTestes()
        {

            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LaunchRegisterServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LaunchRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Marcelo",
                Typeofperson = "FISICA",
                Personalnumber = "999.999.999-99",
                Address = "Rua Avenida",
                Value = 100.90,
                Registrationdate = "2024-09-21T00:00:00Z"
            };

            var response = new LaunchResponse
            {
                Id = request.Id,
                Success = true,
                Message = "Lançamento registrado com sucesso!"
            };

            // Simulando uma chamada gRPC com sucesso
            var asyncUnaryCall = new AsyncUnaryCall<LaunchResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.RegisterLaunchAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento registrado com sucesso!", result.Message);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LaunchRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Marcelo",
                Typeofperson = "FISICA",
                Personalnumber = "999.999.999-99",
                Address = "Rua Avenida",
                Value = 100.90,
                Registrationdate = "2024-09-21T00:00:00Z"
            };

            // Configurando o mock do AsyncUnaryCall para lançar uma exceção RpcException
            var asyncUnaryCall = new AsyncUnaryCall<LaunchResponse>(
                Task.FromException<LaunchResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.RegisterLaunchAsync(request, CancellationToken.None));

            // Verificando se a mensagem da exceção está correta
            Assert.Equal("Erro ao registrar lançamento", ex.Message);
        }
    }
}
