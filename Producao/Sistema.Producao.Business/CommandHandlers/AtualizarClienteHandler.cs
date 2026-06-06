using Grpc.Core;
using InfraEstrutura.Producao.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using InfraEstrutura.Producao.Business.Commands;
using Sistema.Producao.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Producao.Business.CommandHandlers
{
    public class AtualizarClienteHandler : IRequestHandler<AtualizarCliente, LaunchResponse>
    {
        #region Dependencies

        private readonly ILaunchClient _lauchClient;
        private readonly ILogger<AtualizarClienteHandler> _logger;

        public AtualizarClienteHandler(ILaunchClient launchClient
                                      , ILogger<AtualizarClienteHandler> logger)
        {
            _lauchClient = launchClient;
            _logger = logger;
        }

        #endregion END Dependencies

        public async Task<LaunchResponse> Handle(AtualizarCliente request, CancellationToken cancellationToken)
        {
            try
            {
                var lancamentoAtualizar = new LaunchRequest
                {
                    Id = request.Id.ToString(),
                    Name = request.Nome,
                    Typeofperson = request.TipoPessoa,
                    Personalnumber = request.CpfCnpj,
                    Address = request.Endereco,
                    Value = (double)request.Valor,
                    Registrationdate = request.DataCadastro
                };

                var grpcAtualizar = await _lauchClient.UpdateLaunchAsync(lancamentoAtualizar, cancellationToken);

                return new LaunchResponse
                {
                    Id = grpcAtualizar.Id.ToString(),
                    Success = true,
                    Message = "Lançamento atualizado com sucesso"
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro na comunicação com o serviço gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao comunicar com o serviço gRPC", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }
}
