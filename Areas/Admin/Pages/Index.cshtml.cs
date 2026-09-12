using BoutiqueBeads.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Areas.Admin.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int ProductCount { get; set; }
    public int TutorialCount { get; set; }

    public async Task OnGetAsync()
    {
        ProductCount = await _db.Products.CountAsync();
        TutorialCount = await _db.Tutorials.CountAsync(t => t.IsPublished);
    }
}
