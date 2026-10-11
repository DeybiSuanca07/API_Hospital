using Hospital.Application.Abstractions;
using Hospital.Application.Common.Exceptions;
using Hospital.Application.Orders.Dtos;
using Hospital.Application.Orders.Mappings;
using Hospital.Domain.Repositories;

namespace Hospital.Application.Orders.Queries
{
    public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(GetOrderQuery query)
        {
            var order = await _orderRepository.GetByIdAsync(query.Id) ?? throw new NotFoundException($"No existe la orden {query.Id}.");

            return order.ToDto();
        }
    }
}