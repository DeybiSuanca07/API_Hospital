using Hospital.Application.Orders.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace API_Hospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        [HttpGet]
        public ActionResult<string> GetOrders([FromQuery] GetOrderDto getOrderDto)
        {
            return $"Patient ID: {getOrderDto.patientId}, Status: {getOrderDto.status}";
        }
    }
}
