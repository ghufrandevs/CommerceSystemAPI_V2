using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommerceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/Order")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders.ToList();

            return Ok(orders);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetOrderById")]
        public IActionResult GetOrderById(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
            {
                return NotFound("Order Not Found");
            }

            return Ok(order);
        }

        [Authorize]
        [HttpGet("ViewMyOrders")]
        public IActionResult ViewMyOrders()
        {
            int userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!
                    .Value);

            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .ToList();

            return Ok(orders);
        }

        [Authorize]
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder(PlaceOrderDTO dto)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            var duplicateProducts = dto.Items
           .GroupBy(i => i.ProductId)
           .Any(g => g.Count() > 1);

            if (duplicateProducts)
            {
                return BadRequest(
                    "Duplicate products are not allowed in the same order");
            }
            decimal totalAmount = 0;

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Validation Phase
                foreach (var item in dto.Items)
                {
                    var product = _context.Products.Find(item.ProductId);

                    if (product == null)
                    {
                        return BadRequest("Product Not Found");
                    }

                    if (product.Stock < item.Quantity)
                    {
                        return BadRequest(
                            $"Insufficient stock for product {product.ProductName}");
                    }
                }

                // Create Order
                Order order = new Order()
                {
                    UserId = userId,
                    OrderDate = DateTime.Now
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Execute Order
                foreach (var item in dto.Items)
                {
                    var product = _context.Products.Find(item.ProductId);

                    totalAmount += product.Price * item.Quantity;

                    product.Stock -= item.Quantity;

                    OrderProducts orderProduct = new OrderProducts()
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };

                    _context.OrderProductss.Add(orderProduct);
                }

                order.TotalAmount = totalAmount;

                _context.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    Message = "Order Placed Successfully",
                    OrderId = order.OrderId,
                    TotalAmount = order.TotalAmount
                });
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        [Authorize]
        [HttpGet("GetOrderDetails")]
        public IActionResult GetOrderDetails(int orderId)
        {
            int userId = int.Parse(
           User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var order = _context.Orders.Find(orderId);

            if (order == null)
            {
                return NotFound("Order Not Found");
            }

            if (order.UserId != userId)
            {
                return Forbid();
            }
            var details = _context.OrderProductss

                .Include(op => op.Product)
                .Where(op => op.OrderId == orderId)
                .Select(op => new
                {
                    op.ProductId,
                    op.Product.ProductName,
                    op.Product.Price,
                    op.Quantity
                })
                .ToList();

            if (details.Count == 0)
            {
                return NotFound("No Order Details Found");
            }

            return Ok(details);
        }

        
    }
}
