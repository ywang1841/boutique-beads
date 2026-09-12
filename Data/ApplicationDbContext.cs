using BoutiqueBeads.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueBeads.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVideo> ProductVideos => Set<ProductVideo>();
    public DbSet<Tutorial> Tutorials => Set<Tutorial>();
    public DbSet<TutorialProduct> TutorialProducts => Set<TutorialProduct>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TutorialProduct>()
            .HasKey(tp => new { tp.TutorialId, tp.ProductId });

        builder.Entity<TutorialProduct>()
            .HasOne(tp => tp.Tutorial)
            .WithMany(t => t.RelatedProducts)
            .HasForeignKey(tp => tp.TutorialId);

        builder.Entity<TutorialProduct>()
            .HasOne(tp => tp.Product)
            .WithMany(p => p.FeaturedInTutorials)
            .HasForeignKey(tp => tp.ProductId);

        builder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<Tutorial>()
            .HasIndex(t => t.Slug)
            .IsUnique();

        builder.Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique();
    }
}
