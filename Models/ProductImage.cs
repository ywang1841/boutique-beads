using System.ComponentModel.DataAnnotations;

namespace BoutiqueBeads.Models;

public class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    // Full public URL returned by Blob Storage after upload
    [Required, StringLength(1000)]
    public string BlobUrl { get; set; } = string.Empty;

    // Blob name (path within the container) - needed to delete/replace the file later
    [Required, StringLength(500)]
    public string BlobName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AltText { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
