using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using BoutiqueBeads.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Pages.Tutorials;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DetailsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public Tutorial? Tutorial { get; set; }
    public string? EmbedUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Tutorial = await _db.Tutorials
            .Include(t => t.RelatedProducts)
                .ThenInclude(rp => rp.Product)
                    .ThenInclude(p => p!.Images)
            .FirstOrDefaultAsync(t => t.Id == id && t.IsPublished);

        if (Tutorial is null) return NotFound();

        EmbedUrl = YouTubeHelper.GetEmbedUrl(Tutorial.VideoUrl) ?? Tutorial.VideoUrl;
        return Page();
    }
}
