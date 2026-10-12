using Hospital.Application.Abstractions;
using Hospital.Domain.Enums;
using Hospital.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Hospital.Application.Orders.Commands
{
    public class ProcessPendingOrdersCommandHandler : ICommandHandler<ProcessPendingOrdersCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPendingOrdersCommandHandler> _logger;

        public ProcessPendingOrdersCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ILogger<ProcessPendingOrdersCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(ProcessPendingOrdersCommand command)
        {
            var pendingOrders = await _orderRepository.GetPendingOrdersAsync();

            foreach (var order in pendingOrders)
            {
                var previousStatus = order.Status;

                _logger.LogInformation("Orden {OrderId} ({Priority}) en procesamiento", order.Id, order.Priority);

                await Task.Delay(TimeSpan.FromSeconds(2));

                order.Status = OrderStatus.Procesada;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Orden {OrderId} cambió de estado: {PreviousStatus} -> {NewStatus}", order.Id, previousStatus, order.Status);
            }

            return pendingOrders.Count;
        }
    }
}
