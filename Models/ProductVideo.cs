using System.ComponentModel.DataAnnotations;

namespace BoutiqueBeads.Models;

public class ProductVideo
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    // Link to a hosted video (YouTube, Vimeo, etc.) rather than a file in Blob Storage.
    // e.g. https://www.youtube.com/watch?v=XXXXXXXXXXX or https://youtu.be/XXXXXXXXXXX
    [Required, StringLength(500)]
    public string VideoUrl { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Title { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
