using Grpc.Core;
using InfraEstrutura.Producao.Server;

namespace Sistema.Producao.API.Services
{
    public class ClienteService : LaunchRegisterService.LaunchRegisterServiceBase
    {
        public override Task<LaunchResponse> RegisterLaunch(LaunchRequest request, ServerCallContext context)
        {
            var response = new LaunchResponse
            {
                Id = Guid.NewGuid().ToString(),
                Success = true,
                Message = "Lançamento registrado com sucesso!"
            };

            return Task.FromResult(response);
        }
    }
}
