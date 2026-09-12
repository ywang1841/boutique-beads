using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoutiqueBeads.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(170)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    // Optional: a hosted checkout link (e.g. Stripe Payment Link or PayPal button URL)
    // used by the "Quick Order" button on product cards and tutorial pages.
    [StringLength(500)]
    public string? QuickOrderUrl { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    // Videos showing this specific product (close-ups, styling, etc.)
    public ICollection<ProductVideo> Videos { get; set; } = new List<ProductVideo>();

    // "How to make this" tutorials that feature this product as a related buy
    public ICollection<TutorialProduct> FeaturedInTutorials { get; set; } = new List<TutorialProduct>();
}
