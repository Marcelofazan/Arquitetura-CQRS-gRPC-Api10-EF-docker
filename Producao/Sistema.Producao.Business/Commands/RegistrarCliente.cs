using InfraEstrutura.Producao.Server;
using MediatR;

namespace InfraEstrutura.Producao.Business.Commands
{
    public record RegistrarCliente(Guid Id, string Nome, string TipoPessoa, string CpfCnpj, string Endereco, double Valor, string DataCadastro) : IRequest<LaunchResponse>;
}
