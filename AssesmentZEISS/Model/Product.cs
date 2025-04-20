

using System.ComponentModel.DataAnnotations;

namespace AssesmentZEISS.Model
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; } // 6-digit unique
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, int.MaxValue)]
        public int StockAvailable { get; set; }
        public decimal Price { get; set; }
    }
}
