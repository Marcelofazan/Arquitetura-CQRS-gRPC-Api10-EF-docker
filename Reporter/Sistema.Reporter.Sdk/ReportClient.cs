using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sistema.Reporter.Server.Protos;

namespace Sistema.Reporter.Sdk
{
    public class ReportClient : IReportClient
    {
        private readonly RelatorioService.RelatorioServiceClient _grpcClient;
        private readonly ILogger<ReportClient> _logger;

        public ReportClient(
            RelatorioService.RelatorioServiceClient relatorioServiceClient
           , ILogger<ReportClient> logger)
        {
            _grpcClient = relatorioServiceClient;
            _logger = logger;
        }
        public async Task<RelatorioResponse> ObterRelatorioConsolidadoAsync(RelatorioRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _grpcClient.ObterRelatorioConsolidadoAsync(request);
                return response;

            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao obter relatorio via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao obter relatório", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter relatorio: {Message}", ex.Message);
                throw;
            }
        }
    }
}
