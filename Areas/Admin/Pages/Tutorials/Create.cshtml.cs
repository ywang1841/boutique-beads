using System.ComponentModel.DataAnnotations;
using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Areas.Admin.Pages.Tutorials;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public TutorialInput Input { get; set; } = new();

    public List<SelectListItem> ProductOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadProductsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.VideoUrl))
        {
            ModelState.AddModelError(string.Empty, "A tutorial video URL is required.");
        }

        if (!ModelState.IsValid)
        {
            await LoadProductsAsync();
            return Page();
        }

        var tutorial = new Tutorial
        {
            Title = Input.Title,
            Slug = Input.Title.Trim().ToLowerInvariant().Replace(" ", "-"),
            Description = Input.Description,
            VideoUrl = Input.VideoUrl,
            ThumbnailUrl = Input.ThumbnailUrl,
            IsPublished = Input.IsPublished
        };

        if (Input.SelectedProductIds is { Length: > 0 })
        {
            var order = 0;
            foreach (var productId in Input.SelectedProductIds)
            {
                tutorial.RelatedProducts.Add(new TutorialProduct
                {
                    ProductId = productId,
                    DisplayOrder = order++
                });
            }
        }

        _db.Tutorials.Add(tutorial);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadProductsAsync()
    {
        ProductOptions = await _db.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
            .ToListAsync();
    }

    public class TutorialInput
    {
        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }

        [StringLength(1000)]
        public string? ThumbnailUrl { get; set; }

        public int[]? SelectedProductIds { get; set; }

        public bool IsPublished { get; set; } = true;
    }
}
