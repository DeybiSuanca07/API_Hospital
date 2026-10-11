using Hospital.Application.Orders.Dtos;
using Hospital.Domain.Entities;

namespace Hospital.Application.Orders.Mappings
{
    public static class OrderMappings
    {
        public static OrderDto ToDto(this Order order) => new(
            order.Id,
            order.PatientId,
            order.PatientName,
            order.ServiceCode,
            order.ServiceDescription,
            order.Priority.ToString(),
            order.Status.ToString());
    }
}