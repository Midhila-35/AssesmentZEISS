using AssesmentZEISS.IService;
using AssesmentZEISS.Model;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentZEISS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        // PUT: /api/products/decrement-stock/{id}/{quantity}
        [HttpPut("decrement-stock/{id}/{quantity}")]
        public async Task<IActionResult> DecrementStock(string id, int quantity)
        {
            if (!await _productService.ExistsAsync(id))
            {
                return NotFound($"Product with ID '{id}' not found.");
            }

            await _productService.AdjustStockAsync(id, quantity, increase: false);
            return Ok($"Stock decreased by {quantity} for product ID {id}.");
        }

        // PUT: /api/products/add-to-stock/{id}/{quantity}
        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> AddToStock(string id, int quantity)
        {
            if (!await _productService.ExistsAsync(id))
            {
                return NotFound($"Product with ID '{id}' not found.");
            }

            await _productService.AdjustStockAsync(id, quantity, increase: true);
            return Ok($"Stock increased by {quantity} for product ID {id}.");
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.ProductId = await _productService.GenerateUniqueProductIdAsync();
            var createdProduct = await _productService.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }

        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Product product)
        {
            if (id != product.ProductId)
                return BadRequest("Product ID mismatch.");

            if (!await _productService.ExistsAsync(id))
                return NotFound();

            await _productService.UpdateAsync(product);
            return NoContent();
        }

        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteAsync(product);
            return NoContent();
        }

        // PATCH: api/Product/{id}/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> AdjustStock(string id, [FromQuery] int quantity, [FromQuery] bool increase)
        {
            if (!await _productService.ExistsAsync(id))
                return NotFound();

            await _productService.AdjustStockAsync(id, quantity, increase);
            return NoContent();
        }

    }
}
