using Hospital.Domain.Repositories;
using Hospital.Infrastructure.BackgroundJobs;
using Hospital.Infrastructure.Data;
using Hospital.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencys(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHostedService<OrderProcessingWorker>();

            return services;
        }
    }
}