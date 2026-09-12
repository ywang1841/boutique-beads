using System.ComponentModel.DataAnnotations;

namespace BoutiqueBeads.Models;

// A "how to make this" tutorial. Shown on its own page with the video plus
// a row of related products the visitor can quick-order (beads, findings, tools).
public class Tutorial
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(170)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    // Link to a hosted video (YouTube, Vimeo, etc.) rather than a file in Blob Storage.
    // e.g. https://www.youtube.com/watch?v=XXXXXXXXXXX or https://youtu.be/XXXXXXXXXXX
    [Required, StringLength(500)]
    public string VideoUrl { get; set; } = string.Empty;

    // Optional override; if left blank the YouTube thumbnail is derived automatically
    // from VideoUrl (see YouTubeHelper).
    [StringLength(1000)]
    public string? ThumbnailUrl { get; set; }

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<TutorialProduct> RelatedProducts { get; set; } = new List<TutorialProduct>();
}
