namespace DEPI_Project.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }

		// Navigation property
		public virtual ICollection<Product> Products { get; set; }
	}
}
