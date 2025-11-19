using Microsoft.Extensions.DependencyInjection;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Infrastructure.InfrastructureBases;
using ThyroCareX.Infrastructure.Repository;

namespace ThyroCareX.Infrastructure
{
    public static class ModuleInfrustructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<IDoctorRepository, DoctorRepository>();

            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));

            return services;

        }
    }
}
