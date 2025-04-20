using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AssesmentZEISS.Context;
using AssesmentZEISS.Model;
using AssesmentZEISS.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AssesmentZEISS.Tests.Service
{
    public class ProductIdServiceTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<ILogger<ProductIdService>> _mockLogger;
        private readonly ProductIdService _service;

        public ProductIdServiceTests()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockLogger = new Mock<ILogger<ProductIdService>>();
            _service = new ProductIdService(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GenerateUniqueProductIdAsync_ShouldReturnUniqueId()
        {
            // Arrange
            var mockTrackerSet = new Mock<DbSet<ProductIdTracker>>();
            var mockProductSet = new Mock<DbSet<Product>>();

            var tracker = new ProductIdTracker { Id = 1, LastId = 100000 };
            mockTrackerSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tracker);

            mockProductSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockContext.Setup(c => c.ProductIdTrackers).Returns(mockTrackerSet.Object);
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            // Act
            var result = await _service.GenerateUniqueProductIdAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("100001", result);
        }

        [Fact]
        public async Task AdjustStockAsync_ShouldIncreaseStock()
        {
            // Arrange
            var product = new Product { ProductId = "000001", StockAvailable = 10 };
            var mockProductSet = new Mock<DbSet<Product>>();

            mockProductSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            // Act
            await _service.AdjustStockAsync("000001", 5, true);

            // Assert
            Assert.Equal(15, product.StockAvailable);
        }

        [Fact]
        public async Task AdjustStockAsync_ShouldThrowException_WhenStockIsInsufficient()
        {
            // Arrange
            var product = new Product { ProductId = "000001", StockAvailable = 5 };
            var mockProductSet = new Mock<DbSet<Product>>();

            mockProductSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdjustStockAsync("000001", 10, false));
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            var mockProductSet = new Mock<DbSet<Product>>();

            mockProductSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            // Act
            var result = await _service.ExistsAsync("000001");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Arrange
            var mockProductSet = new Mock<DbSet<Product>>();

            mockProductSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            // Act
            var result = await _service.ExistsAsync("000001");

            // Assert
            Assert.False(result);
        }
    }
}
