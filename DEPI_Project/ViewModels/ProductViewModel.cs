using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }  // لعرض اسم الفئة في الواجهة

    public string? ImageUrl { get; set; }  // URL للصورة المحفوظة

    [Display(Name = "Product Image")]
    public IFormFile? ImageFile { get; set; }  // الصورة التي يتم رفعها من قبل المستخدم

    public double? DiscountPercentage { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DiscountStartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DiscountEndDate { get; set; }
}
