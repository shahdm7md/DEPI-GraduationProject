using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Collections.Generic;

namespace DEPI_Project.Models.ViewsModels
{
	public class ProductViewModel
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column(Order = 1)]
		public int Id { get; set; }

		[Display(Name = "Product Name")]
		[Required(ErrorMessage = "Name is required.")]
		[RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Name must contain only letters.")]
		[StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
		public string Prod_Name { get; set; }

		[Display(Name = "Description")]
		[Required(ErrorMessage = "Description is required.")]
		public string Prod_Description { get; set; }

		[Display(Name = "Price")]
		[Required(ErrorMessage = "Price is required.")]
		[Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
		public decimal Prod_Price { get; set; }

		//[Display(Name = "Stock")]
		//[Required(ErrorMessage = "Stock is required.")]
		//[Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
		////public int Prod_Stock { get; set; }

		[Display(Name = "Select Product Picture...")]
        //[Required(ErrorMessage = "Product image is required.")]
        [Required]
        public IFormFile Prod_Image { get; set; } // استقبل الصورة هنا

    }
}
