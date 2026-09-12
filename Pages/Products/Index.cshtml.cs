using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Pages.Products;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Product> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        Products = await _db.Products
            .Where(p => p.IsActive)
            .Include(p => p.Images)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
