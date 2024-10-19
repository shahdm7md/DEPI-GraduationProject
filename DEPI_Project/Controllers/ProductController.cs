using DEPI_Project.Data;
using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
	private readonly ApplicationDbContext _context;

	public ProductController(ApplicationDbContext context)
	{
		_context = context;
	}

	// عرض كل المنتجات أو المنتجات بناءً على الـ Category
	public IActionResult Index(int? categoryId)
	{
		var products = _context.Products
			.Where(p => !categoryId.HasValue || p.CategoryId == categoryId)
			.ToList();

		ViewBag.SelectedCategory = categoryId;
		ViewBag.Categories = _context.Categories.ToList();

		return View(products);
	}
}
