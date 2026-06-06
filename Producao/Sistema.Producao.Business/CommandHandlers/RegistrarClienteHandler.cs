using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using InfraEstrutura.Producao.Business.Commands;
using Sistema.Producao.Sdk;
using InfraEstrutura.Producao.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Producao.Business.CommandHandlers
{
    public class RegistrarClienteHandler : IRequestHandler<RegistrarCliente, LaunchResponse>
    {
        #region Dependencies

        private readonly ILaunchClient _lauchClient;
        private readonly ILogger<RegistrarClienteHandler> _logger;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public RegistrarClienteHandler(ILaunchClient lauchClient
                                      , ILogger<RegistrarClienteHandler> logger
                                      , RabbitMQPublisher rabbitMQPublisher)
        {
            _lauchClient = lauchClient;
            _logger = logger;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        #endregion END Dependencies

        public async Task<LaunchResponse> Handle(RegistrarCliente request, CancellationToken cancellationToken)
        {
            try
            {

                var grpcRequest = new LaunchRequest
                {

                    Id = request.Id.ToString(), 
                    Name = request.Nome,
                    Typeofperson = request.TipoPessoa,
                    Personalnumber = request.CpfCnpj,
                    Address = request.Endereco,
                    Value = (double)request.Valor, 
                    Registrationdate = request.DataCadastro
                };

                var grpcResponse = await _lauchClient.RegisterLaunchAsync(grpcRequest, cancellationToken);

                var message = JsonConvert.SerializeObject(request);
                _rabbitMQPublisher.PublishMessage("queue_consolidacao_diaria", message);

                return new LaunchResponse
                {
                    Id = grpcResponse.Id,
                    Success = grpcResponse.Success,
                    Message = grpcResponse.Message
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro na comunicação com o serviço gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao comunicar com o serviço gRPC", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }
}
