using Hospital.Application.Abstractions;
using Hospital.Application.Orders.Dtos;
using Hospital.Application.Orders.Mappings;
using Hospital.Domain.Repositories;

namespace Hospital.Application.Orders.Queries
{
    public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersQuery query)
        {
            var orders = await _orderRepository.GetAllAsync(query.PatientId, query.Status);

            return orders.Select(o => o.ToDto()).ToList();
        }
    }
}