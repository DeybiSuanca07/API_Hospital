using Hospital.Application.Abstractions;
using Hospital.Domain.Enums;
using Hospital.Domain.Repositories;

namespace Hospital.Application.Orders.Commands
{
    public class ProcessPendingOrdersCommandHandler : ICommandHandler<ProcessPendingOrdersCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessPendingOrdersCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(ProcessPendingOrdersCommand command)
        {
            var pendingOrders = await _orderRepository.GetPendingOrdersAsync();

            foreach (var order in pendingOrders)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));

                order.Status = OrderStatus.Procesada;

                await _unitOfWork.SaveChangesAsync();
            }

            return pendingOrders.Count;
        }
    }
}