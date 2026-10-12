using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.Repositories;
using Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<(List<Order> Items, int TotalCount)> GetAllAsync(string? patientId, OrderStatus? status, int page, int pageSize)
        {
            IQueryable<Order> query = _context.Orders.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(patientId))
                query = query.Where(o => o.PatientId == patientId);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Order>> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Pendiente)
                .OrderByDescending(o => o.Priority == OrderPriority.Urgente)
                .ToListAsync();
        }
    }
}