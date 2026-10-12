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
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly IQueryHandler<GetOrderQuery, OrderDto> _getOrderHandler;
        private readonly IQueryHandler<GetOrdersQuery, PagedResult<OrderDto>> _getOrdersHandler;

        public OrdersController(ICommandHandler<CreateOrderCommand, int> createOrderHandler, IQueryHandler<GetOrderQuery, OrderDto> getOrderHandler, IQueryHandler<GetOrdersQuery, PagedResult<OrderDto>> getOrdersHandler)
        {
            _createOrderHandler = createOrderHandler;
            _getOrderHandler = getOrderHandler;
            _getOrdersHandler = getOrdersHandler;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoResponseData>> GetOrders([FromQuery] string? patientId, [FromQuery] OrderStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getOrdersHandler.Handle(new GetOrdersQuery(patientId, status, page, pageSize));

            return Ok(new DtoResponseData
            {
                Message = $"{result.TotalCount} orden(es) encontrada(s). Página {result.Page} de {result.TotalPages}.",
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoResponseData>> CreateOrder(CreateOrderCommand command)
        {
            var id = await _createOrderHandler.Handle(command);
            var order = await _getOrderHandler.Handle(new GetOrderQuery(id));

            return CreatedAtAction(nameof(GetOrderById), new { id }, new DtoResponseData
            {
                Message = "Orden creada - Pendiente de procesamiento",
                Data = order
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DtoResponseData), StatusCodes.Status500InternalServerError)]
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