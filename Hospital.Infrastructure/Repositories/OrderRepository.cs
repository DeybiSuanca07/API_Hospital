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

        public async Task<List<Order>> GetAllAsync(string? patientId, OrderStatus? status)
        {
            IQueryable<Order> query = _context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(patientId))
                query = query.Where(o => o.PatientId == patientId);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query.OrderByDescending(o => o.Id).ToListAsync();
        }
    }
}