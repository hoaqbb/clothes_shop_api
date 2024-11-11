using clothes_shop_api.Data.Entities;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using clothes_shop_api.Repositories;
using clothes_shop_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace clothes_shop_api.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddDbContext<ecommerce_decryptedContext>(opt => 
                opt.UseSqlServer(config.GetConnectionString("SQLServerCon"))
            );

            services.AddSingleton<IConnectionMultiplexer>(opt =>
            {
                var redisConnString = config.GetConnectionString("Redis")
                    ?? throw new Exception("Cannot get redis connection string");
                var configuration = ConfigurationOptions.Parse(redisConnString, true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.Configure<PayPalSettings>(config.GetSection("PayPalSettings"));
            services.AddHttpClient();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<PayPalClient>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICacheService, CacheService>();

            services.AddAutoMapper(typeof(ApplicationMapper).Assembly);

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.Configure<VnPaySettings>(config.GetSection("VNPaySettings"));
            

            return services;
        }

    }
}
