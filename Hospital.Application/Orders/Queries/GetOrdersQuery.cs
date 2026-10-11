using Hospital.Domain.Enums;

namespace Hospital.Application.Orders.Queries
{
    public record GetOrdersQuery(string? PatientId, OrderStatus? Status);
}