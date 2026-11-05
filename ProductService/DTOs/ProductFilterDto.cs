using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class ProductFilterDto : IValidatableObject
    {
        public string? Search { get; set; }
        public string? Category { get; set; }      // Por nombre (compatibilidad)
        public int? CategoryId { get; set; }       // ✅ NUEVO: Por ID
        public string? Brand { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MinPrice { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MaxPrice { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? OnDiscount { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; } // "price_asc", "price_desc", "newest", "rating"
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice.Value > MaxPrice.Value)
                yield return new ValidationResult("MinPrice cannot exceed MaxPrice.", new[] { nameof(MinPrice), nameof(MaxPrice) });
        }
    }
}