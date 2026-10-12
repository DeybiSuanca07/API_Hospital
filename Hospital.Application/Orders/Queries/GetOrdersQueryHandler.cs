using Hospital.Application.Abstractions;
using Hospital.Application.Common;
using Hospital.Application.Common.Exceptions;
using Hospital.Application.Orders.Dtos;
using Hospital.Application.Orders.Mappings;
using Hospital.Domain.Repositories;

namespace Hospital.Application.Orders.Queries
{
    public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, PagedResult<OrderDto>>
    {
        private const int MaxPageSize = 100;

        private readonly IOrderRepository _orderRepository;

        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery query)
        {
            if (query.Page < 1)
                throw new ValidationException("El número de página debe ser mayor o igual a 1.");

            if (query.PageSize < 1 || query.PageSize > MaxPageSize)
                throw new ValidationException($"El tamaño de página debe estar entre 1 y {MaxPageSize}.");

            var (orders, totalCount) = await _orderRepository.GetAllAsync(query.PatientId, query.Status, query.Page, query.PageSize);

            return new PagedResult<OrderDto>(orders.Select(o => o.ToDto()).ToList(), query.Page, query.PageSize, totalCount);
        }
    }
}
