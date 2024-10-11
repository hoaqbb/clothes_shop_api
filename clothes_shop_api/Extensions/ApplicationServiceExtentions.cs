using clothes_shop_api.Data.Entities;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using clothes_shop_api.Repositories;
using clothes_shop_api.Services;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddDbContext<ecommerceContext>(opt => 
                opt.UseSqlServer(config.GetConnectionString("SQLServerCon"))
            );
            services.Configure<PayPalSettings>(config.GetSection("PayPalSettings"));
            services.AddHttpClient();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<PayPalClient>();

            services.AddAutoMapper(typeof(ApplicationMapper).Assembly);

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.Configure<VnPaySettings>(config.GetSection("VNPaySettings"));
            

            return services;
        }

    }
}
