using MongoDB.Driver;
using InfraEstrutura.Reporter.DataModels.Data;
using Sistema.Reporter.Server.Protos;
using Grpc.Core;

namespace Sistema.Reporter.Server.Services
{
    public class ReportService : RelatorioService.RelatorioServiceBase
    {
        private readonly IMongoCollection<RelatorioDiario> _relatorioCollection;
        private readonly ReporterAppDbContext _reporterAppDbContext;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IMongoDatabase mongoDatabase, ReporterAppDbContext reporterAppDbContext, ILogger<ReportService> logger)
        {
            _relatorioCollection = mongoDatabase.GetCollection<RelatorioDiario>("relatorios_diarios");
            _reporterAppDbContext = reporterAppDbContext;
            _logger = logger;
        }

        public override async Task<RelatorioResponse> ObterRelatorioConsolidado(RelatorioRequest request, ServerCallContext context)
        {
            // Buscar no MongoDB
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, request.Data);
            var relatorioDiario = await _relatorioCollection.Find(filtro).FirstOrDefaultAsync();

            // Se não encontrar no MongoDB, buscar no PostgreSQL
            if (relatorioDiario == null)
            {
                _logger.LogInformation("Relatório não encontrado no MongoDB. Consultando PostgreSQL...");

                var clientes = _reporterAppDbContext.Clientes
                    .Where(l => l.DataCadastro == request.Data)
                    .ToList();

                if (!clientes.Any())
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Relatório não encontrado."));
                }

                var relatorioResponse = new RelatorioResponse
                {
                    Data = request.Data,
                    SaldoTotal = clientes.Where(l => l.TipoPessoa == "fisica").Sum(l => l.Valor) - clientes.Where(l => l.TipoPessoa == "juridica").Sum(l => l.Valor),
                };

                foreach (var cliente in clientes)
                {
                    relatorioResponse.RelatorioClientes.Add(new Cliente
                    {
                        Id = cliente.Id.ToString(),
                        Name = cliente.Nome,
                        Typeofperson = cliente.TipoPessoa,
                        Personalnumber = cliente.CpfCnpj,
                        Address = cliente.Endereco,
                        Value = cliente.Valor,
                        Registrationdate = cliente.DataCadastro
                    });
                }

                return relatorioResponse;
            }

            // Se encontrado no MongoDB, retorna o relatório consolidado
            var response = new RelatorioResponse
            {
                Data = relatorioDiario.Data,
                SaldoTotal = relatorioDiario.TotalFisica + relatorioDiario.TotalJuridica
            };

            // Adiciona os lançamentos à coleção existente em RelatorioClientes
            foreach (var cliente in relatorioDiario.Clientes)
            {
                response.RelatorioClientes.Add(new Cliente
                {
                    Id = cliente.Id.ToString(),
                    Name = cliente.Nome,
                    Typeofperson = cliente.TipoPessoa,
                    Personalnumber = cliente.CpfCnpj,
                    Address = cliente.Endereco,
                    Value = cliente.Valor,
                    Registrationdate = cliente.DataCadastro
                });
            }

            return response;
        }
    }
}
