using BoutiqueBeads.Models;

namespace BoutiqueBeads.Data;

// TEMPORARY: seeds the in-memory database with placeholder JSON-like data so the
// site can be demoed/tested with zero setup (no Azure SQL, no Blob Storage).
// Delete this file and flip Program.cs back to UseSqlServer(...) when you're
// ready to connect a real database.
public static class SeedData
{
    public static void Seed(ApplicationDbContext db)
    {
        if (db.Categories.Any()) return; // already seeded

        var necklaces = new Category { Name = "Necklaces", Slug = "necklaces", Description = "Hand-strung beaded necklaces." };
        var bracelets = new Category { Name = "Bracelets", Slug = "bracelets", Description = "Beaded and wire-wrapped bracelets." };
        db.Categories.AddRange(necklaces, bracelets);
        db.SaveChanges();

        var products = new List<Product>
        {
            new()
            {
                Name = "Ocean Teal Beaded Necklace",
                Slug = "ocean-teal-beaded-necklace",
                Description = "Hand-knotted glass beads in shades of teal, finished with a sterling silver clasp.",
                Price = 38.00m,
                StockQuantity = 12,
                QuickOrderUrl = "https://buy.stripe.com/test_dummy1",
                IsFeatured = true,
                CategoryId = necklaces.Id,
                Images = new List<ProductImage>
                {
                    new() { BlobUrl = "https://placehold.co/600x600/8FBCBB/FFFFFF?text=Teal+Necklace", BlobName = "dummy-1", IsPrimary = true, DisplayOrder = 0 }
                }
            },
            new()
            {
                Name = "Gray Pearl Wrap Bracelet",
                Slug = "gray-pearl-wrap-bracelet",
                Description = "Freshwater pearls in soft gray tones, triple-wrapped on waxed cord.",
                Price = 24.00m,
                StockQuantity = 20,
                QuickOrderUrl = "https://buy.stripe.com/test_dummy2",
                IsFeatured = true,
                CategoryId = bracelets.Id,
                Images = new List<ProductImage>
                {
                    new() { BlobUrl = "https://placehold.co/600x600/4A5859/FFFFFF?text=Pearl+Bracelet", BlobName = "dummy-2", IsPrimary = true, DisplayOrder = 0 }
                }
            },
            new()
            {
                Name = "Seafoam Chip Stone Bracelet",
                Slug = "seafoam-chip-stone-bracelet",
                Description = "Raw chip-cut amazonite stones strung on elastic cord.",
                Price = 18.00m,
                StockQuantity = 30,
                QuickOrderUrl = "https://buy.stripe.com/test_dummy3",
                IsFeatured = false,
                CategoryId = bracelets.Id,
                Images = new List<ProductImage>
                {
                    new() { BlobUrl = "https://placehold.co/600x600/A7CFCB/333333?text=Chip+Stone", BlobName = "dummy-3", IsPrimary = true, DisplayOrder = 0 }
                }
            }
        };
        db.Products.AddRange(products);
        db.SaveChanges();

        var tutorial = new Tutorial
        {
            Title = "How to Knot a Beaded Necklace",
            Slug = "how-to-knot-a-beaded-necklace",
            Description = "A step-by-step look at hand-knotting technique between each bead for a boutique finish.",
            // Placeholder public YouTube video - swap for your own upload.
            VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            IsPublished = true
        };
        db.Tutorials.Add(tutorial);
        db.SaveChanges();

        db.TutorialProducts.AddRange(
            new TutorialProduct { TutorialId = tutorial.Id, ProductId = products[0].Id, DisplayOrder = 0 },
            new TutorialProduct { TutorialId = tutorial.Id, ProductId = products[1].Id, DisplayOrder = 1 }
        );
        db.SaveChanges();
    }
}
