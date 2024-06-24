using StockApp.Domain.Entities;

namespace StockApp.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
    }
}
