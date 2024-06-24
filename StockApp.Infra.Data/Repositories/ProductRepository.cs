using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;

namespace StockApp.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _productContext;

        public ProductRepository(ApplicationDbContext context)
        {
            _productContext = context;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productContext.Products.ToListAsync();
        }

        public async Task<Product> Create(Product product)
        {
            _productContext.Add(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetById(int? id)
        {
            return await _productContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProducts(int pageNumber, int pageSize)
        {
            return await _productContext.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Remove(int id)
        {
            var product = await _productContext.Products.FindAsync(id);
            if (product is not null)
            {
                _productContext.Products.Remove(product);
                await _productContext.SaveChangesAsync();
            }
        }

        public async Task<Product> Update(Product product)
        {
            _productContext.Update(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold)
        {
            return await _productContext.Products
                .Where(propa => propa.Stock < threshold)
                .ToListAsync();
        }

        public async Task BulkUpdateAsync(IEnumerable<Product> products)
        {
            _productContext.Products.UpdateRange(products);
            await _productContext.SaveChangesAsync();
        }
    }
}
