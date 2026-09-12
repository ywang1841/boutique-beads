namespace BoutiqueBeads.Models;

// Many-to-many: a tutorial can feature several buyable products,
// and a product can be featured in several tutorials.
public class TutorialProduct
{
    public int TutorialId { get; set; }
    public Tutorial? Tutorial { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int DisplayOrder { get; set; }
}
