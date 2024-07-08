using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;

namespace SushiSpace.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _orderService.GetAll();
                if (result is null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-all-by-user/{userid}")]
        public async Task<IActionResult> GetAllByUser([FromRoute] string userid)
        {
            try
            {

                if (userid == null) { return BadRequest(userid); }
                var result = await _orderService.GetAllByUser(userid);
                if (result is null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-order/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                if (!(id > 0))
                {
                    return BadRequest();
                }
                var result = await _orderService.GetOrder(id);
                if (result is null) { return BadRequest(); }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result= await _orderService.DeleteOrder(id);
                if (result)
                {
                    return Ok("Order has been deleted sucssfuly");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDTO order,int id)
        {
            try
            {
                var result= await _orderService.UpdateOrder(order,id);
                if (result is not null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]OrderDTO order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var result=await _orderService.CreateOrder(order);
                if (result is  not null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }
        [HttpPost("create-order-products")]
        public async Task<IActionResult> CreateOrderProducts(List<OrderProductDTO> orderProducts)
        {
            try
            {
                if (orderProducts == null || !orderProducts.Any())
                {
                    return BadRequest("No order products provided.");
                }

                var createdOrderProducts = new List<OrderProduct>();
                foreach (var productDto in orderProducts)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderId = productDto.OrderId,
                        ProductId = productDto.ProductId,
                        Quantity = productDto.Quantity,
                        Price = productDto.Price,
                        // You might set other properties here as needed
                    };

                    createdOrderProducts.Add(orderProduct);
                }

                var result = await _orderService.CreateOrderProducts(createdOrderProducts);
                if (result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to create order products.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }
    }
}
