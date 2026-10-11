using Hospital.Application.Abstractions;
using Hospital.Application.Orders.Commands;
using Hospital.Application.Orders.Dtos;
using Hospital.Application.Orders.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<CreateOrderCommand, int>, CreateOrderCommandHandler>();


            services.AddScoped<IQueryHandler<GetOrderQuery, OrderDto>, GetOrderQueryHandler>();
            services.AddScoped<IQueryHandler<GetOrdersQuery, List<OrderDto>>, GetOrdersQueryHandler>();

            return services;
        }
    }
}