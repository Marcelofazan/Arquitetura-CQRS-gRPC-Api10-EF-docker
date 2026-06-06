using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sistema.Reporter.Business.Commands;
using Sistema.Reporter.Business.Interface;
using Sistema.Reporter.Business.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Reporter.Business.CommandHandlers
{
    public class ConsolidacaoDiariaCommandHandler : IRequestHandler<ConsolidacaoDiariaCommand, ConsolidacaoDiariaResponse>
    {
        private readonly IRelatorioGrpcService _relatorioGrpcService;
        private readonly IRelatorioMongoService _relatorioMongoService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<ConsolidacaoDiariaCommandHandler> _logger;

        public ConsolidacaoDiariaCommandHandler(
            IRelatorioGrpcService relatorioGrpcService,
            IRelatorioMongoService relatorioMongoService,
            IRabbitMQService rabbitMQService,
            ILogger<ConsolidacaoDiariaCommandHandler> logger)
        {
            _relatorioGrpcService = relatorioGrpcService;
            _relatorioMongoService = relatorioMongoService;
            _rabbitMQService = rabbitMQService;
            _logger = logger;

            // Inicia o consumo de mensagens RabbitMQ
            _rabbitMQService.StartListening("queue_consolidacao_diaria", async message =>
            {
                await ProcessMessageAsync(message);
            });
        }

        public async Task ProcessMessageAsync(string message)
        {
            _logger.LogInformation($"Mensagem recebida de RabbitMQ: {message}");

            try
            {
                var cliente = JsonConvert.DeserializeObject<ClienteMessage>(message);
                if (cliente != null)
                {
                    var novoCliente = new ClienteDataModel
                    {
                        Id = cliente.Id,
                        Nome = cliente.Nome,
                        TipoPessoa = cliente.TipoPessoa,
                        CpfCnpj = cliente.CpfCnpj,
                        Endereco = cliente.Endereco,
                        Valor = cliente.Valor,
                        DataCadastro = cliente.DataCadastro
                };

                    // Inserir ou atualizar o lançamento no MongoDB
                    await _relatorioMongoService.InsertOrUpdateClienteAsync(novoCliente, cliente.DataCadastro);

                    // Atualizar totais de créditos e débitos no relatório diário
                    var relatorio = await _relatorioMongoService.GetRelatorioDiarioByDateAsync(cliente.DataCadastro);
                    if (relatorio != null)
                    {
                        var totalFisica = relatorio.Clientes.Where(l => l.TipoPessoa == "fisica").Sum(l => l.Valor);
                        var totalJuridica = relatorio.Clientes.Where(l => l.TipoPessoa == "juridica").Sum(l => l.Valor);

                        await _relatorioMongoService.UpdateTotalClientesAsync(cliente.DataCadastro, totalFisica, totalJuridica);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem recebida de RabbitMQ.");
            }
        }

        public async Task<ConsolidacaoDiariaResponse> Handle(ConsolidacaoDiariaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Busca o relatório diário no MongoDB
                var relatorioDiario = await _relatorioMongoService.GetRelatorioDiarioByDateAsync(request.Data);

                if (relatorioDiario == null)
                {
                    _logger.LogInformation("Dados não encontrados no MongoDB. Consultando serviço gRPC.");

                    // Busca os dados via gRPC
                    var grpcResponse = await _relatorioGrpcService.ObterRelatorioConsolidadoAsync(request.Data, cancellationToken);

                    var consolidacaoResponse = new ConsolidacaoDiariaResponse
                    {
                        Data = grpcResponse.Data,
                        SaldoTotal = grpcResponse.SaldoTotal,
                        Sucesso = true,
                        Clientes = grpcResponse.RelatorioClientes.Select(l => new ClienteDto
                        {
                            Id = l.Id.ToString(),
                            Nome = l.Name,
                            TipoPessoa = l.Typeofperson,
                            CpfCnpj = l.Personalnumber,
                            Endereco = l.Address,
                            Valor = l.Value,
                            DataCadastro = l.Registrationdate
                        }).ToList()
                    };

                    // Monta os lançamentos consolidados
                    var clientesConsolidados = grpcResponse.RelatorioClientes.Select(l => new ClienteDataModel
                    {
                        Id = Guid.NewGuid(),
                        Nome = l.Name,
                        TipoPessoa = l.Typeofperson,
                        CpfCnpj = l.Personalnumber,
                        Endereco = l.Address,
                        Valor = l.Value,
                        DataCadastro = l.Registrationdate
                    }).ToList();

                    // Calculos
                    var totalFisica = clientesConsolidados.Where(l => l.TipoPessoa == "fisica").Sum(l => l.Valor);
                    var totalJuridica = clientesConsolidados.Where(l => l.TipoPessoa == "juridica").Sum(l => l.Valor);

                    // Insere ou atualiza o relatório no MongoDB
                    var novoRelatorioDiario = new RelatorioDiario
                    {
                        Data = grpcResponse.Data,
                        TotalFisica = totalFisica,
                        TotalJuridica = totalJuridica,
                        Clientes = clientesConsolidados
                    };

                    await _relatorioMongoService.InsertOrUpdateClienteAsync(novoRelatorioDiario.Clientes.First(), novoRelatorioDiario.Data);

                    return consolidacaoResponse;
                }

                // Calcula o saldo
                var saldoConsolidado = relatorioDiario.TotalFisica + relatorioDiario.TotalJuridica;

                return new ConsolidacaoDiariaResponse
                {
                    Data = relatorioDiario.Data,
                    SaldoTotal = saldoConsolidado,
                    Sucesso = true,
                    Clientes = relatorioDiario.Clientes.Select(l => new ClienteDto
                    {
                        Id = l.Id.ToString(),
                        Nome = l.Nome,
                        TipoPessoa = l.TipoPessoa,
                        CpfCnpj = l.CpfCnpj,
                        Endereco = l.Endereco,
                        Valor = l.Valor,
                        DataCadastro = l.DataCadastro
                    }).ToList()
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao consultar relatório consolidado.");
                return new ConsolidacaoDiariaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar relatório consolidado."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return new ConsolidacaoDiariaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro inesperado."
                };
            }
        }
    }
}
