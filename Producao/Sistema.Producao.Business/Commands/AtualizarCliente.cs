using InfraEstrutura.Producao.Server;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Producao.Business.Commands
{
    public record AtualizarCliente(Guid Id, string Nome, string TipoPessoa, string CpfCnpj, string Endereco, double Valor, string DataCadastro) : IRequest<LaunchResponse>;
}
