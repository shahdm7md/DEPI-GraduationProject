using System.ComponentModel.DataAnnotations.Schema;

public class Comment
{
	public int Id { get; set; }
	public string CommentText { get; set; }
	public DateTime CreatedAt { get; set; }

	// Foreign key and navigation property to ApplicationUser
	[ForeignKey("User")]
	public string UserId { get; set; }
	public ApplicationUser User { get; set; }

	// Foreign key and navigation property to Product
	[ForeignKey("Product")]
	public int ProductId { get; set; }
	public Product Product { get; set; }
}
