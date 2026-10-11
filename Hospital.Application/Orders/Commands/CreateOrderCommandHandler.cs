using Hospital.Application.Abstractions;
using Hospital.Application.Common.Exceptions;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.Repositories;

namespace Hospital.Application.Orders.Commands
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateOrderCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.PatientId))
                throw new ValidationException("El Id del paciente es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.ServiceCode))
                throw new ValidationException("El código del servicio es obligatorio.");

            if (!Enum.TryParse(command.Priority, ignoreCase: true, out OrderPriority priority) || !Enum.IsDefined(priority))
                throw new ValidationException("La prioridad debe ser Normal o Urgente.");

            Order order = new()
            {
                PatientId = command.PatientId,
                PatientName = command.PatientName,
                ServiceCode = command.ServiceCode,
                ServiceDescription = command.ServiceDescription,
                Priority = priority,
                Status = OrderStatus.Pendiente,
            };

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return order.Id;
        }
    }
}