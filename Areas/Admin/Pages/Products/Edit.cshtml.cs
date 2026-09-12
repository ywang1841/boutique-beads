using System.ComponentModel.DataAnnotations;
using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using BoutiqueBeads.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Areas.Admin.Pages.Products;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IBlobStorageService _blobStorage;

    public EditModel(ApplicationDbContext db, IBlobStorageService blobStorage)
    {
        _db = db;
        _blobStorage = blobStorage;
    }

    [BindProperty]
    public ProductEditInput Input { get; set; } = new();

    public List<ProductImage> CurrentImages { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return NotFound();

        Input = new ProductEditInput
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            QuickOrderUrl = product.QuickOrderUrl,
            IsFeatured = product.IsFeatured,
            IsActive = product.IsActive
        };
        CurrentImages = product.Images.OrderBy(i => i.DisplayOrder).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == Input.Id);
        if (product is null) return NotFound();

        if (!ModelState.IsValid)
        {
            CurrentImages = product.Images.OrderBy(i => i.DisplayOrder).ToList();
            return Page();
        }

        product.Name = Input.Name;
        product.Description = Input.Description;
        product.Price = Input.Price;
        product.QuickOrderUrl = Input.QuickOrderUrl;
        product.IsFeatured = Input.IsFeatured;
        product.IsActive = Input.IsActive;
        product.UpdatedAtUtc = DateTime.UtcNow;

        if (Input.NewImageFiles is { Count: > 0 })
        {
            var nextOrder = product.Images.Count == 0 ? 0 : product.Images.Max(i => i.DisplayOrder) + 1;
            foreach (var file in Input.NewImageFiles)
            {
                if (file.Length == 0) continue;
                await using var stream = file.OpenReadStream();
                var result = await _blobStorage.UploadAsync(stream, file.FileName, file.ContentType, "products");

                product.Images.Add(new ProductImage
                {
                    BlobUrl = result.Url,
                    BlobName = result.BlobName,
                    DisplayOrder = nextOrder++,
                    IsPrimary = product.Images.Count == 0
                });
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    // Removes a single photo: deletes the blob from storage, then removes the DB row.
    public async Task<IActionResult> OnPostDeleteImageAsync(int id, int imageId)
    {
        var image = await _db.ProductImages.FindAsync(imageId);
        if (image is not null)
        {
            await _blobStorage.DeleteAsync(image.BlobName);
            _db.ProductImages.Remove(image);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { id = Input.Id == 0 ? id : Input.Id });
    }

    public class ProductEditInput
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? QuickOrderUrl { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }

        public List<IFormFile>? NewImageFiles { get; set; }
    }
}
