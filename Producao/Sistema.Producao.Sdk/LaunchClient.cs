using InfraEstrutura.Producao.Server;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Sistema.Producao.Sdk
{
    public class LauchClient : ILaunchClient
    {
        #region Dependencies

        private readonly LaunchRegisterService.LaunchRegisterServiceClient _lancamentoClient;
        private readonly ILogger<LauchClient> _logger;

        public LauchClient(LaunchRegisterService.LaunchRegisterServiceClient lancamentoClient, ILogger<LauchClient> logger)
        {
            _lancamentoClient = lancamentoClient;
            _logger = logger;
        }

        #endregion END Dependencies

        public async Task<LaunchResponse> UpdateLaunchAsync(LaunchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lancamentoClient.UpdateLaunchAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao atualizar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao atualizar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar lançamento: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<DeleteLaunchResponse> DeleteLaunchAsync(LaunchIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Tenta deletar via gRPC
                var response = await _lancamentoClient.DeleteLaunchAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao deletar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao deletar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar lançamento: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<LaunchResponse> RegisterLaunchAsync(LaunchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lancamentoClient.RegisterLaunchAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao registrar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao registrar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }
}
