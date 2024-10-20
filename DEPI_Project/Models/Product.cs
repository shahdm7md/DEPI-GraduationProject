using DEPI_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace DEPI_Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        // public string Category { get; set; }
        public String ImageUrl { get; set; }
        public double? DiscountPercentage { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public Store Store { get; set; }

        // Navigation properties
        [ForeignKey("Category")]
        public int CategoryId { get; set; } // Add this line
        public Category Category { get; set; } // Add this line
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        public double GetAverageRating()
        {
            if (Ratings == null || !Ratings.Any())
                return 0;

            return Ratings.Average(r => r.RatingValue);
        }
    }
}
