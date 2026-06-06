using InfraEstrutura.Producao.Server;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Producao.Business.Commands
{
    public record ApagarCliente(Guid Id) : IRequest<DeleteLaunchResponse>;
}
