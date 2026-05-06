using Microsoft.AspNetCore.Hosting;
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
             services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAuthentcationService, AuthentcationService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPostLikeService, PostLikeService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IPaymentService, PayMobService>();
            services.AddScoped<IMedicalHistoryServices, MedicalHistoryService>();
            services.AddScoped<ITestService, TestService>();
            services.AddHttpClient<IAIService, AIService>();

            services.AddScoped<IImageService,ImageService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IContactService, ContactService>();
            
            return services;
        }
    }
}
