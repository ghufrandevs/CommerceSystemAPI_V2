using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace CommerceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/Review")]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReviewController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("AddReview")]
        public IActionResult AddReview(ReviewCreateDTO dto)
        {
            int userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var hasPurchased = _context.OrderProductss
            .Any(op =>
            op.ProductId == dto.ProductId &&
            op.Order.UserId == userId);

            if (!hasPurchased)
            {
                return BadRequest("You can only review products you have purchased");
            }

            var existingReview = _context.Reviews
           .Any(r =>
            r.UserId == userId &&
            r.ProductId == dto.ProductId);

            if (existingReview)
            {
                return BadRequest("You have already reviewed this product");
            }

            Review review = new Review()
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            var product = _context.Products.Find(dto.ProductId);

            product.OverallRating = (decimal)_context.Reviews
                .Where(r => r.ProductId == dto.ProductId)
                .Average(r => r.Rating);

            _context.SaveChanges();

            return Ok("Review Added Successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllReview")]
        public IActionResult GetAllReview()
        {
            var reviews = _context.Reviews.ToList();

            return Ok(reviews);
        }

        [AllowAnonymous]

        [HttpGet("GetReviewById")]
        public IActionResult GetReviewById(int id)
        {
            var review = _context.Reviews.Find(id);

            if (review == null)
            {
                return NotFound("Review Not Found");
            }

            return Ok(review);
        }

        [AllowAnonymous]
        [HttpGet("ViewProductReviews")]
        public IActionResult ViewProductReviews( int productId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }
            var reviews = _context.Reviews
          .Where(r => r.ProductId == productId)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();

            if (reviews.Count == 0)
            {
                return NotFound("No Reviews Found");
            }

            return Ok(reviews);
        }
        [Authorize]
        [HttpPut("UpdateReview")]
        public IActionResult UpdateReview(int id, ReviewUpdateDTO dto)
        {
            var rev = _context.Reviews.Find(id);

            if (rev == null)
            {
                return NotFound("Review Not Found");
            }

            int userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (rev.UserId != userId)
            {
                return Forbid();
            }

            rev.Rating = dto.Rating;
            rev.Comment = dto.Comment;
            rev.ReviewDate = DateTime.Now;

            _context.Reviews.Update(rev);

            _context.SaveChanges();
            var product = _context.Products.Find(rev.ProductId);

            product.OverallRating = (decimal)_context.Reviews
                .Where(r => r.ProductId == rev.ProductId)
                .Average(r => r.Rating);

            _context.SaveChanges();

            return Ok("Review Updated Successfully");

        }
        [Authorize]
        [HttpDelete("DeleteReview")]

        public IActionResult DeleteReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                return NotFound("Review Not Found");
            }
            int userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

           
            if (review.UserId != userId)
            {
                return Forbid();
            }
            int productId = review.ProductId;
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            var product = _context.Products.Find(productId);

            var reviews = _context.Reviews
                .Where(r => r.ProductId == productId);

            if (reviews.Any())
            {
                product.OverallRating =
                    (decimal)reviews.Average(r => r.Rating);
            }
            else
            {
                product.OverallRating = 0;
            }

            _context.SaveChanges();
            return Ok("Review Deleted Successfully");
        }








    }
}
