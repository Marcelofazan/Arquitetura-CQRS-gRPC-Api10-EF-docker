

namespace Sistema.Reporter.Business.Interface
{
    public interface IRelatorioMongoService
    {
        Task<RelatorioDiario?> GetRelatorioDiarioByDateAsync(string data);
        Task InsertOrUpdateClienteAsync(ClienteDataModel cliente, string data);
        Task UpdateTotalClientesAsync(string data, double totalFisica, double totalJuridica);
    }
}
