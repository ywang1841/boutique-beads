using BoutiqueBeads.Data;
using BoutiqueBeads.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Pages.Tutorials;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Tutorial> Tutorials { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tutorials = await _db.Tutorials
            .Where(t => t.IsPublished)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync();
    }
}
