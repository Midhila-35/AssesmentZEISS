using AssesmentZEISS.Model;

namespace AssesmentZEISS.IService
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(string id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> ExistsAsync(string id);
        Task AdjustStockAsync(string id, int quantity, bool increase);
        Task<string> GenerateUniqueProductIdAsync();
    }
}
