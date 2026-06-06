using MediatR;
using Sistema.Reporter.Business.Response;

namespace Sistema.Reporter.Business.Commands
{
    public record ConsolidacaoDiariaCommand(string Data) : IRequest<ConsolidacaoDiariaResponse>;
}
