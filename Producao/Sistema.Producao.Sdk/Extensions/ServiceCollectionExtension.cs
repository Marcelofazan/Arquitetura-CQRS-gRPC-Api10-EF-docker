using Microsoft.Extensions.DependencyInjection;
using InfraEstrutura.Producao.Server;

namespace Sistema.Producao.Sdk.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddGrpcSdk(this IServiceCollection services)
        {
            services.AddGrpcClient<LaunchRegisterService.LaunchRegisterServiceClient>(client =>
            {
                client.Address = new Uri("http://localhost:5184");
            });

            services.AddScoped<ILaunchClient, LauchClient>();
        }
    }
}
