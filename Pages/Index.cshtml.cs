using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Product> FeaturedProducts { get; set; } = new();

    public async Task OnGetAsync()
    {
        FeaturedProducts = await _db.Products
            .Where(p => p.IsActive && p.IsFeatured)
            .Include(p => p.Images)
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(8)
            .ToListAsync();
    }
}
