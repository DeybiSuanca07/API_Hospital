using Hospital.Application.Common.Exceptions;
using Hospital.Application.Orders.Commands;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Hospital.Tests.Orders.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _unitOfWorkMock.Object, NullLogger<CreateOrderCommandHandler>.Instance);
        }

        [Fact]
        public async Task Handle_OrdenValida_CreaOrdenPendienteYGuarda()
        {
            // Arrange
            var command = new CreateOrderCommand("12345", "Juan Pérez", "LAB-001", "Hemograma completo", "urgente");

            Order? savedOrder = null;
            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Callback<Order>(order => savedOrder = order).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Callback(() => savedOrder!.Id = 1).ReturnsAsync(1);

            // Act
            var orderId = await _handler.Handle(command);

            // Assert
            Assert.Equal(1, orderId);
            Assert.NotNull(savedOrder);
            Assert.Equal(OrderStatus.Pendiente, savedOrder!.Status);
            Assert.Equal("12345", savedOrder.PatientId);
            Assert.Equal("LAB-001", savedOrder.ServiceCode);

            _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("", "LAB-001", "Normal", "El Id del paciente es obligatorio.")]
        [InlineData("12345", "  ", "Normal", "El código del servicio es obligatorio.")]
        [InlineData("12345", "LAB-001", "Alta", "La prioridad debe ser Normal o Urgente.")]
        [InlineData("12345", "LAB-001", "5", "La prioridad debe ser Normal o Urgente.")]
        public async Task Handle_OrdenInvalida_ValidationExceptionNoGuarda(string patientId, string serviceCode, string priority, string expectedMessage)
        {
            // Arrange
            var command = new CreateOrderCommand(patientId, "Juan Pérez", serviceCode, "Hemograma completo", priority);

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command));

            // Assert
            Assert.Equal(expectedMessage, exception.Message);

            _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
