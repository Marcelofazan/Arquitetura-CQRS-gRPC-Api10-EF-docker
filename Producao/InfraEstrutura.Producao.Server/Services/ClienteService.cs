using InfraEstrutura.Producao.DataModels;
using InfraEstrutura.Producao.DataModels.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;


namespace InfraEstrutura.Producao.Server.Services
{
    public class ClienteService : LaunchRegisterService.LaunchRegisterServiceBase
    {
        private readonly ProducaoAppDbContext _context;
        private readonly ILogger<ClienteService> _logger;


        public ClienteService(ProducaoAppDbContext context, ILogger<ClienteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<LaunchResponse> RegisterLaunch(LaunchRequest request, ServerCallContext context)
        {
            try
            {

                var lancamento = new ClienteDataModel
                {

                    Id = Guid.Parse(request.Id),
                    Nome = request.Name,
                    TipoPessoa = request.Typeofperson,
                    CpfCnpj = request.Personalnumber,
                    Endereco = request.Address,
                    Valor = request.Value,
                    DataCadastro = request.Registrationdate

                };

                _context.Clientes.Add(lancamento);
                await _context.SaveChangesAsync();


                return new LaunchResponse
                {
                    Id = lancamento.Id.ToString(),
                    Success = true,
                    Message = "Lançamento registrado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar lançamento: {Message}", ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao registrar o lançamento: {ex.Message}"));
            }
        }

        public override async Task<LaunchResponse> UpdateLaunch(LaunchRequest request, ServerCallContext context)
        {
            try
            {
                var lancamentoExistente = await _context.Clientes
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id));

                if (lancamentoExistente == null)
                {
                    return new LaunchResponse
                    {
                        Success = false,
                        Message = "Lançamento não encontrado"
                    };
                }

                var lancamentoAtualizado = lancamentoExistente with
                {
                    Nome = request.Name,
                    TipoPessoa = request.Typeofperson,
                    CpfCnpj = request.Personalnumber,
                    Endereco = request.Address,
                    Valor = request.Value,
                    DataCadastro = request.Registrationdate
                };

                _context.Clientes.Update(lancamentoAtualizado);
                await _context.SaveChangesAsync();

                return new LaunchResponse
                {
                    Id = lancamentoExistente.Id.ToString(),
                    Success = true,
                    Message = "Lançamento atualizado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar lançamento: {Message}", ex.Message);

                // Retorna uma resposta de erro apropriada
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao atualizar o lançamento: {ex.Message}"));
            }
        }

        public override async Task<DeleteLaunchResponse> DeleteLaunch(LaunchIdRequest request, ServerCallContext context)
        {
            try
            {

                var lancamentoExistente = await _context.Clientes.FindAsync(Guid.Parse(request.Id));

                if (lancamentoExistente == null)
                {
                    return new DeleteLaunchResponse
                    {
                        Success = false,
                        Message = "Lançamento não encontrado"
                    };
                }

                _context.Clientes.Remove(lancamentoExistente);
                await _context.SaveChangesAsync();

                return new DeleteLaunchResponse
                {
                    Success = true,
                    Message = "Lançamento deletado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar lançamento: {Message}", ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao deletar o lançamento: {ex.Message}"));
            }
        }
    }
}
