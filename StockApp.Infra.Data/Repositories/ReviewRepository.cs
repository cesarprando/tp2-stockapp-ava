using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;

namespace StockApp.Infra.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _reviewContext;

        public ReviewRepository(ApplicationDbContext context)
        {
            _reviewContext = context;
        }

        public async Task AddAsync(Review review)
        {
            _reviewContext.Add(review);
            await _reviewContext.SaveChangesAsync();
        }
    }
}
