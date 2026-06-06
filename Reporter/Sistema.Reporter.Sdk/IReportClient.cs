using Sistema.Reporter.Server.Protos;

namespace Sistema.Reporter.Sdk
{
    public interface IReportClient
    {
        Task<RelatorioResponse> ObterRelatorioConsolidadoAsync(RelatorioRequest request, CancellationToken cancellationToken);
    }
}
