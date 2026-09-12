using System.ComponentModel.DataAnnotations;
using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using BoutiqueBeads.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Areas.Admin.Pages.Products;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IBlobStorageService _blobStorage;

    public CreateModel(ApplicationDbContext db, IBlobStorageService blobStorage)
    {
        _db = db;
        _blobStorage = blobStorage;
    }

    [BindProperty]
    public ProductInput Input { get; set; } = new();

    public List<SelectListItem> CategoryOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return Page();
        }

        var product = new Product
        {
            Name = Input.Name,
            Slug = Slugify(Input.Name),
            Description = Input.Description,
            Price = Input.Price,
            StockQuantity = Input.StockQuantity,
            QuickOrderUrl = Input.QuickOrderUrl,
            IsFeatured = Input.IsFeatured,
            CategoryId = Input.CategoryId,
            IsActive = true
        };

        // Upload each selected photo straight to Blob Storage, then record the
        // resulting URL against the product.
        if (Input.ImageFiles is { Count: > 0 })
        {
            var order = 0;
            foreach (var file in Input.ImageFiles)
            {
                if (file.Length == 0) continue;

                await using var stream = file.OpenReadStream();
                var result = await _blobStorage.UploadAsync(stream, file.FileName, file.ContentType, "products");

                product.Images.Add(new ProductImage
                {
                    BlobUrl = result.Url,
                    BlobName = result.BlobName,
                    DisplayOrder = order,
                    IsPrimary = order == 0,
                    AltText = product.Name
                });
                order++;
            }
        }

        // Optional short video showing the piece (YouTube/Vimeo link, not a file upload)
        if (!string.IsNullOrWhiteSpace(Input.VideoUrl))
        {
            product.Videos.Add(new ProductVideo
            {
                VideoUrl = Input.VideoUrl,
                Title = product.Name
            });
        }

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadCategoriesAsync()
    {
        CategoryOptions = await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();
    }

    private static string Slugify(string name) =>
        name.Trim().ToLowerInvariant().Replace(" ", "-").Replace("'", "");

    public class ProductInput
    {
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        [StringLength(500)]
        public string? QuickOrderUrl { get; set; }

        public bool IsFeatured { get; set; }

        public List<IFormFile>? ImageFiles { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }
    }
}
