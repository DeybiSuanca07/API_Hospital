using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceDescription { get; set; }
        public OrderPriority Priority { get; set; }
        public OrderStatus Status { get; set; }
    }
}