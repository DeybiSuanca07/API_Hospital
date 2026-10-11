using Hospital.Application.Abstractions;
using Hospital.Application.Common;
using Hospital.Application.Orders.Commands;
using Hospital.Application.Orders.Dtos;
using Hospital.Application.Orders.Queries;
using Hospital.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API_Hospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly IQueryHandler<GetOrderQuery, OrderDto> _getOrderHandler;
        private readonly IQueryHandler<GetOrdersQuery, List<OrderDto>> _getOrdersHandler;

        public OrdersController(ICommandHandler<CreateOrderCommand, int> createOrderHandler, IQueryHandler<GetOrderQuery, OrderDto> getOrderHandler, IQueryHandler<GetOrdersQuery, List<OrderDto>> getOrdersHandler)
        {
            _createOrderHandler = createOrderHandler;
            _getOrderHandler = getOrderHandler;
            _getOrdersHandler = getOrdersHandler;
        }

        [HttpGet]
        public async Task<ActionResult<DtoResponseData>> GetOrders([FromQuery] string? patientId, [FromQuery] OrderStatus? status)
        {
            var orders = await _getOrdersHandler.Handle(new GetOrdersQuery(patientId, status));

            return Ok(new DtoResponseData
            {
                Message = $"{orders.Count} orden(es) encontrada(s).",
                Data = orders
            });
        }

        [HttpPost]
        public async Task<ActionResult<DtoResponseData>> CreateOrder(CreateOrderCommand command)
        {
            var id = await _createOrderHandler.Handle(command);

            return CreatedAtAction(nameof(GetOrderById), new { id }, new DtoResponseData
            {
                Message = "Orden creada - Pendiente de procesamiento",
                Data = new
                {
                    id,
                    status = OrderStatus.Pendiente.ToString()
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DtoResponseData>> GetOrderById([FromRoute] int id)
        {
            var order = await _getOrderHandler.Handle(new GetOrderQuery(id));

            return Ok(new DtoResponseData
            {
                Message = "Orden encontrada.",
                Data = order
            });
        }
    }
}