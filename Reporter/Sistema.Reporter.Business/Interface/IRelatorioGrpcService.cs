using Sistema.Reporter.Server.Protos;

namespace Sistema.Reporter.Business.Interface
{
    public interface IRelatorioGrpcService
    {
        Task<RelatorioResponse> ObterRelatorioConsolidadoAsync(string data, CancellationToken cancellationToken);
    }
}
