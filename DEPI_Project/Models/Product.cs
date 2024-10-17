using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public double DiscountPercentage { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("Store")]
    public int StoreId { get; set; }
    public Store Store { get; set; }

    // Navigation properties
    
    public virtual ICollection<Comment> Comments { get; set; }
}
