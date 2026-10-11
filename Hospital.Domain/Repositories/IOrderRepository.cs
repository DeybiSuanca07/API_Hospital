using Hospital.Domain.Entities;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync(string? patientId, OrderStatus? status);
    }
}