using clothes_shop_api.Data.Entities;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using clothes_shop_api.Repositories;
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

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddAutoMapper(typeof(ApplicationMapper).Assembly);

            return services;
        }

    }
}
