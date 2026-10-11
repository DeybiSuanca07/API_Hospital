namespace Hospital.Application.Orders.Dtos
{
    public record OrderDto(int Id, string PatientId, string PatientName, string ServiceCode, string ServiceDescription, string Priority, string Status);
}