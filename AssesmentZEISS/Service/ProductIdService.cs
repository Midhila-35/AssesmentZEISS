using System;
using AssesmentZEISS.Context;
using AssesmentZEISS.IService;
using AssesmentZEISS.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AssesmentZEISS.Service
{
    public class ProductIdService : IProductService
    {
        private readonly AppDbContext _context;
        private static readonly SemaphoreSlim _lock = new(1, 1); // Thread-safe lock for ID generation
        private readonly ILogger<ProductIdService> _logger;
        public ProductIdService(AppDbContext context, ILogger<ProductIdService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Products
                .AsNoTracking()
                .AnyAsync(p => p.ProductId == id);
        }

        public async Task AdjustStockAsync(string id, int quantity, bool increase)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var product = await GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (increase)
            {
                product.StockAvailable += quantity;
            }
            else
            {
                if (product.StockAvailable < quantity)
                    throw new InvalidOperationException("Not enough stock available.");

                product.StockAvailable -= quantity;
            }

            await UpdateAsync(product);
        }

        public async Task<string> GenerateUniqueProductIdAsync()
        {
            _logger.LogInformation("Starting unique Product ID generation.");

            const int maxRetries = 5;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                try
                {
                    var tracker = await _context.ProductIdTrackers.FirstOrDefaultAsync();
                    if (tracker == null)
                    {
                        tracker = new ProductIdTracker { Id = 1, LastId = 100000 };
                        _context.ProductIdTrackers.Add(tracker);
                        await _context.SaveChangesAsync();
                    }

                    tracker.LastId++;
                    var newProductId = tracker.LastId.ToString("D6");

                    if (!await _context.Products.AnyAsync(p => p.ProductId == newProductId))
                    {
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        _logger.LogInformation("Generated unique Product ID: {ProductId}", newProductId);
                        return newProductId;
                    }

                    await transaction.RollbackAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating unique Product ID.");
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                }
            }

            _logger.LogError("Failed to generate a unique Product ID after {MaxRetries} attempts.", maxRetries);
            throw new Exception("Failed to generate a unique Product ID after multiple attempts.");
        }

    }
}
