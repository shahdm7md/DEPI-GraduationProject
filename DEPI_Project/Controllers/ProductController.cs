using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DEPI_Project.Data;
using DEPI_Project.Models;
using DEPI_Project.Models.ViewsModels;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly List<string> _allowedExtensions = new() { ".png", ".jpg", ".jpeg", ".gif" };
    private const long _maxAllowedPosterSize = 1048576; // 1 MB

    public ProductController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // عرض كل المنتجات أو حسب الـ Category
    public IActionResult Index(int? categoryId)
    {
        var products = _context.Products
            .Where(p => !categoryId.HasValue || p.CategoryId == categoryId)
            .ToList();

        ViewBag.SelectedCategory = categoryId;
        ViewBag.Categories = _context.Categories.ToList();
        return View(products);
    }

	[Authorize(Roles = "BusinessOwner")]
	public async Task<IActionResult> MyStoreProducts()
	{
		var user = await _userManager.GetUserAsync(User);
		var businessOwner = await _context.BusinessOwners.FirstOrDefaultAsync(b => b.UserId == user.Id);

		if (businessOwner == null)
		{
			return NotFound("Business owner not found.");
		}

		var products = await _context.Products
			.Where(p => p.BusinessOwnerId == businessOwner.Id)
			.ToListAsync();

		return View(products);
	}

	// عرض صفحة إضافة منتج جديد
	[Authorize(Roles = "BusinessOwner")]
	public IActionResult AddProduct()
	{
		ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
		return View(new ProductViewModel());
	}

    [Authorize(Roles = "BusinessOwner")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // طباعة الأخطاء إذا كان ModelState غير صالح
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var businessOwner = await _context.BusinessOwners.FirstOrDefaultAsync(b => b.UserId == user.Id);

        if (businessOwner == null)
        {
            return Unauthorized();
        }

        var files = Request.Form.Files;
        if (!files.Any())
        {
            ModelState.AddModelError("Prod_Image", "Please select a product image.");
            return View(model);
        }

        var file = files.First();
        if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            ModelState.AddModelError("Prod_Image", "Invalid image format.");
            return View(model);
        }

        if (file.Length > _maxAllowedPosterSize)
        {
            ModelState.AddModelError("Prod_Image", "Image size exceeds 1 MB.");
            return View(model);
        }

        // حفظ الصورة
        var imagePath = Path.Combine("wwwroot/images", file.FileName);
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var product = new Product
        {
            Name = model.Prod_Name,
            Description = model.Prod_Description,
            Price = (double)model.Prod_Price,
            ImageUrl = file.FileName,
            CreatedAt = DateTime.Now,
            BusinessOwnerId = businessOwner.Id,
            CategoryId = model.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }

    // عرض صفحة تعديل منتج
    [Authorize(Roles = "BusinessOwner")]
	public async Task<IActionResult> EditProduct(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null || product.BusinessOwnerId != (await GetCurrentBusinessOwnerId()))
		{
			return NotFound();
		}
		ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
		var model = new ProductViewModel
		{
			Id = product.Id,
			Prod_Name = product.Name,
			Prod_Description = product.Description,
			Prod_Price = (decimal)product.Price,
			CategoryId = product.CategoryId
		};

		return View(model);
	}

	// معالجة تعديل منتج
	[Authorize(Roles = "BusinessOwner")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditProduct(ProductViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		var product = await _context.Products.FindAsync(model.Id);
		if (product == null || product.BusinessOwnerId != (await GetCurrentBusinessOwnerId()))
		{
			return NotFound();
		}

		product.Name = model.Prod_Name;
		product.Description = model.Prod_Description;
		product.Price = (double)model.Prod_Price;
		product.CategoryId = model.CategoryId;

		var files = Request.Form.Files;
		if (files.Any())
		{
			var file = files.First();
			if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
			{
				ModelState.AddModelError("Prod_Image", "Invalid image format.");
				return View(model);
			}

			if (file.Length > _maxAllowedPosterSize)
			{
				ModelState.AddModelError("Prod_Image", "Image size exceeds 1 MB.");
				return View(model);
			}

			// حفظ الصورة
			var imagePath = Path.Combine("wwwroot/images", file.FileName);
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			product.ImageUrl = file.FileName; // تحديث مسار الصورة
		}

		_context.Products.Update(product);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(MyStoreProducts));
	}

	// حذف منتج
	[Authorize(Roles = "BusinessOwner")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteProduct(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null || product.BusinessOwnerId != (await GetCurrentBusinessOwnerId()))
		{
			return NotFound();
		}

		_context.Products.Remove(product);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(MyStoreProducts));
	}

	// دالة للحصول على معرف البيزنس أونر الحالي
	private async Task<string> GetCurrentBusinessOwnerId()
	{
		var user = await _userManager.GetUserAsync(User);
		var businessOwner = await _context.BusinessOwners.FirstOrDefaultAsync(b => b.UserId == user.Id);
		return businessOwner?.Id;
	}

}
