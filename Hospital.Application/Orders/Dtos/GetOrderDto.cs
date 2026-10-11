namespace Hospital.Application.Orders.Dtos
{
    public class GetOrderDto
    {
        public int? patientId { get; set; }
        public int? status { get; set; }
    }
}