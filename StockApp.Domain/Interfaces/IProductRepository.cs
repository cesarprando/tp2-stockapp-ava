using StockApp.Domain.Entities;

namespace StockApp.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetProducts(int pageNumber, int pageSize);
        Task<Product> GetById(int? id);
        Task<Product> Create(Product product);
        Task<Product> Update(Product product);
        Task Remove(int id);
        Task BulkUpdateAsync(IEnumerable<Product> products);
        Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
    }
}
