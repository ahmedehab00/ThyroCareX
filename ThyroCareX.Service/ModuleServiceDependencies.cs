using Microsoft.Extensions.DependencyInjection;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;

namespace ThyroCareX.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            // Here you can register your service dependencies
            
             services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IImageService>(sp =>
                new ImageService(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "doctors")));

            return services;
        }
    }
}
