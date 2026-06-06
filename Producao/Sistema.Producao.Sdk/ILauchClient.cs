using InfraEstrutura.Producao.Server;

namespace Sistema.Producao.Sdk
{
    public interface ILaunchClient
    {
        Task<LaunchResponse> RegisterLaunchAsync(LaunchRequest request, CancellationToken cancellationToken);
        Task<LaunchResponse> UpdateLaunchAsync(LaunchRequest request, CancellationToken cancellationToken);
        Task<DeleteLaunchResponse> DeleteLaunchAsync(LaunchIdRequest request, CancellationToken cancellationToken);
    }
}
