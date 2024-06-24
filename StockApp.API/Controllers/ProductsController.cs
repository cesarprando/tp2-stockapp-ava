using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System.Text;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryService _inventoryService;

        public ProductsController(
            IProductRepository productRepository,
            IInventoryService inventoryService
        )
        {
            _productRepository = productRepository;
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productRepository.GetProducts(pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            await _productRepository.Create(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var existingProduct = await _productRepository.GetById(id);
            if (existingProduct is null)
                return NotFound();

            await _productRepository.Update(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product is null)
                return NotFound();

            await _productRepository.Remove(id);
            return NoContent();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLowStock([FromQuery] int threshold)
        {
            var products = await _productRepository.GetLowStockAsync(threshold);
            return Ok(products);
        }

        [HttpPut("bulk-update")]
        public async Task<IActionResult> BulkUpdate([FromBody] IEnumerable<Product> products)
        {
            await _productRepository.BulkUpdateAsync(products);
            return NoContent();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToCsv()
        {
            var products = await _productRepository.GetAll();

            var csv = new StringBuilder()
                .AppendLine("Id,Name,Description,Price,Stock");

            foreach (var product in products)
            {
                csv.AppendLine($"{product.Id},{product.Name},{product.Description},{product.Price},{product.Stock}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "products.csv");
        }

        [HttpPost("replenish-stock")]
        public async Task<IActionResult> ReplenishStock()
        {
            await _inventoryService.ReplenishStockAsync();
            return Ok();
        }
    }
}
