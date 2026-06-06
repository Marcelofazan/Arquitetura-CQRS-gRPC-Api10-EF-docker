using Grpc.Core;
using InfraEstrutura.Producao.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using Sistema.Producao.Sdk;
using InfraEstrutura.Producao.Business.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Producao.Business.CommandHandlers
{
    public class ApagarClienteHandler : IRequestHandler<ApagarCliente, DeleteLaunchResponse>
    {
        #region Dependencies

        private readonly ILaunchClient _launchClient;
        private readonly ILogger<ApagarClienteHandler> _logger;

        public ApagarClienteHandler(ILaunchClient launchClient
                                      , ILogger<ApagarClienteHandler> logger)
        {
            _launchClient = launchClient;
            _logger = logger;
        }

        #endregion END Dependencies

        public async Task<DeleteLaunchResponse> Handle(ApagarCliente request, CancellationToken cancellationToken)
        {
            try
            {

                var grpRequestExcluir = new LaunchIdRequest { Id = request.Id.ToString() };
                var grpcExcluir = await _launchClient.DeleteLaunchAsync(grpRequestExcluir, cancellationToken);

                return new DeleteLaunchResponse
                {
                    Success = grpcExcluir.Success,
                    Message = grpcExcluir.Message
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro na comunicação com o serviço gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao comunicar com o serviço gRPC", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }
}
