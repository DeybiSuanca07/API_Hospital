namespace Hospital.Application.Orders.Commands
{
    public record CreateOrderCommand(string PatientId, string? PatientName, string ServiceCode, string? ServiceDescription, string Priority);
}