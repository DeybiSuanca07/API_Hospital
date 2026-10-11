using Hospital.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Domain.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        [Required]
        public string ServiceCode { get; set; }
        public string ServiceDescription { get; set; }
        public string Priority { get; set; }
        public OrderStatus Status { get; set; }
    }
}