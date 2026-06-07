using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/Product")]
    public class ProductController:ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddProduct")]
        public IActionResult AddProduct(ProductCreateDTO dto)
        {
            if (_context.Products.Any(p =>
            p.ProductName == dto.ProductName))
            {
                return BadRequest("Product already exists");
            }
            Product product = new Product()
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok("Product Added Successfully");
        }
        [AllowAnonymous]
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products

                .Select(p => new ProductOutputDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    OverallRating = p.OverallRating
                })
             .ToList();

            
            return Ok(products);


        }
        [AllowAnonymous]
        [HttpGet("GetProductById")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products.Find(id);
            if(product == null)

            {
                return NotFound("Product Not Found");

            }
            ProductOutputDTO productOutput = new ProductOutputDTO()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                OverallRating = product.OverallRating
            };
            return Ok(productOutput);
        }
        [AllowAnonymous]
        [HttpGet("FilterProducts")]
        public IActionResult FilterProducts(string search, decimal minPrice, decimal maxPrice, int pageNumber, int pageSize)

        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }
            var products = _context.Products
           .Where(p =>
          (string.IsNullOrEmpty(search) ||
          p.ProductName.Contains(search))
        &&
         p.Price >= minPrice
        &&
        p.Price <= maxPrice)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();
           if (!products.Any())
            {
                return NotFound("No Products Found");
            }
            return Ok(products);
           
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProduct")]
        public IActionResult UpdateProduct(int id , ProductUpdateDTO dto)
        {
            var prod = _context.Products.Find(id);
            if (prod == null)
            {
                return NotFound("product not found");

            }
            if (_context.Products.Any(p =>
            p.ProductName == dto.ProductName &&
            p.ProductId != id))
            {
                return BadRequest("Product already exists");
            }
            prod.ProductName=dto.ProductName;
            prod.Description= dto.Description;
            prod.Price = dto.Price;
            prod.Stock = dto.Stock;

            _context.Products.Update(prod);
            _context.SaveChanges();
            return Ok("Product Updated Successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteProduct")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound("Product Not Found");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok("Product Deleted Successfully");
        }

    }

    
}
