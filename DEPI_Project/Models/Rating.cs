using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int RatingValue { get; set; }
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
