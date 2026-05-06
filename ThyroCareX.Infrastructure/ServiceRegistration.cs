using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using ThyroCareX.Data.Healpers;
using ThyroCareX.Data.Models.Identity;
using ThyroCareX.Infrastructure.Context;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ThyroCareX.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddIdentity<User, IdentityRole<int>>(option =>
            {
                // Password settings.
                option.Password.RequireDigit = true; // لازم يكون فيه رقم واحد على الأقل
                option.Password.RequireLowercase = true; // لازم يكون فيه حرف صغير
                option.Password.RequireNonAlphanumeric = true; // لازم يكون فيه رمز مثل !@#$
                option.Password.RequireUppercase = true; // لازم يكون فيه حرف كبير
                option.Password.RequiredLength = 6; // طول كلمة السر أقل حاجة 6 حروف
                option.Password.RequiredUniqueChars = 1; // لازم يكون فيه حرف فريد واحد على الأقل

                // Lockout settings.
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // لو فشلوا في الدخول أكثر من مرة، الحساب بيتقفل 5 دقايق
                option.Lockout.MaxFailedAccessAttempts = 5; // عدد المحاولات الفاشلة قبل الحظر
                option.Lockout.AllowedForNewUsers = true; // الحظر ينطبق كمان على المستخدمين الجدد

                // User settings.
                option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                // الحروف المسموح بيها في اسم المستخدم
                option.User.RequireUniqueEmail = true; // كل مستخدم لازم يكون عنده إيميل فريد
                option.SignIn.RequireConfirmedEmail = true; // لازم المستخدم يأكد الإيميل قبل الدخول

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


            //Jwt Authentication
            var jwtSettings = new JwtSettings();
            configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer("Bearer", x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = jwtSettings.ValidateIssuer,
                   ValidIssuers = new[] { jwtSettings.Issuer },
                   ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                   ValidAudience = jwtSettings.Audience,
                   ValidateAudience = jwtSettings.ValidateAudience,
                   ValidateLifetime = jwtSettings.ValidateLifeTime,
                   RoleClaimType = ClaimTypes.Role 
               };
           });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DoctorOrAdmin", policy =>
                    policy.RequireRole("Doctor", "Admin"));
            });


            // Add Stripe configuration
            //services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

            // Add PayMob configuration
            services.Configure<PayMobSettings>(configuration.GetSection("PayMobSettings"));

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("FixedPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 100;
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;


                });
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("DefaultCorsPolicy", policy =>
            //    {
            //        policy
            //            .WithOrigins(
            //                "http://localhost:5173",
            //                "https://thyro-care-x-6jdn.vercel.app"
            //            )
            //            .AllowAnyHeader()
            //            .AllowAnyMethod()
            //            
            //    });
            //});

            //Swagger Gen
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ThyroCareX Project", Version = "v1" });
                c.EnableAnnotations();

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
            }
           });
            });
            services.AddHttpContextAccessor();
            //builder.Services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "Catalog API",
            //        Version = "v1",
            //        Description = "Catalog API for E-Commerce Application",
            //        Contact = new OpenApiContact()
            //        {
            //            Name = "Ahmed Ehab",
            //            Email = "ahmedehabahmed31@gmail.com"
            //        }
            //    });
            //});

            return services;
        }
    }
}
