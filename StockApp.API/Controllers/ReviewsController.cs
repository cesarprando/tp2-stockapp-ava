using Microsoft.AspNetCore.Mvc;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsController(
            IReviewRepository reviewRepository
        )
        {
            _reviewRepository = reviewRepository;
        }

        [HttpPost("{productId}/review")]
        public async Task<IActionResult> AddReview(int productId, [FromBody] Review review)
        {
            review.ProductId = productId;
            review.Date = DateTime.Now;

            await _reviewRepository.AddAsync(review);
            return Ok();
        }
    }
}
